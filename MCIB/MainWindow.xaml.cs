using HandyControl.Controls;
using MCIB.Libs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = HandyControl.Controls.MessageBox;

namespace MCIB
{
    public partial class MainWindow : BlurWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ckBox.IsChecked = !HookManager.Instance.GetTaskManagerStatus();
                DragMove();
            }
            catch { }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FillBlackWindow();
            DrawWindow window = new DrawWindow();
            window.ShowDialog();
        }
        private void FillBlackWindow()
        {
            try
            {
                var sl = Screen.AllScreens.Length;
                if (sl > 1)
                {
                    for (var idx = 1; idx < sl; idx++)
                    {
                        BlankWindow blank = new BlankWindow();
                        var screen = Screen.AllScreens.ElementAt(idx);
                        blank.Show(screen);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("FillBlackWindow", ex);
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!HookManager.Instance.DisableTaskManager())
                MessageBox.Show("禁用快捷启动任务管理器失败！");
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!HookManager.Instance.EnableTaskManager())
                MessageBox.Show("启用快捷启动任务管理器失败！");
        }
    }
}