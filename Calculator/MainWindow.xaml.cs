using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CalculatorManager _calculatorManager;

        public MainWindow()
        {
            InitializeComponent();

            // Setează fereastra să nu fie redimensionabilă
            ResizeMode = ResizeMode.NoResize;

            // Setează TextBox să nu fie editabil direct
            ResultTextBox.IsReadOnly = true;
            ResultTextBox.Text = "0";

            // Inițializează și configurează managerul calculatorului
            _calculatorManager = new CalculatorManager(this);

            // Atașează evenimentul pentru tastele de la tastatură
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            _calculatorManager.HandleKeyPress(e);
        }
    }
}