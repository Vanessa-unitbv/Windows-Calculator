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

        // Valoarea curentă stocată ca număr întreg (pe 64 de biți pentru a gestiona numere mai mari)
        private long _currentValue = 0;

        // Flag pentru a ști dacă începem un număr nou
        private bool _isNewNumber = true;

        // Operația în așteptare
        private string _pendingOperation = "";

        // Valoarea din stânga operației
        private long _leftOperand = 0;

        // Butoanele pentru cifrele hexazecimale
        private readonly Button _buttonHexA;
        private readonly Button _buttonHexB;
        private readonly Button _buttonHexC;
        private readonly Button _buttonHexD;
        private readonly Button _buttonHexE;
        private readonly Button _buttonHexF;

        // Butoanele pentru operații
        private readonly Button _buttonAddP;
        private readonly Button _buttonSubtractP;
        private readonly Button _buttonMultiplyP;
        private readonly Button _buttonDivideP;
        private readonly Button _buttonEqualP;
        private readonly Button _buttonClearP;
        private readonly Button _buttonClearEntryP;
        private readonly Button _buttonBackspaceP;
        private readonly Button _buttonPercentP;

        // Butoanele pentru cifre
        private readonly Button _button0P;
        private readonly Button _button1P;
        private readonly Button _button2P;
        private readonly Button _button3P;
        private readonly Button _button4P;
        private readonly Button _button5P;
        private readonly Button _button6P;
        private readonly Button _button7P;
        private readonly Button _button8P;
        private readonly Button _button9P;

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
            _buttonHexA = _mainWindow.FindName("ButtonHexA") as Button;
            _buttonHexB = _mainWindow.FindName("ButtonHexB") as Button;
            _buttonHexC = _mainWindow.FindName("ButtonHexC") as Button;
            _buttonHexD = _mainWindow.FindName("ButtonHexD") as Button;
            _buttonHexE = _mainWindow.FindName("ButtonHexE") as Button;
            _buttonHexF = _mainWindow.FindName("ButtonHexF") as Button;

            // Obținem referințe la butoanele pentru operații
            _buttonAddP = _mainWindow.FindName("ButtonAddP") as Button;
            _buttonSubtractP = _mainWindow.FindName("ButtonSubtractP") as Button;
            _buttonMultiplyP = _mainWindow.FindName("ButtonMultiplyP") as Button;
            _buttonDivideP = _mainWindow.FindName("ButtonDivideP") as Button;
            _buttonEqualP = _mainWindow.FindName("ButtonEqualP") as Button;
            _buttonClearP = _mainWindow.FindName("ButtonClearP") as Button;
            _buttonClearEntryP = _mainWindow.FindName("ButtonClearEntryP") as Button;
            _buttonBackspaceP = _mainWindow.FindName("ButtonBackspaceP") as Button;
            _buttonPercentP = _mainWindow.FindName("ButtonPercentP") as Button;

            // Obținem referințe la butoanele pentru cifre
            _button0P = _mainWindow.FindName("Button0P") as Button;
            _button1P = _mainWindow.FindName("Button1P") as Button;
            _button2P = _mainWindow.FindName("Button2P") as Button;
            _button3P = _mainWindow.FindName("Button3P") as Button;
            _button4P = _mainWindow.FindName("Button4P") as Button;
            _button5P = _mainWindow.FindName("Button5P") as Button;
            _button6P = _mainWindow.FindName("Button6P") as Button;
            _button7P = _mainWindow.FindName("Button7P") as Button;
            _button8P = _mainWindow.FindName("Button8P") as Button;
            _button9P = _mainWindow.FindName("Button9P") as Button;

            // Obținem referințe la RadioButton-urile pentru sistemele numerice
            _hexRadioButton = _mainWindow.FindName("HexRadioButton") as RadioButton;
            _decRadioButton = _mainWindow.FindName("DecRadioButton") as RadioButton;
            _octRadioButton = _mainWindow.FindName("OctRadioButton") as RadioButton;
            _binRadioButton = _mainWindow.FindName("BinRadioButton") as RadioButton;

            // Atașăm evenimentele pentru RadioButton-uri
            AttachNumberSystemEvents();

            // Atașăm evenimentele pentru butoane
            AttachButtonEvents();

            // Inițializăm afișajul valorilor cu 0
            UpdateAllDisplays(0);

            // Actualizează starea butoanelor conform sistemului numeric curent
            UpdateButtonsState();
        }

        /// <summary>
        /// Atașează evenimentele pentru RadioButton-urile bazei numerice
        /// </summary>
        private void AttachNumberSystemEvents()
        {
            if (_hexRadioButton != null) _hexRadioButton.Checked += NumberSystem_Changed;
            if (_decRadioButton != null) _decRadioButton.Checked += NumberSystem_Changed;
            if (_octRadioButton != null) _octRadioButton.Checked += NumberSystem_Changed;
            if (_binRadioButton != null) _binRadioButton.Checked += NumberSystem_Changed;
        }

        /// <summary>
        /// Atașează evenimentele pentru butoanele calculatorului
        /// </summary>
        private void AttachButtonEvents()
        {
            // Atașăm evenimentele pentru cifrele hexazecimale
            if (_buttonHexA != null) _buttonHexA.Click += HexDigit_Click;
            if (_buttonHexB != null) _buttonHexB.Click += HexDigit_Click;
            if (_buttonHexC != null) _buttonHexC.Click += HexDigit_Click;
            if (_buttonHexD != null) _buttonHexD.Click += HexDigit_Click;
            if (_buttonHexE != null) _buttonHexE.Click += HexDigit_Click;
            if (_buttonHexF != null) _buttonHexF.Click += HexDigit_Click;

            // Atașăm evenimentele pentru cifre
            if (_button0P != null) _button0P.Click += Digit_Click;
            if (_button1P != null) _button1P.Click += Digit_Click;
            if (_button2P != null) _button2P.Click += Digit_Click;
            if (_button3P != null) _button3P.Click += Digit_Click;
            if (_button4P != null) _button4P.Click += Digit_Click;
            if (_button5P != null) _button5P.Click += Digit_Click;
            if (_button6P != null) _button6P.Click += Digit_Click;
            if (_button7P != null) _button7P.Click += Digit_Click;
            if (_button8P != null) _button8P.Click += Digit_Click;
            if (_button9P != null) _button9P.Click += Digit_Click;

            // Atașăm evenimentele pentru operații
            if (_buttonAddP != null) _buttonAddP.Click += Operator_Click;
            if (_buttonSubtractP != null) _buttonSubtractP.Click += Operator_Click;
            if (_buttonMultiplyP != null) _buttonMultiplyP.Click += Operator_Click;
            if (_buttonDivideP != null) _buttonDivideP.Click += Operator_Click;
            if (_buttonPercentP != null) _buttonPercentP.Click += Operator_Click;

            // Atașăm evenimentele pentru butoanele speciale
            if (_buttonEqualP != null) _buttonEqualP.Click += Equal_Click;
            if (_buttonClearP != null) _buttonClearP.Click += Clear_Click;
            if (_buttonClearEntryP != null) _buttonClearEntryP.Click += ClearEntry_Click;
            if (_buttonBackspaceP != null) _buttonBackspaceP.Click += Backspace_Click;
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
        /// Gestionează evenimentul de click pe o cifră hexazecimală (A-F)
        /// </summary>
        private void HexDigit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string digit = button.Content.ToString();

                // Convertim cifra hexazecimală în valoare numerică
                int digitValue = 0;
                switch (digit)
                {
                    case "A": digitValue = 10; break;
                    case "B": digitValue = 11; break;
                    case "C": digitValue = 12; break;
                    case "D": digitValue = 13; break;
                    case "E": digitValue = 14; break;
                    case "F": digitValue = 15; break;
                }

                // Actualizăm valoarea curentă
                AppendDigit(digitValue);
            }
        }

        /// <summary>
        /// Gestionează evenimentul de click pe o cifră (0-9)
        /// </summary>
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string digit = button.Content.ToString();
                int digitValue = int.Parse(digit);

                // Actualizăm valoarea curentă
                AppendDigit(digitValue);
            }
        }

        /// <summary>
        /// Adaugă o cifră la valoarea curentă
        /// </summary>
        /// <param name="digit">Cifra de adăugat</param>
        private void AppendDigit(int digit)
        {
            // Verificăm dacă cifra este validă pentru sistemul numeric curent
            if (!IsValidDigit(digit))
                return;

            if (_isNewNumber)
            {
                _currentValue = digit;
                _isNewNumber = false;
            }
            else
            {
                // Calculăm baza curentă
                int baseValue = GetBaseForNumberSystem(_currentNumberSystem);

                // Adăugăm cifra la final
                _currentValue = _currentValue * baseValue + digit;
            }

            UpdateAllDisplays(_currentValue);
        }

        /// <summary>
        /// Verifică dacă o cifră este validă pentru sistemul numeric curent
        /// </summary>
        /// <param name="digit">Cifra de verificat</param>
        /// <returns>True dacă cifra este validă, False altfel</returns>
        private bool IsValidDigit(int digit)
        {
            switch (_currentNumberSystem)
            {
                case NumberSystem.BIN:
                    return digit <= 1;
                case NumberSystem.OCT:
                    return digit <= 7;
                case NumberSystem.DEC:
                    return digit <= 9;
                case NumberSystem.HEX:
                    return digit <= 15;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Obține baza numerică pentru sistemul numeric curent
        /// </summary>
        /// <param name="system">Sistemul numeric</param>
        /// <returns>Baza numerică (2, 8, 10 sau 16)</returns>
        private int GetBaseForNumberSystem(NumberSystem system)
        {
            switch (system)
            {
                case NumberSystem.BIN: return 2;
                case NumberSystem.OCT: return 8;
                case NumberSystem.DEC: return 10;
                case NumberSystem.HEX: return 16;
                default: return 10;
            }
        }

        /// <summary>
        /// Gestionează evenimentul de click pe un operator (+, -, etc.)
        /// </summary>
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string operation = button.Content.ToString();

                // Dacă avem o operație în așteptare, efectuăm calculul mai întâi
                if (!string.IsNullOrEmpty(_pendingOperation) && !_isNewNumber)
                {
                    PerformCalculation();
                }

                // Salvăm operandul din stânga și operația
                _leftOperand = _currentValue;
                _pendingOperation = operation;
                _isNewNumber = true;
            }
        }

        /// <summary>
        /// Gestionează evenimentul de click pe butonul egal (=)
        /// </summary>
        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            PerformCalculation();
        }

        /// <summary>
        /// Gestionează evenimentul de click pe butonul Clear (C)
        /// </summary>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _currentValue = 0;
            _leftOperand = 0;
            _pendingOperation = "";
            _isNewNumber = true;

            UpdateAllDisplays(0);
        }

        /// <summary>
        /// Gestionează evenimentul de click pe butonul Clear Entry (CE)
        /// </summary>
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            _currentValue = 0;
            _isNewNumber = true;

            UpdateAllDisplays(0);
        }

        /// <summary>
        /// Gestionează evenimentul de click pe butonul Backspace (⌫)
        /// </summary>
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewNumber)
                return;

            // Obține baza curentă
            int baseValue = GetBaseForNumberSystem(_currentNumberSystem);

            // Elimină ultima cifră
            _currentValue = _currentValue / baseValue;

            UpdateAllDisplays(_currentValue);
        }

        /// <summary>
        /// Efectuează calculul cu operația în așteptare
        /// </summary>
        private void PerformCalculation()
        {
            if (string.IsNullOrEmpty(_pendingOperation))
                return;

            long rightOperand = _currentValue;

            try
            {
                switch (_pendingOperation)
                {
                    case "+":
                        _currentValue = _leftOperand + rightOperand;
                        break;
                    case "-":
                        _currentValue = _leftOperand - rightOperand;
                        break;
                    case "*":
                    case "×":
                        _currentValue = _leftOperand * rightOperand;
                        break;
                    case "÷":
                        if (rightOperand == 0)
                        {
                            MessageBox.Show("Împărțirea la zero nu este permisă!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        _currentValue = _leftOperand / rightOperand;
                        break;
                    case "%":
                        _currentValue = _leftOperand % rightOperand;
                        break;
                }

                // Actualizăm afișajele cu rezultatul nou
                UpdateAllDisplays(_currentValue);

                // Resetăm pentru o nouă operație
                _pendingOperation = "";
                _isNewNumber = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la calculare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                Reset(); // Resetăm calculatorul în caz de eroare
            }
        }

        /// <summary>
        /// Actualizează starea butoanelor în funcție de sistemul numeric selectat
        /// </summary>
        public void UpdateButtonsState()
        {
            // Activăm toate butoanele cifre și hexazecimale
            SetButtonEnabled(_buttonHexA, true);
            SetButtonEnabled(_buttonHexB, true);
            SetButtonEnabled(_buttonHexC, true);
            SetButtonEnabled(_buttonHexD, true);
            SetButtonEnabled(_buttonHexE, true);
            SetButtonEnabled(_buttonHexF, true);
            SetButtonEnabled(_button0P, true);
            SetButtonEnabled(_button1P, true);
            SetButtonEnabled(_button2P, true);
            SetButtonEnabled(_button3P, true);
            SetButtonEnabled(_button4P, true);
            SetButtonEnabled(_button5P, true);
            SetButtonEnabled(_button6P, true);
            SetButtonEnabled(_button7P, true);
            SetButtonEnabled(_button8P, true);
            SetButtonEnabled(_button9P, true);

            // Dezactivăm butoanele corespunzătoare în funcție de sistemul numeric
            switch (_currentNumberSystem)
            {
                case NumberSystem.BIN:
                    SetButtonEnabled(_buttonHexA, false);
                    SetButtonEnabled(_buttonHexB, false);
                    SetButtonEnabled(_buttonHexC, false);
                    SetButtonEnabled(_buttonHexD, false);
                    SetButtonEnabled(_buttonHexE, false);
                    SetButtonEnabled(_buttonHexF, false);
                    SetButtonEnabled(_button2P, false);
                    SetButtonEnabled(_button3P, false);
                    SetButtonEnabled(_button4P, false);
                    SetButtonEnabled(_button5P, false);
                    SetButtonEnabled(_button6P, false);
                    SetButtonEnabled(_button7P, false);
                    SetButtonEnabled(_button8P, false);
                    SetButtonEnabled(_button9P, false);
                    break;

                case NumberSystem.OCT:
                    SetButtonEnabled(_buttonHexA, false);
                    SetButtonEnabled(_buttonHexB, false);
                    SetButtonEnabled(_buttonHexC, false);
                    SetButtonEnabled(_buttonHexD, false);
                    SetButtonEnabled(_buttonHexE, false);
                    SetButtonEnabled(_buttonHexF, false);
                    SetButtonEnabled(_button8P, false);
                    SetButtonEnabled(_button9P, false);
                    break;

                case NumberSystem.DEC:
                    SetButtonEnabled(_buttonHexA, false);
                    SetButtonEnabled(_buttonHexB, false);
                    SetButtonEnabled(_buttonHexC, false);
                    SetButtonEnabled(_buttonHexD, false);
                    SetButtonEnabled(_buttonHexE, false);
                    SetButtonEnabled(_buttonHexF, false);
                    break;
            }
        }

        /// <summary>
        /// Helper pentru a seta starea de enabled a unui buton, verificând mai întâi dacă butonul există
        /// </summary>
        private void SetButtonEnabled(Button button, bool enabled)
        {
            if (button != null)
            {
                button.IsEnabled = enabled;
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
                    ResultTextBox.Text = value.ToString("X");
                    break;
                case NumberSystem.DEC:
                    ResultTextBox.Text = value.ToString();
                    break;
                case NumberSystem.OCT:
                    ResultTextBox.Text = Convert.ToString(value, 8);
                    break;
                case NumberSystem.BIN:
                    ResultTextBox.Text = Convert.ToString(value, 2);
                    break;
            }

            // Actualizăm toate TextBox-urile cu valorile în diferite baze numerice
            if (HexValueTextBox != null)
                HexValueTextBox.Text = value.ToString("X");

            if (DecValueTextBox != null)
                DecValueTextBox.Text = value.ToString();

            if (OctValueTextBox != null)
                OctValueTextBox.Text = Convert.ToString(value, 8);

            if (BinValueTextBox != null)
                BinValueTextBox.Text = Convert.ToString(value, 2);
        }

        /// <summary>
        /// Resetează calculatorul la starea inițială
        /// </summary>
        public void Reset()
        {
            _currentValue = 0;
            _leftOperand = 0;
            _pendingOperation = "";
            _isNewNumber = true;

            UpdateAllDisplays(0);

            // Setează sistemul hexazecimal ca implicit
            _currentNumberSystem = NumberSystem.HEX;

            if (_hexRadioButton != null)
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
                    if (_hexRadioButton != null)
                        _hexRadioButton.IsChecked = true;
                    break;
                case NumberSystem.DEC:
                    if (_decRadioButton != null)
                        _decRadioButton.IsChecked = true;
                    break;
                case NumberSystem.OCT:
                    if (_octRadioButton != null)
                        _octRadioButton.IsChecked = true;
                    break;
                case NumberSystem.BIN:
                    if (_binRadioButton != null)
                        _binRadioButton.IsChecked = true;
                    break;
            }

            // Actualizează afișajul și starea butoanelor
            UpdateAllDisplays(_currentValue);
            UpdateButtonsState();
        }

        /// <summary>
        /// Gestionează apăsările de taste de la tastatură pentru modul Programmer
        /// </summary>
        /// <param name="e">Datele evenimentului</param>
        public void HandleKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.D0:
                    if (IsValidDigit(0))
                        AppendDigit(0);
                    break;
                case Key.NumPad1:
                case Key.D1:
                    if (IsValidDigit(1))
                        AppendDigit(1);
                    break;
                case Key.NumPad2:
                case Key.D2:
                    if (IsValidDigit(2))
                        AppendDigit(2);
                    break;
                case Key.NumPad3:
                case Key.D3:
                    if (IsValidDigit(3))
                        AppendDigit(3);
                    break;
                case Key.NumPad4:
                case Key.D4:
                    if (IsValidDigit(4))
                        AppendDigit(4);
                    break;
                case Key.NumPad5:
                case Key.D5:
                    if (IsValidDigit(5))
                        AppendDigit(5);
                    break;
                case Key.NumPad6:
                case Key.D6:
                    if (IsValidDigit(6))
                        AppendDigit(6);
                    break;
                case Key.NumPad7:
                case Key.D7:
                    if (IsValidDigit(7))
                        AppendDigit(7);
                    break;
                case Key.NumPad8:
                case Key.D8:
                    if (IsValidDigit(8))
                        AppendDigit(8);
                    break;
                case Key.NumPad9:
                case Key.D9:
                    if (IsValidDigit(9))
                        AppendDigit(9);
                    break;
                case Key.A:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(10);
                    break;
                case Key.B:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(11);
                    break;
                case Key.C:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(12);
                    break;
                case Key.D:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(13);
                    break;
                case Key.E:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(14);
                    break;
                case Key.F:
                    if (_currentNumberSystem == NumberSystem.HEX)
                        AppendDigit(15);
                    break;
                case Key.OemPlus:
                case Key.Add:
                    _leftOperand = _currentValue;
                    _pendingOperation = "+";
                    _isNewNumber = true;
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    _leftOperand = _currentValue;
                    _pendingOperation = "-";
                    _isNewNumber = true;
                    break;
                case Key.Multiply:
                    _leftOperand = _currentValue;
                    _pendingOperation = "*";
                    _isNewNumber = true;
                    break;
                case Key.Divide:
                case Key.OemQuestion:
                    _leftOperand = _currentValue;
                    _pendingOperation = "÷";
                    _isNewNumber = true;
                    break;
                case Key.Enter:
                    PerformCalculation();
                    break;
                case Key.Escape:
                    Clear_Click(null, null);
                    break;
                case Key.Back:
                    Backspace_Click(null, null);
                    break;
            }

            // Marcăm evenimentul ca procesat pentru a preveni propagarea
            e.Handled = true;
        }
    }
}