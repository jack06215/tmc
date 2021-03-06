﻿namespace Tmc.Scada.App
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

            #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.loginAndLogoutButton = new System.Windows.Forms.ToolStripButton();
            this.currentUserLabel = new System.Windows.Forms.ToolStripLabel();
            this.eStopButton = new System.Windows.Forms.ToolStripButton();
            this.plantMimicTab = new System.Windows.Forms.TabPage();
            this.controlTab = new System.Windows.Forms.TabPage();
            this.environmentTab = new System.Windows.Forms.TabPage();
            this.ordersTab = new System.Windows.Forms.TabPage();
            this.OrderListView = new System.Windows.Forms.DataGridView();
            this.orderIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.blackDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.blueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.redDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.greenDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.whiteDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.numberOfProductsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Button_AddNewOrder = new System.Windows.Forms.Button();
            this.reportsTab = new System.Windows.Forms.TabPage();
            this.pnlAlarms = new System.Windows.Forms.FlowLayoutPanel();
            this.btnShowList = new System.Windows.Forms.Button();
            this.tbcContentsTabControl = new System.Windows.Forms.TabControl();
            this.tabAlarmList = new System.Windows.Forms.TabPage();
            this.tabPlantMimic = new System.Windows.Forms.TabPage();
            this.tabEnvironment = new System.Windows.Forms.TabPage();
            this.tabOrders = new System.Windows.Forms.TabPage();
            this.tabCalibration = new System.Windows.Forms.TabPage();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.createUserButton = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.alarmsControl = new Tmc.Scada.App.Alarms();
            this.controlPage1 = new Tmc.Scada.App.controlPage();
            this.plantMimic1 = new Tmc.Scada.App.UserControls.PlantMimic();
            this.environmentControl1 = new Tmc.Scada.App.environmentControl();
            this.orderControl = new Tmc.Scada.App.Order();
            this.calibrationControl1 = new Tmc.Scada.App.CalibrationControl();
            this.reportControl2 = new Tmc.Scada.App.ReportControl();
            this.debugOverrides = new Tmc.Scada.App.DebugOverrides();
            this.toolStrip1.SuspendLayout();
            this.ordersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrderListView)).BeginInit();
            this.tbcContentsTabControl.SuspendLayout();
            this.tabAlarmList.SuspendLayout();
            this.tabPlantMimic.SuspendLayout();
            this.tabEnvironment.SuspendLayout();
            this.tabOrders.SuspendLayout();
            this.tabCalibration.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginAndLogoutButton,
            this.currentUserLabel,
            this.eStopButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.Size = new System.Drawing.Size(696, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // loginAndLogoutButton
            // 
            this.loginAndLogoutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loginAndLogoutButton.Image = ((System.Drawing.Image)(resources.GetObject("loginAndLogoutButton.Image")));
            this.loginAndLogoutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loginAndLogoutButton.Name = "loginAndLogoutButton";
            this.loginAndLogoutButton.Size = new System.Drawing.Size(41, 22);
            this.loginAndLogoutButton.Text = "Login";
            this.loginAndLogoutButton.Click += new System.EventHandler(this.loginAndLogoutButton_Click);
            // 
            // currentUserLabel
            // 
            this.currentUserLabel.Name = "currentUserLabel";
            this.currentUserLabel.Size = new System.Drawing.Size(92, 22);
            this.currentUserLabel.Text = "No Current User";
            // 
            // eStopButton
            // 
            this.eStopButton.BackColor = System.Drawing.Color.Red;
            this.eStopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.eStopButton.ForeColor = System.Drawing.Color.White;
            this.eStopButton.Image = ((System.Drawing.Image)(resources.GetObject("eStopButton.Image")));
            this.eStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eStopButton.Name = "eStopButton";
            this.eStopButton.Size = new System.Drawing.Size(97, 22);
            this.eStopButton.Text = "Emergency Stop";
            this.eStopButton.Click += new System.EventHandler(this.eStopButton_Click);
            // 
            // plantMimicTab
            // 
            this.plantMimicTab.Location = new System.Drawing.Point(4, 22);
            this.plantMimicTab.Name = "plantMimicTab";
            this.plantMimicTab.Padding = new System.Windows.Forms.Padding(3);
            this.plantMimicTab.Size = new System.Drawing.Size(686, 352);
            this.plantMimicTab.TabIndex = 0;
            this.plantMimicTab.Text = "Plant Mimic";
            this.plantMimicTab.UseVisualStyleBackColor = true;
            // 
            // controlTab
            // 
            this.controlTab.Location = new System.Drawing.Point(4, 22);
            this.controlTab.Name = "controlTab";
            this.controlTab.Padding = new System.Windows.Forms.Padding(3);
            this.controlTab.Size = new System.Drawing.Size(686, 352);
            this.controlTab.TabIndex = 1;
            this.controlTab.Text = "Control";
            this.controlTab.UseVisualStyleBackColor = true;
            // 
            // environmentTab
            // 
            this.environmentTab.Location = new System.Drawing.Point(4, 22);
            this.environmentTab.Name = "environmentTab";
            this.environmentTab.Padding = new System.Windows.Forms.Padding(3);
            this.environmentTab.Size = new System.Drawing.Size(686, 352);
            this.environmentTab.TabIndex = 2;
            this.environmentTab.Text = "Environment";
            this.environmentTab.UseVisualStyleBackColor = true;
            // 
            // ordersTab
            // 
            this.ordersTab.Controls.Add(this.OrderListView);
            this.ordersTab.Controls.Add(this.Button_AddNewOrder);
            this.ordersTab.Location = new System.Drawing.Point(4, 22);
            this.ordersTab.Name = "ordersTab";
            this.ordersTab.Padding = new System.Windows.Forms.Padding(3);
            this.ordersTab.Size = new System.Drawing.Size(686, 352);
            this.ordersTab.TabIndex = 3;
            this.ordersTab.Text = "Orders";
            this.ordersTab.UseVisualStyleBackColor = true;
            // 
            // OrderListView
            // 
            this.OrderListView.AllowUserToAddRows = false;
            this.OrderListView.AllowUserToDeleteRows = false;
            this.OrderListView.AutoGenerateColumns = false;
            this.OrderListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OrderListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.orderIDDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.blackDataGridViewTextBoxColumn,
            this.blueDataGridViewTextBoxColumn,
            this.redDataGridViewTextBoxColumn,
            this.greenDataGridViewTextBoxColumn,
            this.whiteDataGridViewTextBoxColumn,
            this.startTimeDataGridViewTextBoxColumn,
            this.endTimeDataGridViewTextBoxColumn,
            this.numberOfProductsDataGridViewTextBoxColumn});
            this.OrderListView.Location = new System.Drawing.Point(26, 21);
            this.OrderListView.Name = "OrderListView";
            this.OrderListView.Size = new System.Drawing.Size(497, 225);
            this.OrderListView.TabIndex = 5;
            // 
            // orderIDDataGridViewTextBoxColumn
            // 
            this.orderIDDataGridViewTextBoxColumn.DataPropertyName = "OrderID";
            this.orderIDDataGridViewTextBoxColumn.HeaderText = "OrderID";
            this.orderIDDataGridViewTextBoxColumn.Name = "orderIDDataGridViewTextBoxColumn";
            this.orderIDDataGridViewTextBoxColumn.Width = 50;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // blackDataGridViewTextBoxColumn
            // 
            this.blackDataGridViewTextBoxColumn.DataPropertyName = "Black";
            this.blackDataGridViewTextBoxColumn.HeaderText = "Black";
            this.blackDataGridViewTextBoxColumn.Name = "blackDataGridViewTextBoxColumn";
            this.blackDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.blackDataGridViewTextBoxColumn.Visible = false;
            // 
            // blueDataGridViewTextBoxColumn
            // 
            this.blueDataGridViewTextBoxColumn.DataPropertyName = "Blue";
            this.blueDataGridViewTextBoxColumn.HeaderText = "Blue";
            this.blueDataGridViewTextBoxColumn.Name = "blueDataGridViewTextBoxColumn";
            this.blueDataGridViewTextBoxColumn.Visible = false;
            // 
            // redDataGridViewTextBoxColumn
            // 
            this.redDataGridViewTextBoxColumn.DataPropertyName = "Red";
            this.redDataGridViewTextBoxColumn.HeaderText = "Red";
            this.redDataGridViewTextBoxColumn.Name = "redDataGridViewTextBoxColumn";
            this.redDataGridViewTextBoxColumn.Visible = false;
            // 
            // greenDataGridViewTextBoxColumn
            // 
            this.greenDataGridViewTextBoxColumn.DataPropertyName = "Green";
            this.greenDataGridViewTextBoxColumn.HeaderText = "Green";
            this.greenDataGridViewTextBoxColumn.Name = "greenDataGridViewTextBoxColumn";
            this.greenDataGridViewTextBoxColumn.Visible = false;
            // 
            // whiteDataGridViewTextBoxColumn
            // 
            this.whiteDataGridViewTextBoxColumn.DataPropertyName = "White";
            this.whiteDataGridViewTextBoxColumn.HeaderText = "White";
            this.whiteDataGridViewTextBoxColumn.Name = "whiteDataGridViewTextBoxColumn";
            this.whiteDataGridViewTextBoxColumn.Visible = false;
            // 
            // startTimeDataGridViewTextBoxColumn
            // 
            this.startTimeDataGridViewTextBoxColumn.DataPropertyName = "StartTime";
            this.startTimeDataGridViewTextBoxColumn.HeaderText = "StartTime";
            this.startTimeDataGridViewTextBoxColumn.Name = "startTimeDataGridViewTextBoxColumn";
            // 
            // endTimeDataGridViewTextBoxColumn
            // 
            this.endTimeDataGridViewTextBoxColumn.DataPropertyName = "EndTime";
            this.endTimeDataGridViewTextBoxColumn.HeaderText = "EndTime";
            this.endTimeDataGridViewTextBoxColumn.Name = "endTimeDataGridViewTextBoxColumn";
            // 
            // numberOfProductsDataGridViewTextBoxColumn
            // 
            this.numberOfProductsDataGridViewTextBoxColumn.DataPropertyName = "NumberOfProducts";
            this.numberOfProductsDataGridViewTextBoxColumn.HeaderText = "NumberOfProducts";
            this.numberOfProductsDataGridViewTextBoxColumn.Name = "numberOfProductsDataGridViewTextBoxColumn";
            // 
            // Button_AddNewOrder
            // 
            this.Button_AddNewOrder.Location = new System.Drawing.Point(26, 278);
            this.Button_AddNewOrder.Name = "Button_AddNewOrder";
            this.Button_AddNewOrder.Size = new System.Drawing.Size(183, 57);
            this.Button_AddNewOrder.TabIndex = 4;
            this.Button_AddNewOrder.Text = "AddNewOrder";
            this.Button_AddNewOrder.UseVisualStyleBackColor = true;
            // 
            // reportsTab
            // 
            this.reportsTab.Location = new System.Drawing.Point(4, 22);
            this.reportsTab.Name = "reportsTab";
            this.reportsTab.Padding = new System.Windows.Forms.Padding(3);
            this.reportsTab.Size = new System.Drawing.Size(686, 352);
            this.reportsTab.TabIndex = 4;
            this.reportsTab.Text = "Reports";
            this.reportsTab.UseVisualStyleBackColor = true;
            // 
            // pnlAlarms
            // 
            this.pnlAlarms.Location = new System.Drawing.Point(78, 28);
            this.pnlAlarms.Name = "pnlAlarms";
            this.pnlAlarms.Size = new System.Drawing.Size(618, 30);
            this.pnlAlarms.TabIndex = 2;
            // 
            // btnShowList
            // 
            this.btnShowList.Location = new System.Drawing.Point(0, 28);
            this.btnShowList.Name = "btnShowList";
            this.btnShowList.Size = new System.Drawing.Size(72, 30);
            this.btnShowList.TabIndex = 3;
            this.btnShowList.Text = "Alarms List";
            this.btnShowList.UseVisualStyleBackColor = true;
            // 
            // tbcContentsTabControl
            // 
            this.tbcContentsTabControl.Controls.Add(this.tabAlarmList);
            this.tbcContentsTabControl.Controls.Add(this.tabPlantMimic);
            this.tbcContentsTabControl.Controls.Add(this.tabEnvironment);
            this.tbcContentsTabControl.Controls.Add(this.tabOrders);
            this.tbcContentsTabControl.Controls.Add(this.tabCalibration);
            this.tbcContentsTabControl.Controls.Add(this.tabReports);
            this.tbcContentsTabControl.Controls.Add(this.tabDebug);
            this.tbcContentsTabControl.ItemSize = new System.Drawing.Size(98, 25);
            this.tbcContentsTabControl.Location = new System.Drawing.Point(0, 63);
            this.tbcContentsTabControl.Name = "tbcContentsTabControl";
            this.tbcContentsTabControl.SelectedIndex = 0;
            this.tbcContentsTabControl.Size = new System.Drawing.Size(695, 387);
            this.tbcContentsTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tbcContentsTabControl.TabIndex = 4;
            this.tbcContentsTabControl.SelectedIndexChanged += new System.EventHandler(this.tbcContentsTabControl_SelectedIndexChanged);
            // 
            // tabAlarmList
            // 
            this.tabAlarmList.Controls.Add(this.alarmsControl);
            this.tabAlarmList.Location = new System.Drawing.Point(4, 29);
            this.tabAlarmList.Name = "tabAlarmList";
            this.tabAlarmList.Size = new System.Drawing.Size(687, 354);
            this.tabAlarmList.TabIndex = 2;
            this.tabAlarmList.Text = "Alarm List";
            this.tabAlarmList.UseVisualStyleBackColor = true;
            // 
            // tabPlantMimic
            // 
            this.tabPlantMimic.Controls.Add(this.controlPage1);
            this.tabPlantMimic.Controls.Add(this.plantMimic1);
            this.tabPlantMimic.Location = new System.Drawing.Point(4, 29);
            this.tabPlantMimic.Name = "tabPlantMimic";
            this.tabPlantMimic.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlantMimic.Size = new System.Drawing.Size(687, 354);
            this.tabPlantMimic.TabIndex = 0;
            this.tabPlantMimic.Text = "Plant Mimic";
            this.tabPlantMimic.UseVisualStyleBackColor = true;
            // 
            // tabEnvironment
            // 
            this.tabEnvironment.Controls.Add(this.environmentControl1);
            this.tabEnvironment.Location = new System.Drawing.Point(4, 29);
            this.tabEnvironment.Name = "tabEnvironment";
            this.tabEnvironment.Size = new System.Drawing.Size(687, 354);
            this.tabEnvironment.TabIndex = 3;
            this.tabEnvironment.Text = "Environment";
            this.tabEnvironment.UseVisualStyleBackColor = true;
            // 
            // tabOrders
            // 
            this.tabOrders.Controls.Add(this.orderControl);
            this.tabOrders.Location = new System.Drawing.Point(4, 29);
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Size = new System.Drawing.Size(687, 354);
            this.tabOrders.TabIndex = 4;
            this.tabOrders.Text = "Orders";
            this.tabOrders.UseVisualStyleBackColor = true;
            // 
            // tabCalibration
            // 
            this.tabCalibration.Controls.Add(this.calibrationControl1);
            this.tabCalibration.Location = new System.Drawing.Point(4, 29);
            this.tabCalibration.Name = "tabCalibration";
            this.tabCalibration.Padding = new System.Windows.Forms.Padding(3);
            this.tabCalibration.Size = new System.Drawing.Size(687, 354);
            this.tabCalibration.TabIndex = 6;
            this.tabCalibration.Text = "Camera Calibration";
            this.tabCalibration.UseVisualStyleBackColor = true;
            // 
            // tabReports
            // 
            this.tabReports.Controls.Add(this.reportControl2);
            this.tabReports.Location = new System.Drawing.Point(4, 29);
            this.tabReports.Name = "tabReports";
            this.tabReports.Size = new System.Drawing.Size(687, 354);
            this.tabReports.TabIndex = 5;
            this.tabReports.Text = "Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.debugOverrides);
            this.tabDebug.Location = new System.Drawing.Point(4, 29);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Size = new System.Drawing.Size(687, 354);
            this.tabDebug.TabIndex = 7;
            this.tabDebug.Text = "Debug Overrides";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // createUserButton
            // 
            this.createUserButton.Location = new System.Drawing.Point(0, 2);
            this.createUserButton.Name = "createUserButton";
            this.createUserButton.Size = new System.Drawing.Size(75, 23);
            this.createUserButton.TabIndex = 5;
            this.createUserButton.Text = "Create User";
            this.createUserButton.UseVisualStyleBackColor = true;
            this.createUserButton.Click += new System.EventHandler(this.createUserButton_Click);
            // 
            // alarmsControl
            // 
            this.alarmsControl.Location = new System.Drawing.Point(9, 4);
            this.alarmsControl.Name = "alarmsControl";
            this.alarmsControl.Size = new System.Drawing.Size(684, 352);
            this.alarmsControl.TabIndex = 0;
            // 
            // controlPage1
            // 
            this.controlPage1.Location = new System.Drawing.Point(377, 60);
            this.controlPage1.Name = "controlPage1";
            this.controlPage1.Size = new System.Drawing.Size(303, 175);
            this.controlPage1.TabIndex = 1;
            // 
            // plantMimic1
            // 
            this.plantMimic1.Location = new System.Drawing.Point(-153, -3);
            this.plantMimic1.Name = "plantMimic1";
            this.plantMimic1.Size = new System.Drawing.Size(543, 354);
            this.plantMimic1.TabIndex = 0;
            // 
            // environmentControl1
            // 
            this.environmentControl1.Location = new System.Drawing.Point(3, 3);
            this.environmentControl1.Name = "environmentControl1";
            this.environmentControl1.Size = new System.Drawing.Size(257, 190);
            this.environmentControl1.TabIndex = 0;
            // 
            // orderControl
            // 
            this.orderControl.Location = new System.Drawing.Point(21, 3);
            this.orderControl.Name = "orderControl";
            this.orderControl.Size = new System.Drawing.Size(639, 343);
            this.orderControl.TabIndex = 0;
            // 
            // calibrationControl1
            // 
            this.calibrationControl1.Location = new System.Drawing.Point(0, 0);
            this.calibrationControl1.Name = "calibrationControl1";
            this.calibrationControl1.Size = new System.Drawing.Size(687, 354);
            this.calibrationControl1.TabIndex = 0;
            // 
            // reportControl2
            // 
            this.reportControl2.Location = new System.Drawing.Point(8, 35);
            this.reportControl2.Name = "reportControl2";
            this.reportControl2.SelectedEndDate = new System.DateTime(2014, 6, 9, 11, 18, 28, 936);
            this.reportControl2.SelectedStartDate = new System.DateTime(2014, 6, 8, 11, 18, 28, 936);
            this.reportControl2.Size = new System.Drawing.Size(691, 267);
            this.reportControl2.TabIndex = 0;
            // 
            // debugOverrides
            // 
            this.debugOverrides.Location = new System.Drawing.Point(8, 13);
            this.debugOverrides.Name = "debugOverrides";
            this.debugOverrides.Size = new System.Drawing.Size(318, 222);
            this.debugOverrides.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 450);
            this.Controls.Add(this.createUserButton);
            this.Controls.Add(this.tbcContentsTabControl);
            this.Controls.Add(this.btnShowList);
            this.Controls.Add(this.pnlAlarms);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "TMC System";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ordersTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OrderListView)).EndInit();
            this.tbcContentsTabControl.ResumeLayout(false);
            this.tabAlarmList.ResumeLayout(false);
            this.tabPlantMimic.ResumeLayout(false);
            this.tabEnvironment.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.tabCalibration.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabDebug.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel currentUserLabel;
        private System.Windows.Forms.ToolStripButton loginAndLogoutButton;
        private System.Windows.Forms.ToolStripButton eStopButton;
        //private TablessControl tablessControlPanel;
        public System.Windows.Forms.TabPage plantMimicTab;
        public System.Windows.Forms.TabPage controlTab;
        private System.Windows.Forms.TabPage environmentTab;
        private System.Windows.Forms.TabPage ordersTab;
        private System.Windows.Forms.TabPage reportsTab;
        private System.Windows.Forms.DataGridView OrderListView;
        private System.Windows.Forms.Button Button_AddNewOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn orderIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn blackDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn blueDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn redDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn greenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn whiteDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn numberOfProductsDataGridViewTextBoxColumn;
        private System.Windows.Forms.FlowLayoutPanel pnlAlarms;
        private System.Windows.Forms.Button btnShowList;
        private System.Windows.Forms.TabControl tbcContentsTabControl;
        private System.Windows.Forms.TabPage tabAlarmList;
        private System.Windows.Forms.TabPage tabEnvironment;
        private System.Windows.Forms.TabPage tabOrders;
        private System.Windows.Forms.TabPage tabReports;
        private System.Windows.Forms.TabPage tabCalibration;
        private CalibrationControl calibrationControl1;
        private System.Windows.Forms.TabPage tabPlantMimic;
        private controlPage controlPage1;
        private UserControls.PlantMimic plantMimic1;
        private environmentControl environmentControl1;
        private ReportControl reportControl2;
        private System.Windows.Forms.Button createUserButton;
        private Order orderControl;
        private System.Windows.Forms.BindingSource bindingSource1;
        private Alarms alarms1;
        private System.Windows.Forms.TabPage tabDebug;
        private DebugOverrides debugOverrides;
        private Alarms alarmsControl;
    }
}

