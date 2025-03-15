using System;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Calculator
{
    /// <summary>
    /// Clasa pentru gestionarea setărilor persistente ale aplicației
    /// </summary>
    public class SettingsManager
    {
        // Singleton instance
        private static SettingsManager _instance;

        // Calea către fișierul de setări
        private readonly string _settingsFilePath;

        // Setările actuale
        private CalculatorSettings _currentSettings;

        /// <summary>
        /// Obține instanța singleton a SettingsManager
        /// </summary>
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Constructor privat pentru singleton
        /// </summary>
        private SettingsManager()
        {
            // Obține directorul aplicației pentru a stoca setările
            string appDirectory = GetApplicationDirectory();

            // Calea către fișierul de setări în directorul aplicației
            _settingsFilePath = Path.Combine(appDirectory, "settings.xml");


            LoadSettings();
        }

        /// <summary>
        /// Obține directorul curent al aplicației
        /// </summary>
        private string GetApplicationDirectory()
        {
            // Obține directorul de execuție al aplicației
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Încarcă setările din fișier sau creează setări implicite dacă fișierul nu există
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CalculatorSettings));
                    using (FileStream fs = new FileStream(_settingsFilePath, FileMode.Open))
                    {
                        _currentSettings = (CalculatorSettings)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    // În caz de eroare, creează setări implicite
                    _currentSettings = new CalculatorSettings();

                    // Afișează eroarea pentru depanare (poți elimina această linie după testare)
                    MessageBox.Show($"Eroare la încărcarea setărilor: {ex.Message}");
                }
            }
            else
            {
                // Creează setări implicite dacă fișierul nu există
                _currentSettings = new CalculatorSettings();
            }
        }

        /// <summary>
        /// Salvează setările curente în fișier
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CalculatorSettings));
                using (FileStream fs = new FileStream(_settingsFilePath, FileMode.Create))
                {
                    serializer.Serialize(fs, _currentSettings);
                }
            }
            catch (Exception ex)
            {
                // Afișează eroarea pentru depanare (poți elimina această linie după testare)
                MessageBox.Show($"Eroare la salvarea setărilor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obține sau setează opțiunea de digit grouping
        /// </summary>
        public bool UseDigitGrouping
        {
            get { return _currentSettings.UseDigitGrouping; }
            set
            {
                _currentSettings.UseDigitGrouping = value;
                SaveSettings();
            }
        }

        /// <summary>
        /// Obține sau setează ultimul mod selectat al calculatorului
        /// </summary>
        public string LastCalculatorMode
        {
            get { return _currentSettings.LastCalculatorMode; }
            set
            {
                _currentSettings.LastCalculatorMode = value;
                SaveSettings();
            }
        }
    }

    /// <summary>
    /// Clasa pentru stocarea setărilor calculatorului
    /// </summary>
    [Serializable]
    public class CalculatorSettings
    {
        // Setarea pentru gruparea cifrelor
        public bool UseDigitGrouping { get; set; }

        // Setarea pentru ultimul mod selectat al calculatorului
        public string LastCalculatorMode { get; set; }

        /// <summary>
        /// Constructor implicit
        /// </summary>
        public CalculatorSettings()
        {
            // Valoarea implicită pentru digit grouping este dezactivată
            UseDigitGrouping = false;

            // Valoarea implicită pentru modul calculatorului este Standard
            LastCalculatorMode = "Standard";
        }
    }
}