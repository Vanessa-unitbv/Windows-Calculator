using System;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Calculator
{
    public class SettingsManager
    {
        private static SettingsManager _instance;
        private readonly string _settingsFilePath;
        private CalculatorSettings _currentSettings;
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
        private SettingsManager()
        {
            string appDirectory = GetApplicationDirectory();
            _settingsFilePath = Path.Combine(appDirectory, "settings.xml");
            LoadSettings();
        }
        private string GetApplicationDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
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
                    _currentSettings = new CalculatorSettings();
                    MessageBox.Show($"Eroare la încărcarea setărilor: {ex.Message}");
                }
            }
            else
            {
                _currentSettings = new CalculatorSettings();
            }
        }
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
                MessageBox.Show($"Eroare la salvarea setărilor: {ex.Message}");
            }
        }
        public bool UseDigitGrouping
        {
            get { return _currentSettings.UseDigitGrouping; }
            set
            {
                _currentSettings.UseDigitGrouping = value;
                SaveSettings();
            }
        }
        public string LastCalculatorMode
        {
            get { return _currentSettings.LastCalculatorMode; }
            set
            {
                _currentSettings.LastCalculatorMode = value;
                SaveSettings();
            }
        }
        public string LastNumberSystem
        {
            get { return _currentSettings.LastNumberSystem; }
            set
            {
                _currentSettings.LastNumberSystem = value;
                SaveSettings();
            }
        }
        public bool UseOrderOfOperations
        {
            get { return _currentSettings.UseOrderOfOperations; }
            set
            {
                _currentSettings.UseOrderOfOperations = value;
                SaveSettings();
            }
        }
    }
    [Serializable]
    public class CalculatorSettings
    {
        public bool UseDigitGrouping { get; set; }
        public string LastCalculatorMode { get; set; }
        public string LastNumberSystem { get; set; }

        public bool UseOrderOfOperations { get; set; }
        public CalculatorSettings()
        {
            UseDigitGrouping = false;
            LastCalculatorMode = "Standard";
            LastNumberSystem = "HEX";
            UseOrderOfOperations = false;
        }
    }
}