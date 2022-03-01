using System.Windows;
using System.Windows.Forms;

namespace MCIB
{
    public partial class BlankWindow : Window
    {
        public BlankWindow()
        {
            InitializeComponent();
        }
        public void Show(Screen screen)
        {
            Top = 0;
            Left = screen.WorkingArea.Left;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Show();
        }
    }
}