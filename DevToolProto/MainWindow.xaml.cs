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

    struct RoomDescData_t
    {
        string id;
        string altname;
        string roomname;
        string description;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TOTAL_IMAGES = 6;
        private const int BASE_IMAGE_INDEX = 1;
        private const string IMAGE_EXT = ".png";
        private const string IMAGE_PATH = "pack://application:,,,/res/Image";

        // Store struct of data to image index
        private Dictionary<int, List<NodeData_t>> nodeData;
        private Dictionary<int, List<RoomDescData_t>> roomData;

        private List<Grid> grids;
        private List<Button> buttons;
        private List<Label> labels;

        private int currentImage;

        public MainWindow()
        {
            InitializeComponent();
            nodeData = new Dictionary<int, List<NodeData_t>>();
            roomData = new Dictionary<int, List<RoomDescData_t>>();

            grids = new List<Grid>
            {
                (Grid)FindName("USBNodeForm"),
                (Grid)FindName("RoomDescForm")
            };
            buttons = new List<Button>();
            labels = new List<Label>();

            currentImage = BASE_IMAGE_INDEX;

        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        // Menu Button click listeners

        private void NodeBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(0);
        }

        private void RoomDescBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(1);
        }
        private void EditNodeBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(2);
            // Output data list
        }
        private void EditRoomDescBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(3);
            // Output data list
        }
        private void ExportBC(object sender, RoutedEventArgs e)
        {
        }
        private void ChangeImageBC(object sender, RoutedEventArgs e)
        {
            currentImage++;
            if(currentImage > TOTAL_IMAGES)
            {
                currentImage = BASE_IMAGE_INDEX;
            }
            string path = IMAGE_PATH + currentImage + IMAGE_EXT;
            ((Image)FindName("imgFloor")).Source = new BitmapImage(new Uri(path));
        }

        // Submit button data listeners
        private void NewNodeSumbit(object sender, RoutedEventArgs e)
        {
            // Populate NodeData_t with data from text boxes
            NodeData_t data = new NodeData_t();



            // Add data to dictionary
            if(nodeData[currentImage] == null)
            {
                nodeData[currentImage] = new List<NodeData_t>();
            }
            nodeData[currentImage].Add(data);
        }
        private void NewRoomDescSumbit(object sender, RoutedEventArgs e)
        {
            // Populate RoomDescData_t with data from text boxes
            RoomDescData_t data = new RoomDescData_t();


            // Add data to dictionary
            if (roomData[currentImage] == null)
            {
                roomData[currentImage] = new List<RoomDescData_t>();
            }
            roomData[currentImage].Add(data);
        }

        private void ChangeGridVisibility(int index)
        {
            // Ensure the index is in range
            if(index >= grids.Count)
            {
                throw new IndexOutOfRangeException("");
            }

            // Hide all grids
            foreach(Grid g in grids)
            {
                g.Visibility = System.Windows.Visibility.Hidden;
            }

            // Unhide requested grid
            grids[index].Visibility = System.Windows.Visibility.Visible;
        }

    }
}
