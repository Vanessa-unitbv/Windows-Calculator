using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Collections.Generic;

namespace Calculator
{
    public class ProgrammerCalculatorManager
    {
        private readonly MainWindow _mainWindow;
        private TextBox ResultTextBox => _mainWindow.ResultTextBox;
        private TextBox HexValueTextBox => _mainWindow.HexValueTextBox;
        private TextBox DecValueTextBox => _mainWindow.DecValueTextBox;
        private TextBox OctValueTextBox => _mainWindow.OctValueTextBox;
        private TextBox BinValueTextBox => _mainWindow.BinValueTextBox;
        public enum NumberSystem { HEX, DEC, OCT, BIN }
        private NumberSystem _currentNumberSystem = NumberSystem.HEX;
        private long _currentValue = 0;
        private bool _isNewNumber = true;
        private string _pendingOperation = "";
        private long _leftOperand = 0;
        private bool _useDigitGrouping = false;

        private readonly Button _buttonHexA;
        private readonly Button _buttonHexB;
        private readonly Button _buttonHexC;
        private readonly Button _buttonHexD;
        private readonly Button _buttonHexE;
        private readonly Button _buttonHexF;

        private readonly Button _buttonAddP;
        private readonly Button _buttonSubtractP;
        private readonly Button _buttonMultiplyP;
        private readonly Button _buttonDivideP;
        private readonly Button _buttonEqualP;
        private readonly Button _buttonClearP;
        private readonly Button _buttonClearEntryP;
        private readonly Button _buttonBackspaceP;
        private readonly Button _buttonPercentP;

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

        private readonly RadioButton _hexRadioButton;
        private readonly RadioButton _decRadioButton;
        private readonly RadioButton _octRadioButton;
        private readonly RadioButton _binRadioButton;

        public ProgrammerCalculatorManager(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _buttonHexA = _mainWindow.FindName("ButtonHexA") as Button;
            _buttonHexB = _mainWindow.FindName("ButtonHexB") as Button;
            _buttonHexC = _mainWindow.FindName("ButtonHexC") as Button;
            _buttonHexD = _mainWindow.FindName("ButtonHexD") as Button;
            _buttonHexE = _mainWindow.FindName("ButtonHexE") as Button;
            _buttonHexF = _mainWindow.FindName("ButtonHexF") as Button;

            _buttonAddP = _mainWindow.FindName("ButtonAddP") as Button;
            _buttonSubtractP = _mainWindow.FindName("ButtonSubtractP") as Button;
            _buttonMultiplyP = _mainWindow.FindName("ButtonMultiplyP") as Button;
            _buttonDivideP = _mainWindow.FindName("ButtonDivideP") as Button;
            _buttonEqualP = _mainWindow.FindName("ButtonEqualP") as Button;
            _buttonClearP = _mainWindow.FindName("ButtonClearP") as Button;
            _buttonClearEntryP = _mainWindow.FindName("ButtonClearEntryP") as Button;
            _buttonBackspaceP = _mainWindow.FindName("ButtonBackspaceP") as Button;
            _buttonPercentP = _mainWindow.FindName("ButtonPercentP") as Button;

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

            _hexRadioButton = _mainWindow.FindName("HexRadioButton") as RadioButton;
            _decRadioButton = _mainWindow.FindName("DecRadioButton") as RadioButton;
            _octRadioButton = _mainWindow.FindName("OctRadioButton") as RadioButton;
            _binRadioButton = _mainWindow.FindName("BinRadioButton") as RadioButton;

            AttachNumberSystemEvents();
            AttachButtonEvents();
            UpdateAllDisplays(0);
            LoadNumberSystemSettings();
            UpdateButtonsState();
        }
        public void SetDigitGrouping(bool useGrouping)
        {
            _useDigitGrouping = useGrouping;
            UpdateAllDisplays(_currentValue);
        }
        private void LoadNumberSystemSettings()
        {
            string lastNumberSystem = SettingsManager.Instance.LastNumberSystem;
            switch (lastNumberSystem)
            {
                case "HEX":
                    _currentNumberSystem = NumberSystem.HEX;
                    if (_hexRadioButton != null)
                        _hexRadioButton.IsChecked = true;
                    break;
                case "DEC":
                    _currentNumberSystem = NumberSystem.DEC;
                    if (_decRadioButton != null)
                        _decRadioButton.IsChecked = true;
                    break;
                case "OCT":
                    _currentNumberSystem = NumberSystem.OCT;
                    if (_octRadioButton != null)
                        _octRadioButton.IsChecked = true;
                    break;
                case "BIN":
                    _currentNumberSystem = NumberSystem.BIN;
                    if (_binRadioButton != null)
                        _binRadioButton.IsChecked = true;
                    break;
                default:
                    _currentNumberSystem = NumberSystem.HEX;
                    if (_hexRadioButton != null)
                        _hexRadioButton.IsChecked = true;
                    break;
            }
        }
        private void AttachNumberSystemEvents()
        {
            if (_hexRadioButton != null) _hexRadioButton.Checked += NumberSystem_Changed;
            if (_decRadioButton != null) _decRadioButton.Checked += NumberSystem_Changed;
            if (_octRadioButton != null) _octRadioButton.Checked += NumberSystem_Changed;
            if (_binRadioButton != null) _binRadioButton.Checked += NumberSystem_Changed;
        }

