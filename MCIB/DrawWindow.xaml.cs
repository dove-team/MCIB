using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MCIB
{
    public partial class DrawWindow : Window
    {
        private int width = 0;
        private List<Brush> Brushes { get; }
        public DrawWindow()
        {
            InitializeComponent();
            Brushes = new List<Brush>
            {
               new SolidColorBrush(Colors.Black),
               new SolidColorBrush(Colors.Green),
               new SolidColorBrush(Colors.White),
               new SolidColorBrush(Colors.Purple),
               new SolidColorBrush(Colors.Yellow),
               new SolidColorBrush(Colors.Orange),
               new SolidColorBrush(Colors.DarkRed),
               new SolidColorBrush(Colors.DarkCyan),
               new SolidColorBrush(Colors.YellowGreen)
            };
            Mouse.OverrideCursor = Cursors.None;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HookManager.Instance.Hook();
            var maxWidth = Convert.ToInt32(Math.Ceiling(ActualWidth));
            Random randomSize = new Random(1),
            randomColor = new Random(1);
            while (width < maxWidth)
            {
                var w = randomSize.Next(20);
                width += w;
                Canvas canvas = new Canvas
                {
                    Width = w,
                    Height = ActualHeight,
                    Background = Brushes.ElementAt(randomColor.Next(Brushes.Count))
                };
                Canvas.SetLeft(canvas, width);
                panel.Children.Add(canvas);
            }
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Alt) && e.Key.HasFlag(Key.H))
            {
                HookManager.Instance.Release();
                Close();
            }
            e.Handled = true;
        }
    }
}