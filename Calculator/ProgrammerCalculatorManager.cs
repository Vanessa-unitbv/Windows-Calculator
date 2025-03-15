using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Collections.Generic;

namespace Calculator
{
    /// <summary>
    /// Clasă care gestionează funcționalitățile calculatorului în modul Programmer
    /// </summary>
    public class ProgrammerCalculatorManager
    {
        private readonly MainWindow _mainWindow;
        
        // Referințe la controalele principale
        private TextBox ResultTextBox => _mainWindow.ResultTextBox;
        private TextBox HexValueTextBox => _mainWindow.HexValueTextBox;
        private TextBox DecValueTextBox => _mainWindow.DecValueTextBox;
        private TextBox OctValueTextBox => _mainWindow.OctValueTextBox;
        private TextBox BinValueTextBox => _mainWindow.BinValueTextBox;
        
        // Sistemul numeric curent selectat
        public enum NumberSystem { HEX, DEC, OCT, BIN }
        private NumberSystem _currentNumberSystem = NumberSystem.HEX;
        
        // Valoarea curentă stocată ca număr întreg (pe 32 de biți)
        private long _currentValue = 0;
        
        // Butoanele pentru cifrele hexazecimale
        private readonly Button _buttonA;
        private readonly Button _buttonB;
        private readonly Button _buttonC;
        private readonly Button _buttonD;
        private readonly Button _buttonE;
        private readonly Button _buttonF;
        
        // RadioButton-urile pentru sistemele numerice
        private readonly RadioButton _hexRadioButton;
        private readonly RadioButton _decRadioButton;
        private readonly RadioButton _octRadioButton;
        private readonly RadioButton _binRadioButton;

        /// <summary>
        /// Constructor pentru managerul calculatorului Programmer
        /// </summary>
        /// <param name="mainWindow">Referință la fereastra principală</param>
        public ProgrammerCalculatorManager(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            
            // Obținem referințe la butoanele hexazecimale
            _buttonA = _mainWindow.FindName("ButtonA") as Button;
            _buttonB = _mainWindow.FindName("ButtonB") as Button;
            _buttonC = _mainWindow.FindName("ButtonC") as Button;
            _buttonD = _mainWindow.FindName("ButtonD") as Button;
            _buttonE = _mainWindow.FindName("ButtonE") as Button;
            _buttonF = _mainWindow.FindName("ButtonF") as Button;
            
            // Obținem referințe la RadioButton-urile pentru sistemele numerice
            _hexRadioButton = _mainWindow.FindName("HexRadioButton") as RadioButton;
            _decRadioButton = _mainWindow.FindName("DecRadioButton") as RadioButton;
            _octRadioButton = _mainWindow.FindName("OctRadioButton") as RadioButton;
            _binRadioButton = _mainWindow.FindName("BinRadioButton") as RadioButton;
            
            // Atașăm evenimentele pentru RadioButton-uri
            AttachNumberSystemEvents();
            
            // Inițializăm afișajul valorilor cu 0
            UpdateAllDisplays(0);
        }
        
        /// <summary>
        /// Atașează evenimentele pentru RadioButton-urile bazei numerice
        /// </summary>
        private void AttachNumberSystemEvents()
        {
            _hexRadioButton.Checked += NumberSystem_Changed;
            _decRadioButton.Checked += NumberSystem_Changed;
            _octRadioButton.Checked += NumberSystem_Changed;
            _binRadioButton.Checked += NumberSystem_Changed;
        }
        
        /// <summary>
        /// Gestionează evenimentul de schimbare a sistemului numeric
        /// </summary>
        private void NumberSystem_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                // Obține sistemul numeric selectat
                string system = radioButton.Content.ToString();
                
                switch (system)
                {
                    case "HEX":
                        _currentNumberSystem = NumberSystem.HEX;
                        break;
                    case "DEC":
                        _currentNumberSystem = NumberSystem.DEC;
                        break;
                    case "OCT":
                        _currentNumberSystem = NumberSystem.OCT;
                        break;
                    case "BIN":
                        _currentNumberSystem = NumberSystem.BIN;
                        break;
                }
                
                // Actualizează afișajul
                UpdateAllDisplays(_currentValue);
                
