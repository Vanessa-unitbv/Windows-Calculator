using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Clasă care implementează operațiile de clipboard (Cut, Copy, Paste) 
    /// fără a folosi funcțiile preimplementate ale controalelor
    /// </summary>
    public class ClipboardManager
    {
        // Referință la TextBox-ul cu rezultatul calculatorului
        private readonly TextBox _resultTextBox;

        // Referință la CalculatorManager pentru a-i comunica schimbările
        private readonly CalculatorManager _calculatorManager;

        // Valoare privată pentru stocarea textului în clipboard
        private string _clipboardText = string.Empty;

        /// <summary>
        /// Constructor pentru managerul de clipboard
        /// </summary>
        /// <param name="resultTextBox">TextBox-ul calculatorului</param>
        /// <param name="calculatorManager">Managerul calculatorului</param>
        public ClipboardManager(TextBox resultTextBox, CalculatorManager calculatorManager)
        {
            _resultTextBox = resultTextBox;
            _calculatorManager = calculatorManager;
        }

        /// <summary>
        /// Obține valoarea curentă din clipboard
        /// </summary>
        /// <returns>Valoarea stocată în clipboard</returns>
        public string GetClipboardText()
        {
            return _clipboardText;
        }

        /// <summary>
        /// Copiază textul din TextBox în clipboard-ul intern
        /// </summary>
        public void Copy()
        {
            if (_resultTextBox != null && !string.IsNullOrEmpty(_resultTextBox.Text))
            {
                _clipboardText = _resultTextBox.Text;

                // Opțional: copiază și în clipboard-ul sistemului
                try
                {
                    Clipboard.SetText(_clipboardText);
                }
                catch (Exception)
                {
                    // Ignoră erorile de clipboard (pot apărea în anumite medii)
                }
            }
        }

        /// <summary>
        /// Copiază textul din TextBox în clipboard-ul intern și șterge textul
        /// </summary>
        public void Cut()
        {
            if (_resultTextBox != null && !string.IsNullOrEmpty(_resultTextBox.Text))
            {
                // Salvează valoarea curentă
                _clipboardText = _resultTextBox.Text;

                // Resetează calculatorul
                _calculatorManager.Reset();

                // Opțional: copiază și în clipboard-ul sistemului
                try
                {
                    Clipboard.SetText(_clipboardText);
                }
                catch (Exception)
                {
                    // Ignoră erorile de clipboard
                }
            }
        }

        /// <summary>
        /// Lipește textul din clipboard-ul intern în TextBox
        /// </summary>
        /// <returns>True dacă operația a reușit, False în caz contrar</returns>
        public bool Paste()
        {
            // Verifică dacă avem o valoare în clipboard-ul intern
            if (!string.IsNullOrEmpty(_clipboardText))
            {
                // Verifică dacă valoarea este un număr valid
                if (IsValidNumber(_clipboardText))
                {
                    // Setează valoarea în calculator
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
                // Încearcă să obțină text din clipboard-ul sistemului
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
                    // Ignoră erorile de clipboard
                }
            }

            return false;
        }

        /// <summary>
        /// Verifică dacă un string este un număr valid pentru calculator
        /// </summary>
        /// <param name="text">Textul care trebuie verificat</param>
        /// <returns>True dacă este număr valid, False în caz contrar</returns>
        private bool IsValidNumber(string text)
        {
            // Înlocuim virgula cu punct pentru parsare
            string normalizedText = text.Replace(',', '.');

            // Încercăm să parsăm ca double
            return double.TryParse(normalizedText,
                                  System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture,
                                  out _);
        }
    }
}