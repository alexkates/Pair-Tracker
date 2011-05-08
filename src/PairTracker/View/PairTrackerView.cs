﻿using PairTracker.Model;
using System;
using System.Collections.Generic;

namespace PairTracker.View
{
    public interface PairTrackerView
    {
        event EventHandler<StartButtonClickedEventArgs> StartButton_Clicked;
        event EventHandler<EventArgs> StopButton_Clicked;
        event EventHandler<ControllerChangedEventArgs> Controller_Changed;

        void SetStartStopButtonsToStartedMode();
        void SetStartStopButtonsToStoppedMode();
        void LockNameEntry();
        void UnlockNameEntry();
        void DisplayController(Programmer name);
        void ResetController();
        void DisplayIntervals(IEnumerable<Interval> intervals);
        void StartIntervalTimeoutTimer(TimeSpan timeout);

        void DisplayStats(IDictionary<Programmer, int> stats);
    }
}