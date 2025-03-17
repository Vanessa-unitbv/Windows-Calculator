using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

namespace Calculator
{
    public class CalculatorManager
    {
        private readonly MainWindow _mainWindow;
        private readonly CalculatorEngine _engine;
        private readonly CalculatorMemoryManager _memoryManager;
        private TextBox ResultTextBox => _mainWindow.ResultTextBox;
        private bool _isNewNumber = true;
        private bool _isResultDisplayed = false;
        private bool _useDigitGrouping = false;
        private string _currentNumberString = "0";
        private bool _hasDecimal = false;
        private readonly CultureInfo _currentCulture;
        public CalculatorManager(MainWindow mainWindow, CalculatorMemoryManager memoryManager)
        {
            _mainWindow = mainWindow;
            _engine = new CalculatorEngine();
            _memoryManager = memoryManager;
            _currentCulture = CultureInfo.CurrentCulture;
            AttachEventHandlers();
        }
        private void AttachEventHandlers()
        {
            foreach (var child in LogicalTreeHelper.GetChildren(_mainWindow))
            {
                if (child is Grid grid)
                {
                    AttachButtonEvents(grid);
                }
            }
        }
        private void AttachButtonEvents(DependencyObject parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button button)
                {
                    switch (button.Content.ToString())
                    {
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            button.Click += Number_Click;
                            break;
                        case ",":
                            button.Click += Decimal_Click;
                            break;
                        case "+":
                        case "-":
                        case "x":
                        case "÷":
                        case "%":
                            button.Click += Operator_Click;
                            break;
                        case "=":
                            button.Click += Equal_Click;
                            break;
                        case "C":
                            button.Click += Clear_Click;
                            break;
                        case "CE":
                            button.Click += ClearEntry_Click;
                            break;
                        case "⌫":
                            button.Click += Backspace_Click;
                            break;
                        case "x²":
                            button.Click += Square_Click;
                            break;
                        case "²√x":
                            button.Click += SquareRoot_Click;
                            break;
                        case "1/x":
                            button.Click += OneOverX_Click;
                            break;
                        case "+/-":
                            button.Click += PlusMinus_Click;
                            break;
                        case "MC":
                            button.Click += MemoryClear_Click;
                            break;
                        case "MR":
                            button.Click += MemoryRecall_Click;
                            break;
                        case "MS":
                            button.Click += MemoryStore_Click;
                            break;
                        case "M+":
                            button.Click += MemoryAdd_Click;
                            break;
                        case "M-":
                            button.Click += MemorySubtract_Click;
                            break;
                        case "M>":
                    
                            break;
                    }
                }
                else
                {
                    AttachButtonEvents(child);
                }
            }
        }

        //Taste si butoane
        public void HandleKeyPress(KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.NumPad0:
                    case Key.D0:
                        HandleDigitInput("0");
                        e.Handled = true;
                        break;
                    case Key.NumPad1:
                    case Key.D1:
                        HandleDigitInput("1");
                        e.Handled = true;
                        break;
                    case Key.NumPad2:
                    case Key.D2:
                        HandleDigitInput("2");
                        e.Handled = true;
                        break;
                    case Key.NumPad3:
                    case Key.D3:
                        HandleDigitInput("3");
                        e.Handled = true;
                        break;
                    case Key.NumPad4:
                    case Key.D4:
                        HandleDigitInput("4");
                        e.Handled = true;
                        break;
                    case Key.D5 when Keyboard.Modifiers != ModifierKeys.Shift:
                    case Key.NumPad5 when Keyboard.Modifiers != ModifierKeys.Shift:
                        HandleDigitInput("5");
                        e.Handled = true;
                        break;
                    case Key.NumPad6:
                    case Key.D6:
                        HandleDigitInput("6");
                        e.Handled = true;
                        break;
                    case Key.NumPad7:
                    case Key.D7:
                        HandleDigitInput("7");
                        e.Handled = true;
                        break;
                    case Key.NumPad8:
                    case Key.D8:
                        HandleDigitInput("8");
                        e.Handled = true;
                        break;
                    case Key.NumPad9:
                    case Key.D9:
                        HandleDigitInput("9");
                        e.Handled = true;
                        break;

                    case Key.Decimal:
                    case Key.OemPeriod:
                    case Key.OemComma:
                        HandleDecimalInput();
                        e.Handled = true;
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        HandleOperation("-");
                        e.Handled = true;
                        break;
                    case Key.X:
                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            HandleOperation("x");
                            e.Handled = true;
                        }
                        break;
                    case Key.OemQuestion:
                    case Key.Divide:
                        HandleOperation("÷");
                        e.Handled = true;
                        break;
                    case Key.D5 when Keyboard.Modifiers == ModifierKeys.Shift:
                    case Key.NumPad5 when Keyboard.Modifiers == ModifierKeys.Shift:
                        HandlePercentOperation();
                        e.Handled = true;
                        break;
                    case Key.Enter:
                    case Key.OemPlus when Keyboard.Modifiers == ModifierKeys.Shift:
                        HandleOperation("=");
                        e.Handled = true;
                        break;
                    case Key.Escape:
                        ClearAll();
                        e.Handled = true;
                        break;
                    case Key.Back:
                        HandleBackspace();
                        e.Handled = true;
                        break;
                    case Key.OemPlus when Keyboard.Modifiers != ModifierKeys.Shift:
                    case Key.Add:
                        HandleOperation("+");
                        e.Handled = true;
                        break;
                    case Key.C:
                    case Key.A:
                    case Key.B:
                    case Key.D:
                    case Key.E:
                    case Key.F:
                        e.Handled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la procesarea tastei: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
        }

        //Operatii
        private void HandleDigitInput(string digit)
        {
            if (_isNewNumber || _currentNumberString == "0")
            {
                _currentNumberString = digit;
                _isNewNumber = false;
                _hasDecimal = false;
            }
            else if (_isResultDisplayed)
            {
                _currentNumberString = digit;
                _isResultDisplayed = false;
                _isNewNumber = false;
                _hasDecimal = false;
            }
            else
            {
                _currentNumberString += digit;
            }
            UpdateDisplay();
        }
        private void HandleDecimalInput()
        {
            string decimalSeparator = _currentCulture.NumberFormat.NumberDecimalSeparator;

            if (_isNewNumber || _isResultDisplayed)
            {
                _currentNumberString = "0" + decimalSeparator;
                _isNewNumber = false;
                _isResultDisplayed = false;
                _hasDecimal = true;
            }
            else if (!_hasDecimal)
            {
                _currentNumberString += decimalSeparator;
                _hasDecimal = true;
            }
            UpdateDisplay();
        }
        private void HandleOperation(string operation)
        {
            double currentValue = ParseCurrentValue();
            if (!string.IsNullOrEmpty(_engine.GetPendingOperation()) && !_isNewNumber)
            {
                _engine.Calculate(currentValue);
                _currentNumberString = _engine.Result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
            }
            else
            {
                _engine.SetValue(currentValue);
            }
            _engine.SetPendingOperation(operation);
            _isNewNumber = true;
            _isResultDisplayed = false;
        }
        private void HandleBackspace()
        {
            if (_isResultDisplayed || _isNewNumber)
                return;

            if (_currentNumberString.Length > 1)
            {
                if (_currentNumberString.EndsWith(_currentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    _hasDecimal = false;
                }

                _currentNumberString = _currentNumberString.Substring(0, _currentNumberString.Length - 1);
            }
            else
            {
                _currentNumberString = "0";
                _isNewNumber = true;
                _hasDecimal = false;
            }
            UpdateDisplay();
        }
        private void HandlePercentOperation()
        {
            double currentValue = ParseCurrentValue();

            if (!string.IsNullOrEmpty(_engine.GetPendingOperation()) && !_isNewNumber)
            {
                string pendingOp = _engine.GetPendingOperation();
                double leftOperand = _engine.Result;
                double percentValue = (leftOperand * currentValue) / 100;
                _engine.Calculate(percentValue);
                _currentNumberString = _engine.Result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
                _engine.SetPendingOperation("");
            }
            else
            {
                _currentNumberString = "0";
                _hasDecimal = false;
                UpdateDisplay();
                _engine.SetValue(0);
            }

            _isNewNumber = true;
            _isResultDisplayed = true;
        }
        private void HandlePlusMinus()
        {
            if (_currentNumberString != "0")
            {
                if (_currentNumberString.StartsWith("-"))
                {
                    _currentNumberString = _currentNumberString.Substring(1);
                }
                else
                {
                    _currentNumberString = "-" + _currentNumberString;
                }
                UpdateDisplay();
            }
        }
        private void ClearAll()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _engine.Reset();
            _isNewNumber = true;
            _isResultDisplayed = false;

            UpdateDisplay();
        }
        private void ClearEntry()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _isNewNumber = true;
            UpdateDisplay();
        }
        private void PerformCalculation()
        {
            double currentValue = ParseCurrentValue();
            try
            {
                _engine.Calculate(currentValue);
                _currentNumberString = _engine.Result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();

                _isNewNumber = true;
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateDisplay()
        {
            if (_useDigitGrouping)
            {
                string integerPart = _currentNumberString;
                string decimalPart = "";
                if (_currentNumberString.Contains(_currentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    string[] parts = _currentNumberString.Split(new[] { _currentCulture.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);
                    integerPart = parts[0];
                    decimalPart = parts.Length > 1 ? _currentCulture.NumberFormat.NumberDecimalSeparator + parts[1] : "";
                }
                bool isNegative = integerPart.StartsWith("-");
                if (isNegative)
                {
                    integerPart = integerPart.Substring(1);
                }
                string formattedInteger = FormatNumberWithGrouping(integerPart);
                if (isNegative)
                {
                    formattedInteger = "-" + formattedInteger;
                }
                ResultTextBox.Text = formattedInteger + decimalPart;
            }
            else
            {
                ResultTextBox.Text = _currentNumberString;
            }
        }

        //Digit grouping
        public void SetDigitGrouping(bool useGrouping)
        {
            _useDigitGrouping = useGrouping;
            UpdateDisplay();
        }
        private string FormatNumberWithGrouping(string number)
        {
            bool isNegative = number.StartsWith("-");
            if (isNegative)
            {
                number = number.Substring(1);
            }
            if (number.Length <= 3)
            {
                return isNegative ? "-" + number : number;
            }
            string separator = _currentCulture.NumberFormat.NumberGroupSeparator;
            string result = "";

            int count = 0;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                result = number[i] + result;
                count++;

                if (count % 3 == 0 && i > 0)
                {
                    result = separator + result;
                }
            }

            return isNegative ? "-" + result : result;
        }

        private double ParseCurrentValue()
        {
            if (double.TryParse(_currentNumberString,
                              NumberStyles.Any,
                              _currentCulture,
                              out double result))
            {
                return result;
            }

            MessageBox.Show("Format invalid de număr!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }

        public void SetDisplayValue(double value)
        {
            _currentNumberString = value.ToString(CultureInfo.InvariantCulture);
            _hasDecimal = _currentNumberString.Contains(".");
            if (_hasDecimal)
            {
                _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
            }
            UpdateDisplay();

            _isNewNumber = true;
            _isResultDisplayed = true;
        }
        public void Reset()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _engine.Reset();
            _isNewNumber = true;
            _isResultDisplayed = false;
            UpdateDisplay();
        }
        public bool SetValueFromString(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, _currentCulture, out double numericValue))
            {
                _currentNumberString = numericValue.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
                _engine.SetValue(numericValue);

                _isNewNumber = true;
                _isResultDisplayed = true;

                return true;
            }

            return false;
        }

        //Click events
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Content.ToString();
            HandleDigitInput(digit);
        }
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            HandleDecimalInput();
        }
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string operation = button.Content.ToString();

            if (operation == "%")
            {
                HandlePercentOperation();
            }
            else
            {
                HandleOperation(operation);
            }
        }
        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            PerformCalculation();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearEntry();
        }
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            HandleBackspace();
        }
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.Square(value);
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.SquareRoot(value);
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OneOverX_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.Reciprocal(value);
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PlusMinus_Click(object sender, RoutedEventArgs e)
        {
            HandlePlusMinus();
        }

        //Memory click
        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryClear();
        }
        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            double memoryValue = _memoryManager.MemoryRecall();
            SetDisplayValue(memoryValue);
        }
        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryStore(ParseCurrentValue());
        }
        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryAdd(ParseCurrentValue());
        }
        private void MemorySubtract_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemorySubtract(ParseCurrentValue());
        }

        //About
        public void ShowAboutInfo()
        {
            MessageBox.Show(
                "Dezvoltator: Palatka Vanessa\nGrupa: 10LF332",
                "Despre Calculator",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}