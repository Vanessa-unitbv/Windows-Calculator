using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Calculator
{
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
            ResizeMode = ResizeMode.NoResize;
            ResultTextBox.IsReadOnly = true;
            ResultTextBox.Text = "0";
            _memoryManager = new CalculatorMemoryManager(MemoryListBox);
            _calculatorManager = new CalculatorManager(this, _memoryManager);
            _programmerCalculatorManager = new ProgrammerCalculatorManager(this);
            _clipboardManager = new ClipboardManager(ResultTextBox, _calculatorManager);
            _modeManager = new CalculatorModeManager(this, _calculatorManager, _programmerCalculatorManager, _memoryManager);
            KeyDown += MainWindow_KeyDown;
            AttachMenuEvents();
            LoadSettings();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
                    _modeManager.SetCalculatorMode(CalculatorModeManager.CalculatorMode.Programmer);
                else
                    _modeManager.SetCalculatorMode(CalculatorModeManager.CalculatorMode.Standard);

                e.Handled = true;
                return;
            }
            if (e.Key == Key.Escape)
            {
                if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
                    _calculatorManager.Reset();
                else
                    _programmerCalculatorManager.Reset();

                e.Handled = true;
                return;
            }
            if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
            {
                _calculatorManager.HandleKeyPress(e);
            }
            else if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Programmer)
            {
                _programmerCalculatorManager.HandleKeyPress(e);
            }
        }
        private void LoadSettings()
        {
            bool useDigitGrouping = SettingsManager.Instance.UseDigitGrouping;
            bool useOrderOfOperations = SettingsManager.Instance.UseOrderOfOperations;

            if (FindMenuItem("Digit Grouping") is MenuItem digitGroupingMenuItem)
            {
                digitGroupingMenuItem.IsChecked = useDigitGrouping;
            }

            if (FindMenuItem("Order of Operations") is MenuItem orderOfOperationsMenuItem)
            {
                orderOfOperationsMenuItem.IsChecked = useOrderOfOperations;
            }

            _calculatorManager.SetDigitGrouping(useDigitGrouping);
            _calculatorManager.SetUseOrderOfOperations(useOrderOfOperations);
            _programmerCalculatorManager.SetDigitGrouping(useDigitGrouping);
            _modeManager.LoadSettings();
        }

        private void AttachMenuEvents()
        {
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
                digitGroupingMenuItem.IsCheckable = true;
                digitGroupingMenuItem.Click += DigitGroupingMenuItem_Click;
            }

            if (FindMenuItem("Order of Operations") is MenuItem orderOfOperationsMenuItem)
            {
                orderOfOperationsMenuItem.IsCheckable = true;
                orderOfOperationsMenuItem.Click += OrderOfOperationsMenuItem_Click;
            }
        }
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
        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Cut();
        }
        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Copy();
        }
        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _clipboardManager.Paste();
        }
        private void MemoryShow_Click(object sender, RoutedEventArgs e)
        {
            _memoryManager.ToggleMemoryStack();
        }
        private void MemoryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MemoryListBox.SelectedItem != null)
            {
                double selectedValue = _memoryManager.GetSelectedMemoryValue();
                _calculatorManager.SetDisplayValue(selectedValue);
            }
        }
        private void DigitGroupingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                bool isChecked = menuItem.IsChecked;
                if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
                {
                    _calculatorManager.SetDigitGrouping(isChecked);
                }
                else
                {
                    _programmerCalculatorManager.SetDigitGrouping(isChecked);
                }
                SettingsManager.Instance.UseDigitGrouping = isChecked;
            }
        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _calculatorManager.ShowAboutInfo();
        }
        private void StandardModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ProgrammerModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OrderOfOperationsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                bool isChecked = menuItem.IsChecked;

                if (_modeManager.CurrentMode == CalculatorModeManager.CalculatorMode.Standard)
                {
                    _calculatorManager.SetUseOrderOfOperations(isChecked);
                }
                else
                {
                    MessageBox.Show("Ordinea operațiilor este disponibilă doar în modul Standard.",
                                   "Informație", MessageBoxButton.OK, MessageBoxImage.Information);
                    menuItem.IsChecked = false;
                }

                SettingsManager.Instance.UseOrderOfOperations = isChecked;
            }
        }
    }
}