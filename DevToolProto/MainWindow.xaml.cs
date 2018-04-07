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
    struct NodeData_t
    {
        string id;
        string rdid;
        string position;
        string connecting;
        string level;
        string isAccessible;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Dictionary<int, NodeData_t> x;
        private Grid usbNodeGrid;
        private Grid roomDescGrid;

        private List<Grid> grids;
        private List<Button> menuButtons;

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

        private void NodeBC(object sender, RoutedEventArgs e)
        {
            roomDescGrid.Visibility = System.Windows.Visibility.Hidden;
            usbNodeGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void RoomDescBC(object sender, RoutedEventArgs e)
        {
            usbNodeGrid.Visibility = System.Windows.Visibility.Hidden;
            roomDescGrid.Visibility = System.Windows.Visibility.Visible;
        }
        private void EditNodeBC(object sender, RoutedEventArgs e)
        {
            usbNodeGrid.Visibility = System.Windows.Visibility.Hidden;
            roomDescGrid.Visibility = System.Windows.Visibility.Visible;
        }
        private void EditRoomDescBC(object sender, RoutedEventArgs e)
        {
            usbNodeGrid.Visibility = System.Windows.Visibility.Hidden;
            roomDescGrid.Visibility = System.Windows.Visibility.Visible;
        }
        private void ExportBC(object sender, RoutedEventArgs e)
        {
        }
        private void ChangeImageBC(object sender, RoutedEventArgs e)
        {
        }

        private void ChangeGridVisibility()
        {

        }

    }
}
