﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.GPU;
using Tmc.Common;


namespace Tmc.Vision
{
    public abstract class VisionBase
    {
        /// <summary>
        /// This is used for lower and higher HSV values
        /// </summary>
        public enum HSVRange { Low = 0, High };

        public enum HSVdata { Hue = 0, Sat, Val };

        public enum TC { Black = 0, White, Red, Blue, Green, Bad, Na };


        /// <summary>
        /// This Function lets us detect circles in an image
        /// </summary>
        /// <returns>
        /// it return an CircleF[] which is an array of circles which detected in the image
        /// </returns>
        /// <param name="src">
        /// THis is the input image in which we want to find the circles
        /// </param>
        /// <param name="minRadius">
        /// smallest circle diameter that will be detected, note this is in pixcels
        /// </param>
        /// <param name="maxRadius">
        /// biggest circle diameter that will be detected, note this is in pixcels
        /// </param>
        /// <param name="cannyThresh">
        /// 
        /// </param>
        /// <param name="cannyAccumThresh">
        /// 
        /// </param>
        public CircleF[] DetectTablets(Image<Bgr, Byte> src, int minRadius, int maxRadius, double dp, double minDist, int cannyThresh, int cannyAccumThresh, Form1 f)
        {//double dp, double minDist
            Image<Gray, Byte> graySoft = src.Convert<Gray, Byte>();
            Image<Gray, Byte> gray = graySoft;

            Gray cannyThreshold = new Gray(cannyThresh);
            Gray cannyThresholdLinking = new Gray(100);
            Gray circleAccumulatorThreshold = new Gray(cannyAccumThresh);

            Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold.Intensity, cannyThresholdLinking.Intensity);

            f.pictureBox1_draw(gray);
            CircleF[] circles = gray.HoughCircles(
                cannyThreshold,
                circleAccumulatorThreshold,
                dp,
                minDist,
                minRadius,
                maxRadius
                )[0]; //Get the circles from the first channel

            #if true
            Image<Bgr, Byte> a = src.Clone();
            
            Image<Gray, Byte> imageL = a[1];
            imageL._EqualizeHist();
            a[1] = imageL;

            Image<Gray, Byte> imageL2 = a[2];
            imageL2._EqualizeHist();
            a[2] = imageL2;

            Image<Gray, Byte> imageL3 = a[0];
            imageL3._EqualizeHist();
            a[0] = imageL3;

            foreach (CircleF circle in circles)
            {
                a.Draw(circle, new Bgr(Color.Red), 2);
            }

            

            f.pictureBox2_draw(a);

            #endif
            CvInvoke.cvWaitKey(40);//remove move later
            return circles;
        }
        /*
        public void calibration(Image<Bgr, byte> src)
        {
            const int width = 9;
            const int height = 6;
            Size patternSize = new Size(width, height);
            Bgr[] line_colour_array = new Bgr[width * height]; // just for displaying coloured lines of detected chessboard
            Image<Gray, Byte>[] Frame_array_buffer = new Image<Gray, byte>[100];
            MCvPoint3D32f[][] corners_object_list = new MCvPoint3D32f[Frame_array_buffer.Length][];
            PointF[][] corners_points_list = new PointF[Frame_array_buffer.Length][];
        }*/


        /// <summary>
        /// This function tells us what color the source image is
        /// </summary>
        /// <param name="src">
        /// Input image of whichw e want to determin the color
        /// </param>
        /// <param name="HSVTabletColourRange">
        /// 2d array containing all the color ranges of tablets also has min and max for each colour, uses HSV color range
        /// </param>
        /// <returns>
        /// Returns the colour of the tablet with enum TabletColors
        /// </returns>
        /// <todo>
        /// add the ol, oh into the function
        /// </todo>
        public TabletColors detectColour(Image<Bgr, byte> src, Hsv[,] HSVTabletColourRange)
        {
            int ol = 5;
            int oh = 5;
            MCvScalar srcScalar;

            Image<Hsv, byte> hsv = src.Convert<Hsv, byte>();
            Hsv abc;
            hsv.AvgSdv(out abc, out srcScalar);
            //TabletColors
            if (true == InHSVRange(abc, HSVTabletColourRange, TabletColors.Green, ol, oh))
            {//green
                return TabletColors.Green;
            }
            else if (true == InHSVRange(abc, HSVTabletColourRange, TabletColors.Red, ol, oh))
            {//red
                return TabletColors.Red;
            }
            else if (true == InHSVRange(abc, HSVTabletColourRange, TabletColors.White, ol, oh))
            {//white
                return TabletColors.White;
            }
            else if (true == InHSVRange(abc, HSVTabletColourRange, TabletColors.Blue, ol, oh))
            {//blue
                return TabletColors.Blue;
            }
            else if (true == InHSVRange(abc, HSVTabletColourRange, TabletColors.Black, ol, oh))
            {//black
                return TabletColors.Black;
            }
            else
            {
                return TabletColors.Unknown;
            }
        }

