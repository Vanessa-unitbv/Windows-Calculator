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
        private CalculatorMemoryManager _memoryManager;

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

            // Inițializează și configurează managerul calculatorului
            _calculatorManager = new CalculatorManager(this, _memoryManager);

            // Atașează evenimentul pentru tastele de la tastatură
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            _calculatorManager.HandleKeyPress(e);
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
    }
}