        private void AttachButtonEvents()
        {
            if (_buttonHexA != null) _buttonHexA.Click += HexDigit_Click;
            if (_buttonHexB != null) _buttonHexB.Click += HexDigit_Click;
            if (_buttonHexC != null) _buttonHexC.Click += HexDigit_Click;
            if (_buttonHexD != null) _buttonHexD.Click += HexDigit_Click;
            if (_buttonHexE != null) _buttonHexE.Click += HexDigit_Click;
            if (_buttonHexF != null) _buttonHexF.Click += HexDigit_Click;

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

            if (_buttonAddP != null) _buttonAddP.Click += Operator_Click;
            if (_buttonSubtractP != null) _buttonSubtractP.Click += Operator_Click;
            if (_buttonMultiplyP != null) _buttonMultiplyP.Click += Operator_Click;
            if (_buttonDivideP != null) _buttonDivideP.Click += Operator_Click;
            if (_buttonPercentP != null) _buttonPercentP.Click += Operator_Click;

            if (_buttonEqualP != null) _buttonEqualP.Click += Equal_Click;
            if (_buttonClearP != null) _buttonClearP.Click += Clear_Click;
            if (_buttonClearEntryP != null) _buttonClearEntryP.Click += ClearEntry_Click;
            if (_buttonBackspaceP != null) _buttonBackspaceP.Click += Backspace_Click;
        }
        private void NumberSystem_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
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
                SettingsManager.Instance.LastNumberSystem = system;
                UpdateAllDisplays(_currentValue);
                UpdateButtonsState();
            }
        }
        private void HexDigit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string digit = button.Content.ToString();
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
                AppendDigit(digitValue);
            }
        }
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string digit = button.Content.ToString();
                int digitValue = int.Parse(digit);
                AppendDigit(digitValue);
            }
        }
        private void AppendDigit(int digit)
        {
            if (!IsValidDigit(digit))
                return;

            if (_isNewNumber)
            {
                _currentValue = digit;
                _isNewNumber = false;
            }
            else
            {
                int baseValue = GetBaseForNumberSystem(_currentNumberSystem);
                _currentValue = _currentValue * baseValue + digit;
            }

            UpdateAllDisplays(_currentValue);
        }
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
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string operation = button.Content.ToString();
                if (!string.IsNullOrEmpty(_pendingOperation) && !_isNewNumber)
                {
                    PerformCalculation();
                }
                _leftOperand = _currentValue;
                _pendingOperation = operation;
                _isNewNumber = true;
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _currentValue = 0;
            _leftOperand = 0;
            _pendingOperation = "";
            _isNewNumber = true;

            UpdateAllDisplays(0);
        }
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            _currentValue = 0;
            _isNewNumber = true;

            UpdateAllDisplays(0);
        }
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewNumber)
                return;
            int baseValue = GetBaseForNumberSystem(_currentNumberSystem);
            _currentValue = _currentValue / baseValue;
            UpdateAllDisplays(_currentValue);
        }
        public void UpdateButtonsState()
        {
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
        private void SetButtonEnabled(Button button, bool enabled)
        {
            if (button != null)
            {
                button.IsEnabled = enabled;
            }
        }

        private string FormatWithGrouping(string input, int groupSize)
        {
            bool isNegative = input.StartsWith("-");
            if (isNegative)
            {
                input = input.Substring(1);
            }
            if (input.Length <= groupSize)
            {
                return isNegative ? "-" + input : input;
            }
            string separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (i > 0 && i % groupSize == 0)
                {
                    result.Insert(0, separator);
                }
                result.Insert(0, input[input.Length - 1 - i]);
            }
            if (isNegative)
            {
                result.Insert(0, "-");
            }
            return result.ToString();
        }
        public void UpdateAllDisplays(long value)
        {
            _currentValue = value;
            string hexString = value.ToString("X");
            string decString = value.ToString();
            string octString = Convert.ToString(value, 8);
            string binString = Convert.ToString(value, 2);
            if (_useDigitGrouping)
            {
                hexString = FormatWithGrouping(hexString, 4);
                decString = FormatWithGrouping(decString, 3);
                octString = FormatWithGrouping(octString, 3);
                binString = FormatWithGrouping(binString, 4);
            }
            switch (_currentNumberSystem)
            {
                case NumberSystem.HEX:
                    ResultTextBox.Text = hexString;
                    break;
                case NumberSystem.DEC:
                    ResultTextBox.Text = decString;
                    break;
                case NumberSystem.OCT:
                    ResultTextBox.Text = octString;
                    break;
                case NumberSystem.BIN:
                    ResultTextBox.Text = binString;
                    break;
            }
            if (HexValueTextBox != null)
                HexValueTextBox.Text = hexString;

            if (DecValueTextBox != null)
                DecValueTextBox.Text = decString;

            if (OctValueTextBox != null)
                OctValueTextBox.Text = octString;

            if (BinValueTextBox != null)
                BinValueTextBox.Text = binString;
        }
        public void Reset()
        {
            _currentValue = 0;
            _leftOperand = 0;
            _pendingOperation = "";
            _isNewNumber = true;
            UpdateAllDisplays(0);
            LoadNumberSystemSettings();
            UpdateButtonsState();
        }
        public void SetNumberSystem(NumberSystem system)
        {
            _currentNumberSystem = system;
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
            UpdateAllDisplays(_currentValue);
            UpdateButtonsState();
        }
        public void PerformCalculation()
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
                    case "x":
                        _currentValue = _leftOperand * rightOperand;
                        break;
                    case "/":
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
                UpdateAllDisplays(_currentValue);
                _leftOperand = _currentValue;
                _pendingOperation = "";
                _isNewNumber = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la calculare: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                Reset(); 
            }
        }
        public void HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.None)
                {
                    if (_currentNumberSystem == NumberSystem.HEX && !_isNewNumber)
                    {
                        AppendDigit(12);
                    }
                    else
                    {
                        Clear_Click(null, null);
                    }
                    e.Handled = true;
                    return;
                }
                if (e.Key == Key.Enter || (e.Key == Key.OemPlus && Keyboard.Modifiers == ModifierKeys.Shift))
                {
                    SimulateEqualClick();
                    e.Handled = true;
                    return;
                }
                switch (e.Key)
                {
                    case Key.Add:
                    case Key.OemPlus when Keyboard.Modifiers != ModifierKeys.Shift:
                        Operator_Click(new Button { Content = "+" }, new RoutedEventArgs());
                        break;
                    case Key.Subtract:
                    case Key.OemMinus:
                        Operator_Click(new Button { Content = "-" }, new RoutedEventArgs());
                        break;
                    case Key.Multiply:
                        Operator_Click(new Button { Content = "*" }, new RoutedEventArgs());
                        break;
                    case Key.Divide:
                    case Key.OemQuestion:
                        Operator_Click(new Button { Content = "÷" }, new RoutedEventArgs());
                        break;
                    case Key.D5 when Keyboard.Modifiers == ModifierKeys.Shift:
                    case Key.NumPad5 when Keyboard.Modifiers == ModifierKeys.Shift:
                        Operator_Click(new Button { Content = "%" }, new RoutedEventArgs());
                        break;
                    case Key.Escape:
                        Clear_Click(null, null);
                        break;
                    case Key.Back:
                        Backspace_Click(null, null);
                        break;
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
                            AppendDigit(11);
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
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la procesarea tastei: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SimulateEqualClick()
        {
            Equal_Click(null, new RoutedEventArgs());
        }
        private void Equal_Click(object sender, RoutedEventArgs e)
        { 
            PerformCalculation();
        }

    }
}