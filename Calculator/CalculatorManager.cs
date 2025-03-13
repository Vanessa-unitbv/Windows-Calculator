using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

namespace Calculator
{
    /// <summary>
    /// Clasa care coordonează toate componentele calculatorului și gestionează evenimentele de butoane
    /// </summary>
    public class CalculatorManager
    {
        private readonly MainWindow _mainWindow;
        private readonly CalculatorEngine _engine;
        private readonly CalculatorMemoryManager _memoryManager;

        // Referință la TextBox-ul pentru afișare
        private TextBox ResultTextBox => _mainWindow.ResultTextBox;

        // Flag pentru a ști dacă următoarea cifră începe un număr nou
        private bool _isNewNumber = true;

        // Flag pentru a ști dacă rezultatul este afișat
        private bool _isResultDisplayed = false;

        // Flag pentru a controla gruparea digitală
        private bool _useDigitGrouping = false;

        // Stocăm valoarea actuală ca string pentru a facilita formatarea
        private string _currentNumberString = "0";

        // Stocăm dacă numărul conține zecimale
        private bool _hasDecimal = false;

        // Cultura curentă a sistemului
        private readonly CultureInfo _currentCulture;

        /// <summary>
        /// Constructor pentru managerul calculatorului
        /// </summary>
        /// <param name="mainWindow">Referință la fereastra principală</param>
        /// <param name="memoryManager">Referință la managerul de memorie</param>
        public CalculatorManager(MainWindow mainWindow, CalculatorMemoryManager memoryManager)
        {
            _mainWindow = mainWindow;
            _engine = new CalculatorEngine();
            _memoryManager = memoryManager;

            // Obține cultura curentă a sistemului
            _currentCulture = CultureInfo.CurrentCulture;

            // Atașează handler-uri de evenimente pentru butoane
            AttachEventHandlers();
        }

        #region Event Attachment

        /// <summary>
        /// Atașează handler-uri de evenimente pentru toate butoanele din interfață
        /// </summary>
        private void AttachEventHandlers()
        {
            // Obține toate butoanele din interfață
            foreach (var child in LogicalTreeHelper.GetChildren(_mainWindow))
            {
                if (child is Grid grid)
                {
                    AttachButtonEvents(grid);
                }
            }
        }

