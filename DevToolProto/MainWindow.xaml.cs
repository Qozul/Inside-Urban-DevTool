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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevToolProto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Grid usbNodeGrid;
        private Grid roomDescGrid;

        public MainWindow()
        {
            InitializeComponent();
            usbNodeGrid = (Grid)FindName("USBNodeForm");
            roomDescGrid = (Grid)FindName("RoomDescForm");
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private void NodeButtonClick(object sender, RoutedEventArgs e)
        {
            roomDescGrid.Visibility = System.Windows.Visibility.Hidden;
            usbNodeGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void RoomDescButtonClick(object sender, RoutedEventArgs e)
        {
            usbNodeGrid.Visibility = System.Windows.Visibility.Hidden;
            roomDescGrid.Visibility = System.Windows.Visibility.Visible;
        }

    }
}
