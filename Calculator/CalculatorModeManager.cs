using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
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

            // Setează modul implicit din setări
            LoadSettings();
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

        // Modificare în CalculatorModeManager.cs - Metoda SetCalculatorMode

        /// <summary>
        /// Setează modul calculatorului și ajustează interfața în consecință
        /// </summary>
        /// <param name="mode">Modul calculatorului de setat</param>
        // În CalculatorModeManager.cs - Actualizarea metodei SetCalculatorMode

        /// <summary>
        /// Setează modul calculatorului și ajustează interfața în consecință
        /// </summary>
        /// <param name="mode">Modul calculatorului de setat</param>
        // În CalculatorModeManager.cs - Modificare în metoda SetCalculatorMode

        /// <summary>
        /// Setează modul calculatorului și ajustează interfața în consecință
        /// </summary>
        /// <param name="mode">Modul calculatorului de setat</param>
        public void SetCalculatorMode(CalculatorMode mode)
        {
            // Dacă modul este același, nu facem nimic
            if (_currentMode == mode)
                return;

            // Salvăm setarea curentă de digit grouping înainte de schimbare
            bool useDigitGrouping = SettingsManager.Instance.UseDigitGrouping;

            // Stocăm modul anterior pentru a putea face modificări specifice tranziției
            CalculatorMode previousMode = _currentMode;

            // Actualizăm modul curent
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

            // Resetăm afișajul când trecem de la un mod la altul
            // Doar dacă comutăm între moduri diferite
            if (previousMode != mode)
            {
                // Resetăm TextBox-ul principal când trecem între moduri
                if (_mainWindow.ResultTextBox != null)
                {
                    _mainWindow.ResultTextBox.Text = "0";
                }
            }

            // Verificăm dacă avem moduri valide înainte de a seta digit grouping
            if (mode == CalculatorMode.Standard && _standardManager != null)
            {
                // Inițializăm afișajul standard cu zero
                _standardManager.Reset();

                // Aplicăm setarea de digit grouping pentru modul standard
                _standardManager.SetDigitGrouping(useDigitGrouping);

                // Ascunde stiva de memorie (dacă este vizibilă)
                if (_memoryManager != null)
                    _memoryManager.HideMemoryStack();
            }
            else if (mode == CalculatorMode.Programmer && _programmerManager != null)
            {
                // Inițializăm afișajul programmer cu zero
                _programmerManager.Reset();

                // Aplicăm setarea de digit grouping pentru modul programmer
                _programmerManager.SetDigitGrouping(useDigitGrouping);

                // Ascunde stiva de memorie (dacă este vizibilă)
                if (_memoryManager != null)
                    _memoryManager.HideMemoryStack();
            }

            // Salvează modul curent în setări
            SettingsManager.Instance.LastCalculatorMode = mode.ToString();
        }

        public void ApplyDigitGroupingWithoutReset(bool useGrouping)
        {
            // Aplicăm setarea de digit grouping direct la ambele moduri
            _standardManager.SetDigitGrouping(useGrouping);
            _programmerManager.SetDigitGrouping(useGrouping);
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
        public void SetDigitGroupingForAllModes(bool useGrouping)
        {
            // Setăm gruparea pentru modul standard
            _standardManager.SetDigitGrouping(useGrouping);

            // Setăm gruparea pentru modul programmer
            _programmerManager.SetDigitGrouping(useGrouping);
        }
    }
}