                // Actualizează starea butoanelor
                UpdateButtonsState();
            }
        }
        
        /// <summary>
        /// Actualizează starea butoanelor în funcție de sistemul numeric selectat
        /// </summary>
        public void UpdateButtonsState()
        {
            // Resetăm starea tuturor butoanelor
            _buttonA.IsEnabled = true;
            _buttonB.IsEnabled = true;
            _buttonC.IsEnabled = true;
            _buttonD.IsEnabled = true;
            _buttonE.IsEnabled = true;
            _buttonF.IsEnabled = true;
            
            // Butoanele pentru cifre sunt gestionate în AttachButtonEvents din MainWindow
            
            // Dezactivăm butoanele corespunzătoare în funcție de sistemul numeric
            switch (_currentNumberSystem)
            {
                case NumberSystem.BIN:
                    _buttonA.IsEnabled = false;
                    _buttonB.IsEnabled = false;
                    _buttonC.IsEnabled = false;
                    _buttonD.IsEnabled = false;
                    _buttonE.IsEnabled = false;
                    _buttonF.IsEnabled = false;
                    // Aici ar trebui să dezactivăm și butoanele pentru 2-9
                    break;
                    
                case NumberSystem.OCT:
                    _buttonA.IsEnabled = false;
                    _buttonB.IsEnabled = false;
                    _buttonC.IsEnabled = false;
                    _buttonD.IsEnabled = false;
                    _buttonE.IsEnabled = false;
                    _buttonF.IsEnabled = false;
                    // Aici ar trebui să dezactivăm și butoanele pentru 8-9
                    break;
                    
                case NumberSystem.DEC:
                    _buttonA.IsEnabled = false;
                    _buttonB.IsEnabled = false;
                    _buttonC.IsEnabled = false;
                    _buttonD.IsEnabled = false;
                    _buttonE.IsEnabled = false;
                    _buttonF.IsEnabled = false;
                    break;
            }
        }
        
        /// <summary>
        /// Actualizează toate afișajele cu valoarea curentă
        /// </summary>
        /// <param name="value">Valoarea de afișat</param>
        public void UpdateAllDisplays(long value)
        {
            // Actualizăm valoarea curentă
            _currentValue = value;
            
            // Actualizăm TextBox-ul principal conform sistemului numeric curent
            switch (_currentNumberSystem)
            {
                case NumberSystem.HEX:
                    ResultTextBox.Text = $"0x{value:X}";
                    break;
                case NumberSystem.DEC:
                    ResultTextBox.Text = value.ToString();
                    break;
                case NumberSystem.OCT:
                    ResultTextBox.Text = $"0{Convert.ToString(value, 8)}";
                    break;
                case NumberSystem.BIN:
                    ResultTextBox.Text = $"0b{Convert.ToString(value, 2)}";
                    break;
            }
            
            // Actualizăm toate TextBox-urile cu valorile în diferite baze numerice
            if (HexValueTextBox != null)
                HexValueTextBox.Text = $"0x{value:X}";
                
            if (DecValueTextBox != null)
                DecValueTextBox.Text = value.ToString();
                
            if (OctValueTextBox != null)
                OctValueTextBox.Text = $"0{Convert.ToString(value, 8)}";
                
            if (BinValueTextBox != null)
                BinValueTextBox.Text = $"0b{Convert.ToString(value, 2)}";
        }
        
        /// <summary>
        /// Resetează calculatorul la starea inițială
        /// </summary>
        public void Reset()
        {
            _currentValue = 0;
            UpdateAllDisplays(0);
            
            // Setează sistemul hexazecimal ca implicit
            _currentNumberSystem = NumberSystem.HEX;
            _hexRadioButton.IsChecked = true;
            
            // Actualizează starea butoanelor
            UpdateButtonsState();
        }
        
        /// <summary>
        /// Schimbă sistemul numeric curent
        /// </summary>
        /// <param name="system">Sistemul numeric de selectat</param>
        public void SetNumberSystem(NumberSystem system)
        {
            _currentNumberSystem = system;
            
            // Selectează RadioButton-ul corespunzător
            switch (system)
            {
                case NumberSystem.HEX:
                    _hexRadioButton.IsChecked = true;
                    break;
                case NumberSystem.DEC:
                    _decRadioButton.IsChecked = true;
                    break;
                case NumberSystem.OCT:
                    _octRadioButton.IsChecked = true;
                    break;
                case NumberSystem.BIN:
                    _binRadioButton.IsChecked = true;
                    break;
            }
            
            // Actualizează afișajul și starea butoanelor
            UpdateAllDisplays(_currentValue);
            UpdateButtonsState();
        }
        
        // Aici poți adăuga metodele pentru implementarea funcționalităților butoanelor
        // (operații logice, aritmetice, etc.) când vei fi gata să implementezi logica completă
    }
}