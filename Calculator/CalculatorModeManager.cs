using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public class CalculatorModeManager
    {
        private readonly MainWindow _mainWindow;
        private readonly CalculatorManager _standardManager;
        private readonly ProgrammerCalculatorManager _programmerManager;
        private readonly CalculatorMemoryManager _memoryManager;
        private readonly Grid _standardCalculatorGrid;
        private readonly Grid _programmerCalculatorGrid;
        private readonly MenuItem _standardModeMenuItem;
        private readonly MenuItem _programmerModeMenuItem;
        public enum CalculatorMode
        {
            Standard,
            Programmer
        }
        private CalculatorMode _currentMode = CalculatorMode.Standard;
        public CalculatorModeManager(MainWindow mainWindow, CalculatorManager standardManager,
                                     ProgrammerCalculatorManager programmerManager, CalculatorMemoryManager memoryManager)
        {
            _mainWindow = mainWindow;
            _standardManager = standardManager;
            _programmerManager = programmerManager;
            _memoryManager = memoryManager;

            _standardCalculatorGrid = _mainWindow.FindName("StandardCalculatorGrid") as Grid;
            _programmerCalculatorGrid = _mainWindow.FindName("ProgrammerCalculatorGrid") as Grid;

            _standardModeMenuItem = _mainWindow.FindName("StandardModeMenuItem") as MenuItem;
            _programmerModeMenuItem = _mainWindow.FindName("ProgrammerModeMenuItem") as MenuItem;

            AttachMenuEvents();
            LoadSettings();
        }
        private void AttachMenuEvents()
        {
            if (_standardModeMenuItem != null)
            {
                _standardModeMenuItem.Click += (s, e) => {
                    SetCalculatorMode(CalculatorMode.Standard);
                };
            }

            if (_programmerModeMenuItem != null)
            {
                _programmerModeMenuItem.Click += (s, e) => {
                    SetCalculatorMode(CalculatorMode.Programmer);
                };
            }
        }
        public void SetCalculatorMode(CalculatorMode mode)
        {
            if (_currentMode == mode)
                return;
            bool useDigitGrouping = SettingsManager.Instance.UseDigitGrouping;
            CalculatorMode previousMode = _currentMode;
            _currentMode = mode;
            if (_standardModeMenuItem != null)
                _standardModeMenuItem.IsChecked = (mode == CalculatorMode.Standard);

            if (_programmerModeMenuItem != null)
                _programmerModeMenuItem.IsChecked = (mode == CalculatorMode.Programmer);

            if (_standardCalculatorGrid != null)
                _standardCalculatorGrid.Visibility = (mode == CalculatorMode.Standard) ? Visibility.Visible : Visibility.Collapsed;

            if (_programmerCalculatorGrid != null)
                _programmerCalculatorGrid.Visibility = (mode == CalculatorMode.Programmer) ? Visibility.Visible : Visibility.Collapsed;

            if (previousMode != mode)
            {
                if (_mainWindow.ResultTextBox != null)
                {
                    _mainWindow.ResultTextBox.Text = "0";
                }
            }
            if (mode == CalculatorMode.Standard && _standardManager != null)
            {
                _standardManager.Reset();
                _standardManager.SetDigitGrouping(useDigitGrouping);
                if (_memoryManager != null)
                    _memoryManager.HideMemoryStack();
            }
            else if (mode == CalculatorMode.Programmer && _programmerManager != null)
            {
                _programmerManager.Reset();
                _programmerManager.SetDigitGrouping(useDigitGrouping);
                if (_memoryManager != null)
                    _memoryManager.HideMemoryStack();
            }
            SettingsManager.Instance.LastCalculatorMode = mode.ToString();
        }
        public void ApplyDigitGroupingWithoutReset(bool useGrouping)
        {
            _standardManager.SetDigitGrouping(useGrouping);
            _programmerManager.SetDigitGrouping(useGrouping);
        }
        public CalculatorMode CurrentMode
        {
            get { return _currentMode; }
        }
        public void LoadSettings()
        {
            string lastMode = SettingsManager.Instance.LastCalculatorMode;

            if (lastMode == CalculatorMode.Programmer.ToString())
            {
                SetCalculatorMode(CalculatorMode.Programmer);
            }
            else
            {
                SetCalculatorMode(CalculatorMode.Standard);
            }
        }
        public void SetDigitGroupingForAllModes(bool useGrouping)
        {
            _standardManager.SetDigitGrouping(useGrouping);
            _programmerManager.SetDigitGrouping(useGrouping);
        }
    }
}