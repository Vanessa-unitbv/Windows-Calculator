using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Clasa care gestionează comutarea între diferitele moduri ale calculatorului
    /// </summary>
    public class CalculatorModeManager
    {
        // Referință la fereastra principală
        private readonly MainWindow _mainWindow;

        // Referințe la managerii specifici fiecărui mod
        private readonly CalculatorManager _standardManager;
        private readonly ProgrammerCalculatorManager _programmerManager;

        // Referință la managerul de memorie
        private readonly CalculatorMemoryManager _memoryManager;

        // Referință la grid-urile pentru fiecare mod
        private readonly Grid _standardCalculatorGrid;
        private readonly Grid _programmerCalculatorGrid;

        // Referințe la elementele de meniu pentru moduri
        private readonly MenuItem _standardModeMenuItem;
        private readonly MenuItem _programmerModeMenuItem;

        // Modul curent al calculatorului
        public enum CalculatorMode
        {
            Standard,
            Programmer
        }

        // Modul curent selectat
        private CalculatorMode _currentMode = CalculatorMode.Standard;

        /// <summary>
        /// Constructor pentru managerul de moduri
        /// </summary>
        /// <param name="mainWindow">Referință la fereastra principală</param>
        /// <param name="standardManager">Referință la managerul pentru modul standard</param>
        /// <param name="programmerManager">Referință la managerul pentru modul programator</param>
        /// <param name="memoryManager">Referință la managerul de memorie</param>
        public CalculatorModeManager(MainWindow mainWindow, CalculatorManager standardManager,
                                     ProgrammerCalculatorManager programmerManager, CalculatorMemoryManager memoryManager)
        {
            _mainWindow = mainWindow;
            _standardManager = standardManager;
            _programmerManager = programmerManager;
            _memoryManager = memoryManager;

            // Obține referințe la grid-uri
            _standardCalculatorGrid = _mainWindow.FindName("StandardCalculatorGrid") as Grid;
            _programmerCalculatorGrid = _mainWindow.FindName("ProgrammerCalculatorGrid") as Grid;

            // Obține referințe la elementele de meniu
            _standardModeMenuItem = _mainWindow.FindName("StandardModeMenuItem") as MenuItem;
            _programmerModeMenuItem = _mainWindow.FindName("ProgrammerModeMenuItem") as MenuItem;

            // Atașează evenimentele pentru elementele de meniu
            AttachMenuEvents();

            // Setează modul implicit
            SetCalculatorMode(CalculatorMode.Standard);
        }

        /// <summary>
        /// Atașează evenimentele pentru elementele de meniu pentru moduri
        /// </summary>
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

        /// <summary>
        /// Setează modul calculatorului și ajustează interfața în consecință
        /// </summary>
        /// <param name="mode">Modul calculatorului de setat</param>
        public void SetCalculatorMode(CalculatorMode mode)
        {
            _currentMode = mode;

            // Actualizează starea checkbox-urilor din meniu
            if (_standardModeMenuItem != null)
                _standardModeMenuItem.IsChecked = (mode == CalculatorMode.Standard);

            if (_programmerModeMenuItem != null)
                _programmerModeMenuItem.IsChecked = (mode == CalculatorMode.Programmer);

            // Comută între grid-uri
            if (_standardCalculatorGrid != null)
                _standardCalculatorGrid.Visibility = (mode == CalculatorMode.Standard) ? Visibility.Visible : Visibility.Collapsed;

            if (_programmerCalculatorGrid != null)
                _programmerCalculatorGrid.Visibility = (mode == CalculatorMode.Programmer) ? Visibility.Visible : Visibility.Collapsed;

            // Resetează managerul corespunzător modului
            if (mode == CalculatorMode.Standard)
            {
                _standardManager.Reset();

                // Ascunde stiva de memorie (dacă este vizibilă)
                _memoryManager.HideMemoryStack();
            }
            else // Mode == CalculatorMode.Programmer
            {
                _programmerManager.Reset();

                // Ascunde stiva de memorie (dacă este vizibilă)
                _memoryManager.HideMemoryStack();
            }

            // Salvează modul curent în setări
            SettingsManager.Instance.LastCalculatorMode = mode.ToString();
        }

        /// <summary>
        /// Obține modul curent al calculatorului
        /// </summary>
        public CalculatorMode CurrentMode
        {
            get { return _currentMode; }
        }

        /// <summary>
        /// Încărcă setările salvate pentru modul calculatorului
        /// </summary>
        public void LoadSettings()
        {
            // Încarcă ultimul mod selectat
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
    }
}