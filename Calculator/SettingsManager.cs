using System;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

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
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string calculatorDir = Path.Combine(appDataPath, "Calculator");

            // Creează directorul dacă nu există
            if (!Directory.Exists(calculatorDir))
            {
                try
                {
                    Directory.CreateDirectory(calculatorDir);
                }
                catch (Exception)
                {
                    // Tratează eroarea - eventual loghează sau notifică utilizatorul
                }
            }

            _settingsFilePath = Path.Combine(calculatorDir, "settings.xml");
            LoadSettings();
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
                catch (Exception)
                {
                    // În caz de eroare, creează setări implicite
                    _currentSettings = new CalculatorSettings();
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
            catch (Exception)
            {
                // Tratează eroarea de salvare - eventual notifică utilizatorul
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
    }

    /// <summary>
    /// Clasa pentru stocarea setărilor calculatorului
    /// </summary>
    [Serializable]
    public class CalculatorSettings
    {
        // Setarea pentru gruparea cifrelor
        public bool UseDigitGrouping { get; set; }

        /// <summary>
        /// Constructor implicit
        /// </summary>
        public CalculatorSettings()
        {
            // Valoarea implicită pentru digit grouping este dezactivată
            UseDigitGrouping = false;
        }
    }
}