using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculatorManager _calculatorManager;
        private ProgrammerCalculatorManager _programmerCalculatorManager;
        private CalculatorMemoryManager _memoryManager;
        private CalculatorModeManager _modeManager;
        private ClipboardManager _clipboardManager;

        public MainWindow()
        {
            InitializeComponent();

            // Setează fereastra să nu fie redimensionabilă
            ResizeMode = ResizeMode.NoResize;

            // Setează TextBox să nu fie editabil direct
            ResultTextBox.IsReadOnly = true;
            ResultTextBox.Text = "0";

            // Inițializează managerul de memorie
            _memoryManager = new CalculatorMemoryManager(MemoryListBox);

            // Inițializează și configurează managerul calculatorului Standard
            _calculatorManager = new CalculatorManager(this, _memoryManager);

            // Inițializează și configurează managerul calculatorului Programmer
            _programmerCalculatorManager = new ProgrammerCalculatorManager(this);

            // Inițializează managerul de clipboard
            _clipboardManager = new ClipboardManager(ResultTextBox, _calculatorManager);

            // Inițializează managerul de moduri
            _modeManager = new CalculatorModeManager(this, _calculatorManager, _programmerCalculatorManager, _memoryManager);

            // Atașează evenimentul pentru tastele de la tastatură
            KeyDown += MainWindow_KeyDown;

            // Atașează evenimentele pentru meniu
            AttachMenuEvents();

            // Încarcă setările salvate
            LoadSettings();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Trimite tastele doar la managerul modului standard
            if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
            {
                _calculatorManager.HandleKeyPress(e);
            }
        }

        /// <summary>
        /// Încarcă setările salvate
        /// </summary>
        private void LoadSettings()
        {
            // Obține setarea pentru digit grouping din SettingsManager
            bool useDigitGrouping = SettingsManager.Instance.UseDigitGrouping;

            // Găsește și actualizează starea checkbox-ului din meniu
            if (FindMenuItem("Digit Grouping") is MenuItem digitGroupingMenuItem)
            {
                digitGroupingMenuItem.IsChecked = useDigitGrouping;
            }

            // Setează gruparea de digiti în calculator
            _calculatorManager.SetDigitGrouping(useDigitGrouping);

            // Încarcă setările specifice modului
            _modeManager.LoadSettings();
        }

        /// <summary>
        /// Gestionează evenimentul de afișare/ascundere a stivei de memorie
        /// </summary>
        private void MemoryShow_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.ToggleMemoryStack();
        }

        /// <summary>
        /// Gestionează evenimentul de dublu click pe un element din stiva de memorie
        /// </summary>
        private void MemoryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MemoryListBox.SelectedItem != null)
            {
                // Obține valoarea selectată și o setează în calculator
                double selectedValue = _memoryManager.GetSelectedMemoryValue();
                _calculatorManager.SetDisplayValue(selectedValue);
            }
        }

        /// <summary>
        /// Atașează evenimentele pentru elementele de meniu
        /// </summary>
        private void AttachMenuEvents()
        {
            // Atașează evenimentele pentru Cut, Copy, Paste
            if (FindMenuItem("Cut") is MenuItem cutMenuItem)
            {
                cutMenuItem.Click += CutMenuItem_Click;
            }

            if (FindMenuItem("Copy") is MenuItem copyMenuItem)
            {
                copyMenuItem.Click += CopyMenuItem_Click;
            }

            if (FindMenuItem("Paste") is MenuItem pasteMenuItem)
            {
                pasteMenuItem.Click += PasteMenuItem_Click;
            }

            if (FindMenuItem("Digit Grouping") is MenuItem digitGroupingMenuItem)
            {
                // Asigură-te că este checkable
                digitGroupingMenuItem.IsCheckable = true;

                // Adaugă handler pentru eveniment
                digitGroupingMenuItem.Click += DigitGroupingMenuItem_Click;
            }
        }

        /// <summary>
        /// Găsește un element de meniu după textul său
        /// </summary>
        /// <param name="header">Textul elementului de meniu</param>
        /// <returns>Elementul de meniu găsit sau null</returns>
        private MenuItem FindMenuItem(string header)
        {
            foreach (var item in MainMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    foreach (var subItem in menuItem.Items)
                    {
                        if (subItem is MenuItem subMenuItem && subMenuItem.Header.ToString() == header)
                        {
                            return subMenuItem;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Eveniment pentru opțiunea Cut din meniu
        /// </summary>
        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Cut();
        }

        /// <summary>
        /// Eveniment pentru opțiunea Copy din meniu
        /// </summary>
        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Copy();
        }

        /// <summary>
        /// Eveniment pentru opțiunea Paste din meniu
        /// </summary>
        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Paste();
        }

        /// <summary>
        /// Eveniment pentru opțiunea Digit Grouping din meniu
        /// </summary>
        private void DigitGroupingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                // Luam direct starea actuală a checkbox-ului
                bool isChecked = menuItem.IsChecked;

                // Setează gruparea de digiti în calculator
                _calculatorManager.SetDigitGrouping(isChecked);

                // Salvează setarea în SettingsManager
                SettingsManager.Instance.UseDigitGrouping = isChecked;
            }
        }

        /// <summary>
        /// Gestionează evenimentul de click pe meniul About
        /// </summary>
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Folosește CalculatorManager pentru a afișa informații despre aplicație
            _calculatorManager.ShowAboutInfo();
        }

        /// <summary>
        /// Eveniment pentru selectarea modului Standard - delegat către ModeManager
        /// </summary>
        private void StandardModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Nu avem nevoie să implementăm această metodă, deoarece evenimentele sunt atașate în CalculatorModeManager
        }

        /// <summary>
        /// Eveniment pentru selectarea modului Programmer - delegat către ModeManager
        /// </summary>
        private void ProgrammerModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Nu avem nevoie să implementăm această metodă, deoarece evenimentele sunt atașate în CalculatorModeManager
        }
    }
}