using System.Windows;

namespace MCIB
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawWindow window = new DrawWindow();
            window.ShowDialog();
        }
    }
}