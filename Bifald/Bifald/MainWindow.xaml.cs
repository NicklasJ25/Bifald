using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bifald
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            TextBlock testBlock = listBox.SelectedItem as TextBlock;
            frame.Navigate(new Uri(testBlock.Tag.ToString(), UriKind.Relative));

            MenuToggleButton.IsChecked = false;
        }
    }
}
