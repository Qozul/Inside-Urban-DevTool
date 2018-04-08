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
        public string id;
        public string rdid;
        public string position;
        public string connecting;
        public string level;
        public string isAccessible;
    }

    struct RoomDescData_t
    {
        public string id;
        public string altname;
        public string roomname;
        public string description;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Image constants
        private const int TOTAL_IMAGES = 6;
        private const int BASE_IMAGE_INDEX = 1;
        private const string IMAGE_EXT = ".png";
        private const string IMAGE_PATH = "pack://application:,,,/res/Image";

        // XAML elements
        private readonly string[] GRID_NAMES = { "USBNodeForm", "RoomDescForm", "EditUSBNodeForm", "EditRoomDescForm", "lstbNodesForm" };
        private List<Grid> grids;

        // Store struct of data to image index
        private Dictionary<int, List<NodeData_t>> nodeData;
        private List<RoomDescData_t> roomData;

        // IDs
        private int nextNodeID;
        private int nextRDID;

        // Images
        private ListBox listOutput;

        // Track index of currently displayed image
        private int currentImage;

        public MainWindow()
        {
            InitializeComponent();
            // Initialise
            nodeData = new Dictionary<int, List<NodeData_t>>();
            roomData = new List<RoomDescData_t>();

            grids = new List<Grid>();
            foreach (string name in GRID_NAMES)
            {
                grids.Add((Grid)FindName(name));
            }

            currentImage = BASE_IMAGE_INDEX;
            nextNodeID = 0;
            nextRDID = 0;

            listOutput = (ListBox)FindName("lstbSelectNode");
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        // Node button click displays the form for making a new node
        private void NodeBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(0);
        }

        // RooomDesc button click displays the form for making a new RoomDesc
        private void RoomDescBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(1);
        }

        // Opens a list of already made nodes
        private void EditNodeBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(4);
            // Output data list
            listOutput.Items.Clear();
            if(nodeData.TryGetValue(currentImage, out List<NodeData_t> currentNodes))
            {
                foreach (NodeData_t ndt in currentNodes)
                {

                    listOutput.Items.Add(new ListBoxItem
                    {
                        Content = "ID: " + ndt.id + " | Pos: " + ndt.position + " | Cons: " + ndt.connecting + " | Level: " + ndt.level + " | Access: " + ndt.isAccessible
                    });
                }
            }
        }

        // Opens a list of already made RoomDescs
        private void EditRoomDescBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(4);
            // Output data list
            listOutput.Items.Clear();
            foreach (RoomDescData_t rdt in roomData)
            {
                listOutput.Items.Add(new ListBoxItem
                {
                    Content = "ID: " + rdt.id + " Name: " + rdt.roomname + " | Altname: " + rdt.altname
                });
            }
        }

        // Exports the data to xml
        private void ExportBC(object sender, RoutedEventArgs e)
        {
        }

        // Changes the image being displayed to the next one (index increment)
        private void ChangeImageBC(object sender, RoutedEventArgs e)
        {
            currentImage++;
            if (currentImage > TOTAL_IMAGES)
            {
                currentImage = BASE_IMAGE_INDEX;
            }
            string path = IMAGE_PATH + currentImage + IMAGE_EXT;
            ((Image)FindName("imgFloor")).Source = new BitmapImage(new Uri(path));
        }

        // Submit button data listeners
        private void NewNodeSubmit(object sender, RoutedEventArgs e)
        {
            // Get RDID data and validate
            string inRdid = ((TextBox)FindName("txbRDID_Value")).Text;
            string inPosition = ((TextBox)FindName("txbPosition_Value")).Text;
            string inConnecting = ((TextBox)FindName("txbConnecting_Value")).Text;
            string inLevel = ((TextBox)FindName("txbLevel")).Text;
            bool inIsAccessible = ((RadioButton)FindName("rdbAccessible_True")).IsEnabled; // If true then it is accessible, if not then false

            if(!ValidateNodeData(inRdid, inPosition, inConnecting, inLevel))
            {
                return;
            }


            // Populate NodeData_t with data from text boxes
            NodeData_t data = new NodeData_t
            {
                id = nextNodeID.ToString(),
                rdid = inRdid,
                position = inPosition,
                connecting = inConnecting,
                level = inLevel,
                isAccessible = inIsAccessible.ToString()
            };

            // Add data to dictionary
            if (!nodeData.ContainsKey(currentImage))
            {
                nodeData[Int32.Parse(inLevel)] = new List<NodeData_t>();
            }
            nodeData[Int32.Parse(inLevel)].Add(data);

            MessageBox.Show("New node has been created with id " + nextNodeID + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            nextNodeID++;
        }

        private void NewRoomDescSubmit(object sender, RoutedEventArgs e)
        {
            string inAltname = ((TextBox)FindName("txbAltName_Value")).Text;
            string inRoomname = ((TextBox)FindName("txbRoomName_Value")).Text;
            string inDesc = ((TextBox)FindName("txbDescription_Value")).Text;

            if(!ValidateRDData(inAltname, inRoomname, inDesc))
            {
                return;
            }

            // Populate RoomDescData_t with data from text boxes
            RoomDescData_t data = new RoomDescData_t
            {
                id = nextRDID.ToString(),
                altname = inAltname,
                roomname = inRoomname,
                description = inDesc
            };

            // Add data to list
            roomData.Add(data);

            MessageBox.Show("New room desc has been created with id " + nextRDID + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            nextRDID++;
        }

        private bool ValidateNodeData(string inRdid, string inPosition, string inConnecting, string inLevel)
        {
            bool isRdidValid = true;
            if (Int32.TryParse(inRdid, out int rdidResult))
            {
                isRdidValid = rdidResult < 0 ? false : true;
            }
            else
            {
                isRdidValid = false;
            }
            if (!isRdidValid)
            {
                // Invalid
                MessageBox.Show("RDID must be a number and greater than or equal to 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Get position data and validate
            string[] posSplit = inPosition.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (posSplit.Length != 2)
            {
                MessageBox.Show("Incorrect number of coordinates, should be two in format (x,y).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            foreach (string pos in posSplit)
            {
                if (!Int32.TryParse(pos, out int posResult))
                {
                    MessageBox.Show("Position x and y must be a number, use format (x,y).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            // Get connecting data and validate
            string[] icSplit = inConnecting.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (icSplit.Length <= 0)
            {
                var warnResult = MessageBox.Show("This node does not have any connections. Continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (warnResult == MessageBoxResult.No)
                {
                    return false;
                }
            }
            foreach (string ic in icSplit)
            {
                bool isIcValid = true;
                if (Int32.TryParse(ic, out int icResult))
                {
                    isIcValid = icResult < 0 ? false : true;
                }
                else
                {
                    isIcValid = false;
                }
                if (!isIcValid)
                {
                    MessageBox.Show("Each connection must be an ID of a node, which is a number greater than or equal to 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            int levelResult = Int32.MinValue;
            Int32.TryParse(inLevel, out levelResult);
            if (levelResult < BASE_IMAGE_INDEX || levelResult > TOTAL_IMAGES)
            {
                MessageBox.Show("Level must be a number between " + BASE_IMAGE_INDEX + " and " + TOTAL_IMAGES + " inclusively.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool ValidateRDData(string inAltname, string inRoomname, string inDesc)
        {
            string warnText = "";
            if (inAltname.Length <= 0)
            {
                warnText += "This room desc does not have any altname. Continue?\n";
            }

            if (inRoomname.Length <= 0)
            {
                warnText += "This room desc does not have any roomname. Continue?\n";
            }

            if (inDesc.Length <= 0)
            {
                warnText += "This room desc does not have any description. Continue?\n";
            }
            if (!warnText.Equals(""))
            {
                var warnResult = MessageBox.Show(warnText, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (warnResult == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private void ChangeGridVisibility(int index)
        {
            // Ensure the index is in range
            if (index >= grids.Count)
            {
                throw new IndexOutOfRangeException("");
            }

            // Hide all grids
            foreach (Grid g in grids)
            {
                g.Visibility = System.Windows.Visibility.Hidden;
            }

            // Unhide requested grid
            grids[index].Visibility = System.Windows.Visibility.Visible;
        }

    }
}
