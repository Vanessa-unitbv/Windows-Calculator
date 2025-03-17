using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace Calculator
{
    public class ClipboardManager
    {
        private readonly TextBox _resultTextBox;
        private readonly CalculatorManager _calculatorManager;
        private string _clipboardText = string.Empty;
        private readonly CultureInfo _currentCulture;
        public ClipboardManager(TextBox resultTextBox, CalculatorManager calculatorManager)
        {
            _resultTextBox = resultTextBox;
            _calculatorManager = calculatorManager;
            _currentCulture = CultureInfo.CurrentCulture;
        }
        public string GetClipboardText()
        {
            return _clipboardText;
        }
        public void Copy()
        {
            if (_resultTextBox != null && !string.IsNullOrEmpty(_resultTextBox.Text))
            {
                _clipboardText = _resultTextBox.Text;
                try
                {
                    Clipboard.SetText(_clipboardText);
                }
                catch (Exception)
                {
                }
            }
        }
        public void Cut()
        {
            if (_resultTextBox != null && !string.IsNullOrEmpty(_resultTextBox.Text))
            {
                _clipboardText = _resultTextBox.Text;
                _calculatorManager.Reset();
                try
                {
                    Clipboard.SetText(_clipboardText);
                }
                catch (Exception)
                {

                }
            }
        }
        public bool Paste()
        {
            if (!string.IsNullOrEmpty(_clipboardText))
            {
                if (IsValidNumber(_clipboardText))
                {
                    _calculatorManager.SetValueFromString(_clipboardText);
                    return true;
                }
                else
                {
                    MessageBox.Show("Valoarea din clipboard nu este un număr valid.",
                                    "Eroare de lipire",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
            else
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        string systemClipboardText = Clipboard.GetText();
                        if (IsValidNumber(systemClipboardText))
                        {
                            _clipboardText = systemClipboardText;
                            _calculatorManager.SetValueFromString(systemClipboardText);
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Valoarea din clipboard nu este un număr valid.",
                                           "Eroare de lipire",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return false;
        }
        private bool IsValidNumber(string text)
        {
            return double.TryParse(text,
                                  NumberStyles.Any,
                                  _currentCulture,
                                  out _);
        }
    }
}