        /// <summary>
        /// Atașează handler-uri de evenimente pentru butoanele din grid-uri
        /// </summary>
        /// <param name="parent">Elementul părinte care conține butoanele</param>
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
                            // Acest buton este gestionat direct în MainWindow.xaml.cs
                            break;
                    }
                }
                else
                {
                    // Recursiv pentru fiecare copil Grid
                    AttachButtonEvents(child);
                }
            }
        }

        #endregion

        #region Keyboard Handling

        /// <summary>
        /// Gestionează apăsările de taste de la tastatură
        /// </summary>
        /// <param name="e">Datele evenimentului</param>
        public void HandleKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.D0:
                    HandleDigitInput("0");
                    break;
                case Key.NumPad1:
                case Key.D1:
                    HandleDigitInput("1");
                    break;
                case Key.NumPad2:
                case Key.D2:
                    HandleDigitInput("2");
                    break;
                case Key.NumPad3:
                case Key.D3:
                    HandleDigitInput("3");
                    break;
                case Key.NumPad4:
                case Key.D4:
                    HandleDigitInput("4");
                    break;
                case Key.NumPad5:
                case Key.D5:
                    HandleDigitInput("5");
                    break;
                case Key.NumPad6:
                case Key.D6:
                    HandleDigitInput("6");
                    break;
                case Key.NumPad7:
                case Key.D7:
                    HandleDigitInput("7");
                    break;
                case Key.NumPad8:
                case Key.D8:
                    HandleDigitInput("8");
                    break;
                case Key.NumPad9:
                case Key.D9:
                    HandleDigitInput("9");
                    break;
                case Key.Decimal:
                case Key.OemPeriod:
                case Key.OemComma:
                    HandleDecimalInput();
                    break;
                case Key.OemPlus:
                    HandleOperation("+");
                    break;
                case Key.OemMinus:
                    HandleOperation("-");
                    break;
                case Key.X:
                    HandleOperation("x");
                    break;
                case Key.OemQuestion:
                    HandleOperation("÷");
                    break;
                case Key.Enter:
                    HandleOperation("=");
                    break;
                case Key.Escape:
                    ClearAll();
                    break;
                case Key.Back:
                    HandleBackspace();
                    break;
            }
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Gestionează intrarea unei cifre
        /// </summary>
        /// <param name="digit">Cifra care trebuie adăugată</param>
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
                // Adăugăm cifra la numărul curent
                _currentNumberString += digit;
            }

            // Afișăm numărul cu sau fără grupare
            UpdateDisplay();
        }

        /// <summary>
        /// Gestionează intrarea unui separator zecimal
        /// </summary>
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

            // Afișăm numărul cu separatorul zecimal
            UpdateDisplay();
        }

        /// <summary>
        /// Gestionează operațiile (+, -, *, /, %)
        /// </summary>
        /// <param name="operation">Operația de efectuat</param>
        private void HandleOperation(string operation)
        {
            double currentValue = ParseCurrentValue();

            // Verifică dacă avem o operație anterioară neefectuată
            if (!string.IsNullOrEmpty(_engine.GetPendingOperation()) && !_isNewNumber)
            {
                // Dacă da, finalizează acea operație înainte de a începe una nouă
                _engine.Calculate(currentValue);

                // Actualizăm string-ul curent
                _currentNumberString = _engine.Result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm rezultatul
                UpdateDisplay();
            }
            else
            {
                // Altfel, doar setează valoarea curentă ca rezultat
                _engine.SetValue(currentValue);
            }

            // Setează noua operație
            _engine.SetPendingOperation(operation);
            _isNewNumber = true;
            _isResultDisplayed = false;
        }

        /// <summary>
        /// Gestionează acțiunea de backspace
        /// </summary>
        private void HandleBackspace()
        {
            if (_isResultDisplayed || _isNewNumber)
                return;

            if (_currentNumberString.Length > 1)
            {
                // Verificăm dacă am șters separatorul zecimal
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

            // Actualizăm afișajul
            UpdateDisplay();
        }

        /// <summary>
        /// Gestionează schimbarea semnului numărului (+/-)
        /// </summary>
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

                // Actualizăm afișajul
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Șterge toată starea calculatorului
        /// </summary>
        private void ClearAll()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _engine.Reset();
            _isNewNumber = true;
            _isResultDisplayed = false;

            // Actualizăm afișajul
            UpdateDisplay();
        }

        /// <summary>
        /// Șterge doar intrarea curentă
        /// </summary>
        private void ClearEntry()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _isNewNumber = true;

            // Actualizăm afișajul
            UpdateDisplay();
        }

        /// <summary>
        /// Efectuează calculul și afișează rezultatul
        /// </summary>
        private void PerformCalculation()
        {
            double currentValue = ParseCurrentValue();
            try
            {
                _engine.Calculate(currentValue);

                // Actualizăm string-ul curent
                _currentNumberString = _engine.Result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm rezultatul
                UpdateDisplay();

                _isNewNumber = true;
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parsează textul curent din TextBox la un număr double
        /// </summary>
        /// <returns>Valoarea numerică din TextBox</returns>
        private double ParseCurrentValue()
        {
            // Convertim valoarea curentă la număr
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

        /// <summary>
        /// Actualizează afișajul cu valoarea curentă
        /// </summary>
        /// <summary>
        /// Actualizează afișajul cu valoarea curentă
        /// </summary>
        private void UpdateDisplay()
        {
            // Verificăm dacă trebuie să aplicăm gruparea
            if (_useDigitGrouping)
            {
                // Aplicăm gruparea pentru orice număr, chiar și cu zecimale
                string integerPart = _currentNumberString;
                string decimalPart = "";

                // Separăm partea întreagă de partea zecimală
                if (_currentNumberString.Contains(_currentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    string[] parts = _currentNumberString.Split(new[] { _currentCulture.NumberFormat.NumberDecimalSeparator }, StringSplitOptions.None);
                    integerPart = parts[0];
                    decimalPart = parts.Length > 1 ? _currentCulture.NumberFormat.NumberDecimalSeparator + parts[1] : "";
                }

                // Verificăm dacă avem un număr negativ
                bool isNegative = integerPart.StartsWith("-");
                if (isNegative)
                {
                    integerPart = integerPart.Substring(1);
                }

                // Aplicăm gruparea doar pentru partea întreagă
                string formattedInteger = FormatNumberWithGrouping(integerPart);

                // Adăugăm semnul negativ dacă e cazul
                if (isNegative)
                {
                    formattedInteger = "-" + formattedInteger;
                }

                // Setăm textul cu grupare pentru partea întreagă și păstrăm partea zecimală
                ResultTextBox.Text = formattedInteger + decimalPart;
            }
            else
            {
                // Afișăm numărul fără grupare
                ResultTextBox.Text = _currentNumberString;
            }
        }


        /// <summary>
        /// Formatează un număr întreg cu separatoarele de grupare
        /// </summary>
        private string FormatNumberWithGrouping(string number)
        {
            // Eliminăm eventualele semne negative pentru formatare
            bool isNegative = number.StartsWith("-");
            if (isNegative)
            {
                number = number.Substring(1);
            }

            // Dacă numărul e prea mic, nu aplicăm gruparea
            if (number.Length <= 3)
            {
                return isNegative ? "-" + number : number;
            }

            // Aplicăm gruparea
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

        /// <summary>
        /// Setează o valoare în afișaj (folosită pentru valorile selectate din memorie)
        /// </summary>
        /// <param name="value">Valoarea de afișat</param>
        public void SetDisplayValue(double value)
        {
            // Actualizăm string-ul curent
            _currentNumberString = value.ToString(CultureInfo.InvariantCulture);
            _hasDecimal = _currentNumberString.Contains(".");

            // Înlocuim punctul cu separatorul zecimal specific culturii
            if (_hasDecimal)
            {
                _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
            }

            // Afișăm valoarea
            UpdateDisplay();

            _isNewNumber = true;
            _isResultDisplayed = true;
        }

        /// <summary>
        /// Resetează calculatorul la starea inițială
        /// </summary>
        public void Reset()
        {
            _currentNumberString = "0";
            _hasDecimal = false;
            _engine.Reset();
            _isNewNumber = true;
            _isResultDisplayed = false;

            // Actualizăm afișajul
            UpdateDisplay();
        }

        /// <summary>
        /// Setează valoarea calculatorului din string
        /// </summary>
        /// <param name="value">Valoarea ca string</param>
        /// <returns>True dacă setarea a reușit, False în caz contrar</returns>
        public bool SetValueFromString(string value)
        {
            // Încercăm să parsăm valoarea
            if (double.TryParse(value, NumberStyles.Any, _currentCulture, out double numericValue))
            {
                // Actualizăm string-ul curent
                _currentNumberString = numericValue.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm valoarea
                UpdateDisplay();

                // Setăm valoarea în motor
                _engine.SetValue(numericValue);

                _isNewNumber = true;
                _isResultDisplayed = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Setează gruparea digitală pentru afișare
        /// </summary>
        /// <param name="useGrouping">True pentru a activa gruparea, False pentru a o dezactiva</param>
        public void SetDigitGrouping(bool useGrouping)
        {
            _useDigitGrouping = useGrouping;

            // Actualizăm afișajul cu starea curentă
            UpdateDisplay();
        }

        #endregion

        #region Event Handlers

        // Eveniment pentru cifrele 0-9
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Content.ToString();
            HandleDigitInput(digit);
        }

        // Eveniment pentru butonul zecimal (virgulă)
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            HandleDecimalInput();
        }

        // Eveniment pentru operatori (+, -, x, ÷, %)
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string operation = button.Content.ToString();
            HandleOperation(operation);
        }

        // Eveniment pentru butonul egal
        private void Equal_Click(object sender, RoutedEventArgs e)
        {
            PerformCalculation();
        }

        // Eveniment pentru butonul C (Clear)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        // Eveniment pentru butonul CE (Clear Entry)
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearEntry();
        }

        // Eveniment pentru butonul Backspace
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            HandleBackspace();
        }

        // Eveniment pentru butonul Square (x²)
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.Square(value);
                // Actualizăm string-ul curent
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm rezultatul
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Eveniment pentru butonul Square Root (²√x)
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.SquareRoot(value);
                // Actualizăm string-ul curent
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm rezultatul
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Eveniment pentru butonul 1/x
        private void OneOverX_Click(object sender, RoutedEventArgs e)
        {
            double value = ParseCurrentValue();
            try
            {
                double result = _engine.Reciprocal(value);
                // Actualizăm string-ul curent
                _currentNumberString = result.ToString(CultureInfo.InvariantCulture);
                _hasDecimal = _currentNumberString.Contains(".");

                // Înlocuim punctul cu separatorul zecimal specific culturii
                if (_hasDecimal)
                {
                    _currentNumberString = _currentNumberString.Replace(".", _currentCulture.NumberFormat.NumberDecimalSeparator);
                }

                // Afișăm rezultatul
                UpdateDisplay();
                _isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Eveniment pentru butonul +/-
        private void PlusMinus_Click(object sender, RoutedEventArgs e)
        {
            HandlePlusMinus();
        }

        #endregion

        #region Memory Event Handlers

        // Eveniment pentru butonul MC (Memory Clear)
        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryClear();
        }

        // Eveniment pentru butonul MR (Memory Recall)
        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            double memoryValue = _memoryManager.MemoryRecall();
            SetDisplayValue(memoryValue);
        }

        // Eveniment pentru butonul MS (Memory Store)
        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryStore(ParseCurrentValue());
        }

        // Eveniment pentru butonul M+ (Memory Add)
        private void MemoryAdd_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemoryAdd(ParseCurrentValue());
        }

        // Eveniment pentru butonul M- (Memory Subtract)
        private void MemorySubtract_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.MemorySubtract(ParseCurrentValue());
        }

        #endregion

        #region Menu Operations

        /// <summary>
        /// Afișează informații despre aplicație
        /// </summary>
        public void ShowAboutInfo()
        {
            MessageBox.Show(
                "Dezvoltator: Palatka Vanessa\nGrupa: 10LF332",
                "Despre Calculator",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        #endregion
    }
}