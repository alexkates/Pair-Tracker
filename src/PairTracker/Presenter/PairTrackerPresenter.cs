﻿using PairTracker.Model;
using PairTracker.View;
using System.Collections.Generic;
using System;

namespace PairTracker.Presenter
{
    public class PairTrackerPresenter
    {
        public PairTrackerView view { get; private set; }
        Session model;
        SessionPercentageStatisticGenerator statGenerator;

        public PairTrackerPresenter(PairTrackerView view, Session model, SessionPercentageStatisticGenerator statGenerator)
        {
            this.view = view;
            this.model = model;
            this.statGenerator = statGenerator;

            view.StartButton_Clicked += new EventHandler<StartButtonClickedEventArgs>(StartSession);
            view.StopButton_Clicked += new EventHandler<EventArgs>(EndSession);
            view.Controller_Changed += new EventHandler<ControllerChangedEventArgs>(ChangeControllerHandler);
        }

        private void StartSession(object sender, StartButtonClickedEventArgs e) 
        {
            model.Start(e.Programmer1, e.Programmer2);
            view.LockNameEntry();
            view.SetStartStopButtonsToStartedMode();
            view.DisplayIntervals(model.Intervals);
        }

        private void EndSession(object sender, EventArgs e) {
            model.Stop();

            view.ResetController();
            view.UnlockNameEntry();
            view.SetStartStopButtonsToStoppedMode();
            view.DisplayIntervals(model.Intervals);

            DisplayStats();
        }

        private void DisplayStats()
        {
            view.DisplayStats(statGenerator.Generate(model));
        }

        private void ChangeControllerHandler(object sender, ControllerChangedEventArgs e)
        {
            model.SwitchController(e.Programmer);
            view.StartIntervalTimeoutTimer(model.IntervalTimeout);
            if (model.CurrentInterval.Programmer == Programmer.Neither)
                view.ResetController();
            else
                view.DisplayController(model.CurrentInterval.Programmer);
            view.DisplayIntervals(model.Intervals);
            DisplayStats();
        }
    }
}