        /// <summary>
        /// Tells us if the the HSV color is in range
        /// </summary>
        /// <param name="srcHsv">
        /// The HSV values we want to check are in range
        /// </param>
        /// <param name="targetHsv">
        /// The range that we want the color to be in
        /// </param>
        /// <param name="colour">
        /// THe color that we want it to be in range of since the 'targetHsv' can be indexed for colors
        /// </param>
        /// <param name="lowerLimitExtra">
        /// how much extra lower we want se the lower side of the range
        /// </param>
        /// <param name="higherLimitExtra">
        /// How much higher we want o set higer side of the range
        /// </param>
        /// <returns>
        /// returns true if it is in range, false if it is not
        /// </returns>
        /// <todo>
        /// make lowerLimitsExtra in to HSV or make overload for it
        /// </todo>
        public bool InHSVRange(Hsv srcHsv, Hsv[,] targetHsv, TabletColors colour, int lowerLimitExtra, int higherLimitExtra)
        {
            if (targetHsv[(int)colour, (int)HSVRange.Low].Hue < targetHsv[(int)colour, (int)HSVRange.High].Hue)
            {
                return ((srcHsv.Hue >= (targetHsv[(int)colour, (int)HSVRange.Low].Hue - lowerLimitExtra)) &&             //lower values
                (srcHsv.Satuation >= (targetHsv[(int)colour, (int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                (srcHsv.Value >= (targetHsv[(int)colour, (int)HSVRange.Low].Value - lowerLimitExtra)) &&
                (srcHsv.Hue <= (targetHsv[(int)colour, (int)HSVRange.High].Hue + higherLimitExtra)) &&               //higher values
                (srcHsv.Satuation <= (targetHsv[(int)colour, (int)HSVRange.High].Satuation + higherLimitExtra)) &&
                (srcHsv.Value <= (targetHsv[(int)colour, (int)HSVRange.High].Value + higherLimitExtra)));
            }
            else
            {
                if (srcHsv.Hue >= (targetHsv[(int)colour, (int)HSVRange.Low].Hue - lowerLimitExtra))
                {
                    return ((srcHsv.Hue >= (targetHsv[(int)colour, (int)HSVRange.Low].Hue - lowerLimitExtra)) &&             //lower values
                    (srcHsv.Satuation >= (targetHsv[(int)colour, (int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                    (srcHsv.Value >= (targetHsv[(int)colour, (int)HSVRange.Low].Value - lowerLimitExtra)) &&
                    (srcHsv.Hue <= (179)) &&               //higher values
                    (srcHsv.Satuation <= (targetHsv[(int)colour, (int)HSVRange.High].Satuation + higherLimitExtra)) &&
                    (srcHsv.Value <= (targetHsv[(int)colour, (int)HSVRange.High].Value + higherLimitExtra)));
                }
                else if (srcHsv.Hue <= (targetHsv[(int)colour, (int)HSVRange.High].Hue + higherLimitExtra))
                {
                    return ((srcHsv.Hue >= (0)) &&             //lower values
                    (srcHsv.Satuation >= (targetHsv[(int)colour, (int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                    (srcHsv.Value >= (targetHsv[(int)colour, (int)HSVRange.Low].Value - lowerLimitExtra)) &&
                    (srcHsv.Hue <= (targetHsv[(int)colour, (int)HSVRange.High].Hue + higherLimitExtra)) &&               //higher values
                    (srcHsv.Satuation <= (targetHsv[(int)colour, (int)HSVRange.High].Satuation + higherLimitExtra)) &&
                    (srcHsv.Value <= (targetHsv[(int)colour, (int)HSVRange.High].Value + higherLimitExtra)));
                }
                else return false;
            }
        }

        /// <summary>
        /// Tells us if the the HSV color is in range
        /// </summary>
        /// <param name="srcHsv">
        /// The HSV values we want to check are in range
        /// </param>
        /// <param name="targetHsv">
        /// The range that we want the color to be in
        /// </param>
        /// <param name="lowerLimitExtra">
        /// how much extra lower we want se the lower side of the range
        /// </param>
        /// <param name="higherLimitExtra">
        /// How much higher we want o set higer side of the range
        /// </param>
        /// <returns>
        /// returns true if it is in range, false if it is not
        /// </returns>
        public bool InHSVRange(Hsv srcHsv, Hsv[] targetHsv, int lowerLimitExtra, int higherLimitExtra)
        {
            if (targetHsv[(int)HSVRange.Low].Hue < targetHsv[(int)HSVRange.High].Hue)
            {
                return ((srcHsv.Hue >= (targetHsv[(int)HSVRange.Low].Hue - lowerLimitExtra)) &&             //lower values
                (srcHsv.Satuation >= (targetHsv[(int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                (srcHsv.Value >= (targetHsv[(int)HSVRange.Low].Value - lowerLimitExtra)) &&
                (srcHsv.Hue <= (targetHsv[(int)HSVRange.High].Hue + higherLimitExtra)) &&               //higher values
                (srcHsv.Satuation <= (targetHsv[(int)HSVRange.High].Satuation + higherLimitExtra)) &&
                (srcHsv.Value <= (targetHsv[(int)HSVRange.High].Value + higherLimitExtra)));
            }
            else 
            {
                if (srcHsv.Hue >= (targetHsv[(int)HSVRange.Low].Hue - lowerLimitExtra))
                {
                    return ((srcHsv.Hue >= (targetHsv[(int)HSVRange.Low].Hue - lowerLimitExtra)) &&             //lower values
                    (srcHsv.Satuation >= (targetHsv[(int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                    (srcHsv.Value >= (targetHsv[(int)HSVRange.Low].Value - lowerLimitExtra)) &&
                    (srcHsv.Hue <= (179)) &&               //higher values
                    (srcHsv.Satuation <= (targetHsv[(int)HSVRange.High].Satuation + higherLimitExtra)) &&
                    (srcHsv.Value <= (targetHsv[(int)HSVRange.High].Value + higherLimitExtra)));
                }
                else if (srcHsv.Hue <= (targetHsv[(int)HSVRange.High].Hue + higherLimitExtra))
                {
                    return ((srcHsv.Hue >= (0)) &&             //lower values
                    (srcHsv.Satuation >= (targetHsv[(int)HSVRange.Low].Satuation - lowerLimitExtra)) &&
                    (srcHsv.Value >= (targetHsv[(int)HSVRange.Low].Value - lowerLimitExtra)) &&
                    (srcHsv.Hue <= (targetHsv[(int)HSVRange.High].Hue + higherLimitExtra)) &&               //higher values
                    (srcHsv.Satuation <= (targetHsv[(int)HSVRange.High].Satuation + higherLimitExtra)) &&
                    (srcHsv.Value <= (targetHsv[(int)HSVRange.High].Value + higherLimitExtra)));
                }
                else return false;
            }
        }

        /// <summary>
        /// This function allows you to remove everything(change to black) in a image apart from the color you want(change to white)
        /// </summary>
        /// <param name="src">
        /// Image that you want to remove every colour apart from yours
        /// </param>
        /// <param name="targetHsv">
        /// THe color range you don't want to be removed, need specify high and low values, uses HSV color range
        /// </param>
        /// <returns>
        /// Returns the image with everything removed apart from what you wanted
        /// </returns>
        public Image<Bgr, Byte> RemoveEverythingButRange(Image<Bgr, Byte> src, Hsv[] targetHsv)//, Hsv colorGood, Hsv colorBad)
        {
            Image<Hsv, Byte> temp = src.Convert<Hsv, Byte>();
            Image<Bgr, Byte> dst;
            Hsv currentPixcelHSV;

            Hsv colorGood = new Hsv();//color we use to replace the correct data
            colorGood.Hue = 0;
            colorGood.Value = 255;
            colorGood.Satuation = 0;

            Hsv colorBad = new Hsv();//color we use to replace the not wanted colors
            colorBad.Hue = 0;
            colorBad.Value = 0;
            colorBad.Satuation = 0;

            for (int i = 0; i < src.Cols; i++)
            {
                for (int j = 0; j < src.Rows; j++)
                {
                    currentPixcelHSV = temp[j, i];
                    if (InHSVRange(currentPixcelHSV, targetHsv, 10, 20) == true)
                    {//make white if we are in range
                        temp[j, i] = colorGood;
                    }
                    else
                    {//make black otherwise
                        temp[j, i] = colorBad;
                    }
                }
            }
            dst = temp.Convert<Bgr, Byte>();//covert back to BGR
            return dst;
        }

        /// <summary>
        /// crops image to wanted size
        /// </summary>
        /// <param name="src">
        /// image that will be croped
        /// </param>
        /// <param name="x">
        /// The starting X location
        /// </param>
        /// <param name="y">
        /// Starting Y location
        /// </param>
        /// <param name="width">
        /// How wide we want to crop from origin that we set
        /// </param>
        /// <param name="height">
        /// How high we want to crop from origin that we set
        /// </param>
        /// <returns>
        /// returns croped image
        /// </returns>
        public Image<Bgr, Byte> CropImage(Image<Bgr, Byte> src, int x, int y, int width, int height)
        {
            Rectangle rect = new Rectangle();

            rect.X = x;
            rect.Y = y;
            rect.Width = width;
            rect.Height = height;
            if (x < 0 )
            {
                rect.X = 0; 
            }
            else if (x > src.Cols)
            {
                rect.X = src.Cols;
            }

            if (y < 0)
            {
                rect.Y = 0;
            }
            else if (y > src.Rows)
            {
                rect.Y = src.Rows;
            }

            if ((rect.X + width) > src.Cols)
            {
                rect.Width = src.Cols - rect.X;
            }

            if ((rect.Y + height) > src.Rows)
            {
                rect.Height = src.Rows - rect.Y;
            }
            
                

            return src.GetSubRect(rect);
        }

        /// <summary>
        /// workout where the peaks in
        /// </summary>
        /// <param name="srcHSV"></param>
        /// <param name="limit"></param>
        /// <param name="hsvPart"></param>
        /// <returns></returns>
        public int[][] getHighLowHSV(float[][] srcHSV, int limit, HSVdata hsvPart)//int hsvPart)
        {
            var HsvList = new List<int[]>();

            int[] hsvLH = new int[2];

            int toggle = 0;

            for (int j = 0; j < 256; j++)
            {
                if (srcHSV[(int)hsvPart][j] > limit)
                {
                    if (toggle == 0)
                    {
                        hsvLH[(int)HSVRange.Low] = j;
                        toggle = 1;

                    }

                }
                else if (srcHSV[(int)hsvPart][j] < limit)
                {
                    if (toggle == 1)
                    {
                        hsvLH[(int)HSVRange.High] = j;
                        HsvList.Add(hsvLH.Clone() as int[]);
                        toggle = 0;
                    }
                }
                if ((j == 255) && (toggle == 1))
                {

                    hsvLH[(int)HSVRange.High] = 255;
                    HsvList.Add(hsvLH.Clone() as int[]);
                    toggle = 0;

                }
            }
            //}
            return HsvList.ToArray();
        }

        /// <summary>
        /// works out hsv histogram of image and give us back an arrays for Hue, saturation, and value
        /// </summary>
        /// <param name="src">
        /// source image we want to get the historgram of
        /// </param>
        /// <returns>
        /// returns 2d array for hue sat and val
        /// </returns>
        public float[][] HsvValueFloatArray(Image<Bgr, Byte> src)
        {
            var HsvList = new List<float[]>();

            float[] HueHist;
            float[] SatHist;
            float[] ValHist;

            HueHist = new float[256];
            SatHist = new float[256];
            ValHist = new float[256];

            DenseHistogram HistoHue = new DenseHistogram(256, new RangeF(0, 256));
            DenseHistogram HistoSat = new DenseHistogram(256, new RangeF(0, 256));
            DenseHistogram HistoVal = new DenseHistogram(256, new RangeF(0, 256));

            Image<Hsv, Byte> hsvColor = src.Convert<Hsv, Byte>();
            Image<Gray, Byte> Comparedimg2Hsv = hsvColor[0];
            Image<Gray, Byte> Comparedimg2Sat = hsvColor[1];
            Image<Gray, Byte> Comparedimg2Val = hsvColor[2];

            HistoHue.Calculate(new Image<Gray, Byte>[] { Comparedimg2Hsv }, true, null);
            HistoSat.Calculate(new Image<Gray, Byte>[] { Comparedimg2Sat }, true, null);
            HistoVal.Calculate(new Image<Gray, Byte>[] { Comparedimg2Val }, true, null);

            //HistoVal.Calculate(

            HistoHue.MatND.ManagedArray.CopyTo(HueHist, 0);
            HistoSat.MatND.ManagedArray.CopyTo(SatHist, 0);
            HistoVal.MatND.ManagedArray.CopyTo(ValHist, 0);

            HsvList.Add(HueHist);
            HsvList.Add(SatHist);
            HsvList.Add(ValHist);

            return HsvList.ToArray();
        }

        public Image<Bgr, Byte> DrawPoints(Image<Bgr, Byte> src, PointF[] points)
        {
            foreach (PointF point in points)
            {//draw dots just for debuging atm
                Rectangle rect = new Rectangle();
                rect.X = (int)point.X;
                rect.Y = (int)point.Y;
                rect.Width = 2;
                rect.Height = 2;
                //if (a == 0)
                //{
                //    src.Draw(rect, new Bgr(Color.Blue), 1);
                //    a++;
                //}
                src.Draw(rect, new Bgr(Color.Red), 1);
                //a++;
            }
            return src;
        }

        public double Mag(PointF a, PointF b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X),2) + Math.Pow((a.Y - b.Y),2) );
        }
    }
}
