using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            if (_isNewNumber || ResultTextBox.Text == "0")
            {
                ResultTextBox.Text = digit;
                _isNewNumber = false;
            }
            else if (_isResultDisplayed)
            {
                ResultTextBox.Text = digit;
                _isResultDisplayed = false;
                _isNewNumber = false;
            }
            else
            {
                ResultTextBox.Text += digit;
            }
        }

        /// <summary>
        /// Gestionează intrarea unui separator zecimal
        /// </summary>
        private void HandleDecimalInput()
        {
            if (_isNewNumber || _isResultDisplayed)
            {
                ResultTextBox.Text = "0,";
                _isNewNumber = false;
                _isResultDisplayed = false;
            }
            else if (!ResultTextBox.Text.Contains(","))
            {
                ResultTextBox.Text += ",";
            }
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
                UpdateDisplay(_engine.Result);
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

            string text = ResultTextBox.Text;
            if (text.Length > 1)
            {
                ResultTextBox.Text = text.Substring(0, text.Length - 1);
            }
            else
            {
                ResultTextBox.Text = "0";
                _isNewNumber = true;
            }
        }

        /// <summary>
        /// Gestionează schimbarea semnului numărului (+/-)
        /// </summary>
        private void HandlePlusMinus()
        {
            string text = ResultTextBox.Text;

            if (text != "0")
            {
                if (text.StartsWith("-"))
                {
                    ResultTextBox.Text = text.Substring(1);
                }
                else
                {
                    ResultTextBox.Text = "-" + text;
                }
            }
        }

        /// <summary>
        /// Șterge toată starea calculatorului
        /// </summary>
        private void ClearAll()
        {
            ResultTextBox.Text = "0";
            _engine.Reset();
            _isNewNumber = true;
            _isResultDisplayed = false;
        }

        /// <summary>
        /// Șterge doar intrarea curentă
        /// </summary>
        private void ClearEntry()
        {
            ResultTextBox.Text = "0";
            _isNewNumber = true;
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
                UpdateDisplay(_engine.Result);
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
            if (double.TryParse(ResultTextBox.Text.Replace(',', '.'),
                               System.Globalization.NumberStyles.Any,
                               System.Globalization.CultureInfo.InvariantCulture,
                               out double result))
            {
                return result;
            }

            MessageBox.Show("Format invalid de număr!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }

        /// <summary>
        /// Actualizează afișajul cu valoarea specificată
        /// </summary>
        /// <param name="value">Valoarea care trebuie afișată</param>
        private void UpdateDisplay(double value)
        {
            ResultTextBox.Text = value.ToString("G15").Replace('.', ',');
        }

        /// <summary>
        /// Setează o valoare în afișaj (folosită pentru valorile selectate din memorie)
        /// </summary>
        /// <param name="value">Valoarea de afișat</param>
        public void SetDisplayValue(double value)
        {
            UpdateDisplay(value);
            _isNewNumber = true;
            _isResultDisplayed = true;
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
                UpdateDisplay(result);
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
                UpdateDisplay(result);
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
                UpdateDisplay(result);
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
            UpdateDisplay(memoryValue);
            _isNewNumber = true;
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
    }
}