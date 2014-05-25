﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tmc.Sensors;
using System.Configuration;
using System.Timers;
using TmcData;

namespace Tmc.Scada.Core
{
    public class EnvironmentMonitor
    {
        public bool LoggingEnabled { get; set; }
        private List<ISensor> _sensors;
        public List<EnvironmentLogEntry> Log { get; set; }
        private Timer _updateTimer;

        // SensorProperties Entry will have the Dictionary as follows
        // <Channel, Tuple<Units, Maximum threshold, minimum threshold>>
        public Dictionary<String, Tuple<String, float, float>> SensorProperties { get; private set; }

        public EnvironmentMonitor(ClusterConfig config)
        {
            InitialiseSensorProperties();
            Log = new List<EnvironmentLogEntry>();
            _sensors = new List<ISensor>();
            _sensors.AddRange(config.Sensors.Values);
            this.LoggingEnabled = true;
            int updateTime = 1000;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["EnvironmentMonitorUpdateRateMsec"], out updateTime))
            {
                Logger.Instance.Write(new LogEntry("EnvironmentMonitorUpdateRateMsec is invalid, defaulting to 1000 msec",
                    LogType.Warning));
            }
            this._updateTimer = new Timer(updateTime);
            this._updateTimer.Elapsed += new ElapsedEventHandler((s, e) => this.Update());
        }

        private void InitialiseSensorProperties()
        {
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
            SensorProperties.Add("Temperature", new Tuple<string, float, float>("°C", 50, 10));
        }

        public void Start()
        {
            this._updateTimer.Start();
        }

        public void Stop()
        {
            this._updateTimer.Stop();
        }

        private void Update()
        {
            while (LoggingEnabled)
            {
                foreach (var sensor in _sensors)
                {
                    LogType logType;
                    if ((sensor.GetData() > SensorProperties[sensor.Channel].Item2) || (sensor.GetData() < SensorProperties[sensor.Channel].Item3)) 
                    {
                        logType = LogType.Warning;
                    }
                    else
                    {
                        logType = LogType.Message;
                    }
                    Log.Add(new EnvironmentLogEntry(sensor.Channel, sensor.GetData(), SensorProperties[sensor.Channel].Item1, logType));
                    // enter in the database once its merged with scada
                }
            }
        }
    }
}
