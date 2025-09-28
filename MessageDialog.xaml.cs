using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PeriodicTable
{
    /// <summary>
    /// Interaction logic for MassageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public MessageDialog(String message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        // Enable dragging when clicking the title bar
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void CloseHandler(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the custom message box when button is clicked
        }
    }
}
