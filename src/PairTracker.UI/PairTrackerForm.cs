﻿using RawInput;
using PairTracker.View;
using PairTracker.Model;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace PairTracker.UI
{
    public partial class PairTrackerForm : Form, PairTrackerView
    {
        InputDevice inputDevice;
        InputDevice.DeviceEventHandler handler;
        bool tracking = false;

        public PairTrackerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inputDevice = new InputDevice(Handle);
            inputDevice.EnumerateDevices();

            handler = new InputDevice.DeviceEventHandler(inputDevice_KeyPressed);

            IntervalTimer = new Timer();
            IntervalTimer.Tick += new EventHandler(IntervalTimer_Tick);
        }

        void IntervalTimer_Tick(object sender, EventArgs e)
        {
            if (Controller_Changed != null)
                Controller_Changed(this, new ControllerChangedEventArgs(Programmer.Neither));
        }

        Timer IntervalTimer { get; set; }

        void inputDevice_KeyPressed(object sender, InputDevice.KeyControlEventArgs e)
        {
            //TODO: How can I clean this up?
            //TODO: Associate the keyboards from config or admin menu
            Programmer controllingProgrammer = Programmer.Neither;
            if (e.Keyboard.deviceHandle.ToString() == "65603")
                controllingProgrammer = programmer1;
            if (e.Keyboard.deviceHandle.ToString() == "720961")
                controllingProgrammer = programmer2;

            if (Controller_Changed != null)
                Controller_Changed(this, new ControllerChangedEventArgs(controllingProgrammer));
        }
        
        protected override void WndProc(ref Message message)
        {
            if (inputDevice != null && tracking)
                inputDevice.ProcessMessage(message);
            base.WndProc(ref message);
        }

        private Programmer programmer1;
        private Programmer programmer2;

        private void btnStart_Click(object sender, EventArgs e)
        {
            inputDevice.KeyPressed += handler;
            programmer1 = new Programmer(txtProgrammerOne.Text);
            programmer2 = new Programmer(txtProgrammerTwo.Text);
            if(StartButton_Clicked != null)
                StartButton_Clicked(this, new StartButtonClickedEventArgs(programmer1, programmer2));
 
            tracking = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            inputDevice.KeyPressed -= handler;
            if(StopButton_Clicked != null)
                StopButton_Clicked(this, new EventArgs());
            tracking = false;
        }

        public event EventHandler<StartButtonClickedEventArgs> StartButton_Clicked;
        public event EventHandler<EventArgs> StopButton_Clicked;
        public event EventHandler<ControllerChangedEventArgs> Controller_Changed;

        public void SetStartStopButtonsToStoppedMode()
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        public void SetStartStopButtonsToStartedMode()
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        public void LockNameEntry()
        {
            txtProgrammerOne.Enabled = false;
            txtProgrammerTwo.Enabled = false;
        }

        public void UnlockNameEntry()
        {
            txtProgrammerOne.Enabled = true;
            txtProgrammerTwo.Enabled = true;
        }

        public void DisplayController(Programmer programmer)
        {
            lblControllerName.Text = programmer.Name + " is currently typing.";
        }

        public void ResetController()
        {
            lblControllerName.Text = string.Empty;
        }

        public void DisplayIntervals(IEnumerable<Interval> intervals)
        {
            SessionDetails.Items.Clear();
            foreach (var interval in intervals)
                DisplayInterval(interval);
        }

        void DisplayInterval(Interval interval)
        {
            var item = SessionDetails.Items.Add(interval.Programmer.Name);
            item.SubItems.Add(interval.StartTime.TimeOfDay.ToString());
            item.SubItems.Add(interval.EndTime.TimeOfDay.ToString());
            item.SubItems.Add(interval.Length.ToString());
        }

        public void StartIntervalTimeoutTimer(TimeSpan timeout)
        {
            IntervalTimer.Interval = (int)timeout.TotalMilliseconds;
            IntervalTimer.Stop();
            IntervalTimer.Start();
        }

        public void DisplayStats(IDictionary<Programmer, int> stats)
        {
            Stats.Items.Clear();
            foreach (var stat in stats)
                DisplayStat(stat);
        }

        private void DisplayStat(KeyValuePair<Programmer, int> stat)
        {
            Stats.Items.Add(stat.Key.Name).SubItems.Add(stat.Value.ToString());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: Go through presenter, so he can clean up (verify before close if session is running)
            this.Close();
        }
    }
}