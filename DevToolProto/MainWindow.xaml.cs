using DevToolProto.data;
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
using System.Xml;

namespace DevToolProto
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Image constants
        private const int IMAGE_DIMENS = 13;
        private const int TOTAL_IMAGES = 6;
        private const int BASE_IMAGE_INDEX = 1;
        private const string IMAGE_EXT = ".png";
        private const string CANVAS_IMAGE_PATH = "pack://application:,,,/res/";
        private const string IMAGE_PATH = CANVAS_IMAGE_PATH + "sImage";

        // XAML elements
        private readonly string[] GRID_NAMES = { "USBNodeForm", "RoomDescForm", "EditUSBNodeForm", "EditRoomDescForm", "lstbNodesForm" };
        private List<Grid> grids;

        // Store struct of data to image index
        private Dictionary<int, List<NodeData>> nodeData;
        private List<RoomData> roomData;

        // IDs
        private int nextNodeID;
        private int nextRDID;

        // Images
        private ListBox listOutput;

        // Track index of currently displayed image
        private int currentImage;

        // Track if edit button was pressed for node or for rd
        private bool? isEditNode;
        private NodeData currentEditNode;
        private RoomData currentEditRoomDesc;

        private int currentVisibleIdx;

        private TextBlock customConsole;
        
        private Canvas canvas;

        public MainWindow()
        {
            InitializeComponent();
            // Initialise
            nodeData = new Dictionary<int, List<NodeData>>();
            roomData = new List<RoomData>();

            grids = new List<Grid>();
            foreach (string name in GRID_NAMES)
            {
                grids.Add((Grid)FindName(name));
            }

            currentImage = BASE_IMAGE_INDEX;
            nextNodeID = 0;
            nextRDID = 0;
            currentVisibleIdx = -1;

            listOutput = (ListBox)FindName("lstbSelectNode");

            isEditNode = null;

            customConsole = (TextBlock)FindName("outConsole");
            customConsole.Text += "Dev Tool Successfully initialised.\n";

            canvas = (Canvas)FindName("canvasView");

            // Import data on start
            ImportData();
        }

        private void ImportData()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("./book.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/data/nodes/node");
            foreach (XmlNode node in nodeList)
            {
                string[] nData = new string[6];
                int nPtr = 0;
                foreach (XmlNode nChild in node)
                {
                    if (nChild.NodeType != XmlNodeType.Comment)
                    {
                        nData[nPtr++] = nChild.Attributes["value"].Value;
                    }
                }
                int lvl = Int32.Parse(nData[4]);
                if (!nodeData.ContainsKey(lvl))
                {
                    nodeData[lvl] = new List<NodeData>();
                }
                if(nData[5] == "")
                {
                    nData[5] = "True";
                }
                nodeData[lvl].Add(new NodeData(nData[0], nData[1], nData[2], nData[3], nData[4], nData[5], GenNodeImage(nData[2])));
            }
            XmlNodeList roomList = xmlDoc.DocumentElement.SelectNodes("/data/descs/desc");
            foreach (XmlNode desc in roomList)
            {
                string[] rData = new string[4];
                int rPtr = 0;
                foreach(XmlNode rChild in desc)
                {
                    if (rChild.NodeType != XmlNodeType.Comment)
                    {
                        rData[rPtr++] = rChild.Attributes["value"].Value;
                    }
                }
                roomData.Add(new RoomData(rData[0], rData[1], rData[2], rData[3]));
            }
            currentImage--;
            ChangeImageBC(null, null);
        }

        private Image GenNodeImage(string position)
        {
            Image img = new Image()
            {
                Source = new BitmapImage(new Uri(CANVAS_IMAGE_PATH + "blackcirc" + IMAGE_EXT))
            };
            String[] parsePositions = position.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            canvas.Children.Add(img);
            // Safe to parse here because already checked in validation
            Canvas.SetLeft(img, Int32.Parse(parsePositions[0]));
            Canvas.SetTop(img, Int32.Parse(parsePositions[1]));
            img.MouseUp += new MouseButtonEventHandler(ImageMouseUp);
            return img;
        }

        private NodeData GetNodeByPosition(string pos)
        {
            foreach(NodeData nData in nodeData[currentImage])
            {
                if(nData.Position == pos)
                {
                    return nData;
                }
            }
            return null;
        }

        private void ImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            List<string> positionPerms = new List<string>();
            for (int i = -(IMAGE_DIMENS / 2); i < IMAGE_DIMENS / 2; i++)
            {
                for (int j = -(IMAGE_DIMENS / 2); j < IMAGE_DIMENS / 2; j++)
                {
                    positionPerms.Add((e.GetPosition(this).X - 300 + i) + ", " + (e.GetPosition(this).Y - 1 + j));
                }
            }
            foreach (string perm in positionPerms)
            {
                NodeData data = GetNodeByPosition(perm);
                if (data != null)
                {
                    // Found a match
                    customConsole.Text += "Pressed node with ID " + data.Id + "\n";
                    break;
                }
            }
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(grids[2].Visibility == Visibility.Visible)
            {
                ((TextBox)FindName("txbEditPosition_Value")).Text = (e.GetPosition(this).X - 300) + ", " + (e.GetPosition(this).Y - 1);
            }
            else
            {
                ((TextBox)FindName("txbPosition_Value")).Text = (e.GetPosition(this).X - 300) + ", " + (e.GetPosition(this).Y - 1);
            }
        }

        private void UpdateNodeVisibility()
        {
            foreach(int key in nodeData.Keys)
            {
                Visibility flag = key == currentImage ? Visibility.Visible : Visibility.Hidden;
                foreach(NodeData nData in nodeData[key])
                {
                    nData.Img.Visibility = flag;
                }
            }
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
            isEditNode = true;
            // Output data list
            listOutput.Items.Clear();
            if(nodeData.TryGetValue(currentImage, out List<NodeData> currentNodes))
            {
                foreach (NodeData ndt in currentNodes)
                {
                    listOutput.Items.Add(new ListBoxItem()
                    {
                        Name = "n" + ndt.Id,
                        Content = "ID: " + ndt.Id + " | Pos: " + ndt.Position + " | Cons: " + ndt.Connecting + " | Level: " + ndt.Level + " | Access: " + ndt.IsAccessible
                    });
                }
            }
        }

        // Opens a list of already made RoomDescs
        private void EditRoomDescBC(object sender, RoutedEventArgs e)
        {
            ChangeGridVisibility(4);
            isEditNode = false;
            // Output data list
            listOutput.Items.Clear();
            foreach (RoomData rdt in roomData)
            {
                listOutput.Items.Add(new ListBoxItem
                {
                    Name = "r" + rdt.Id,
                    Content = "ID: " + rdt.Id + " Name: " + rdt.Roomname + " | Altname: " + rdt.Altname
                });
            }
        }

        private void SelectEditBC(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbt = (ListBoxItem)listOutput.SelectedItem;
            if(lbt == null)
            {
                return;
            }
            if(isEditNode != null && isEditNode == true)
            {
                foreach (NodeData nData in nodeData[currentImage])
                {
                    if (nData.Id == lbt.Name.Replace("n", ""))
                    {
                        // Found
                        ChangeGridVisibility(2);
                        ((TextBox)FindName("txbEditID_Value")).Text = nData.Id;
                        ((TextBox)FindName("txbEditRDID_Value")).Text = nData.Rdid;
                        ((TextBox)FindName("txbEditPosition_Value")).Text = nData.Position;
                        ((TextBox)FindName("txbEditConnecting_Value")).Text = nData.Connecting;
                        ((TextBox)FindName("txbEditLevel")).Text = nData.Level;
                        ((RadioButton)FindName("rdbEditAccessible_True")).IsChecked = Boolean.Parse(nData.IsAccessible);
                        currentEditNode = nData;
                    }
                }
            }
            else
            {
                foreach (RoomData rData in roomData)
                {
                    if (rData.Id == lbt.Name.Replace("r", ""))
                    {
                        // Found
                        ChangeGridVisibility(3);
                        ((TextBox)FindName("txbEditRoomDesc_Value")).Text = rData.Id;
                        ((TextBox)FindName("txbEditAltName_Value")).Text = rData.Altname;
                        ((TextBox)FindName("txbEditRoomName_Value")).Text = rData.Roomname;
                        ((TextBox)FindName("txbEditDescription_Value")).Text = rData.Description;
                        currentEditRoomDesc = rData;
                    }
                }
            }
        }

        // Exports the data to xml
        private void ExportBC(object sender, RoutedEventArgs e)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };
            using (XmlWriter writer = XmlWriter.Create("book.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("data");

                writer.WriteStartElement("nodes");
                foreach (int key in nodeData.Keys)
                {
                    foreach (NodeData nData in nodeData[key])
                    {
                        writer.WriteStartElement("node");

                        writer.WriteStartElement("id");
                        writer.WriteAttributeString("value", nData.Id);
                        writer.WriteEndElement();

                        writer.WriteStartElement("rdid");
                        writer.WriteAttributeString("value", nData.Rdid);
                        writer.WriteEndElement();

                        writer.WriteStartElement("position");
                        writer.WriteAttributeString("value", nData.Position);
                        writer.WriteEndElement();

                        writer.WriteStartElement("connecting");
                        writer.WriteAttributeString("value", nData.Connecting);
                        writer.WriteEndElement();

                        writer.WriteStartElement("level");
                        writer.WriteAttributeString("value", nData.Level);
                        writer.WriteEndElement();

                        writer.WriteStartElement("isAccessible");
                        writer.WriteAttributeString("value", nData.IsAccessible);
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.WriteStartElement("descs");
                foreach (RoomData rData in roomData)
                {
                    writer.WriteStartElement("desc");
                    writer.WriteStartElement("id");
                    writer.WriteAttributeString("value", rData.Id);
                    writer.WriteEndElement();
                    writer.WriteStartElement("altname");
                    writer.WriteAttributeString("value", rData.Altname);
                    writer.WriteEndElement();
                    writer.WriteStartElement("roomname");
                    writer.WriteAttributeString("value", rData.Roomname);
                    writer.WriteEndElement();
                    writer.WriteStartElement("description");
                    writer.WriteAttributeString("value", rData.Description);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            customConsole.Text += "Success: Data has been exported to book.xml.\n";
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
            UpdateNodeVisibility();
            if (((Grid)FindName(GRID_NAMES[4])).Visibility == Visibility.Visible)
            {
                if(isEditNode == true)
                {
                    EditNodeBC(sender, e);
                }
                else
                {
                    EditRoomDescBC(sender, e);
                }
            }
            else
            {
                ChangeGridVisibility(currentVisibleIdx);
            }
        }

        // Submit button data listeners
        private void NewNodeSubmit(object sender, RoutedEventArgs e)
        {
            // Get RDID data and validate
            string inRdid = ((TextBox)FindName("txbRDID_Value")).Text;
            string inPosition = ((TextBox)FindName("txbPosition_Value")).Text;
            string inConnecting = ((TextBox)FindName("txbConnecting_Value")).Text;
            string inLevel = ((TextBox)FindName("txbLevel")).Text;
            bool? inIsAccessible = ((RadioButton)FindName("rdbAccessible_True")).IsChecked; // If true then it is accessible, if not then false
            if(inIsAccessible == null)
            {
                customConsole.Text += "Error: inIsAccessible is null. Aborting.\n";
                return;
            }

            if(!ValidateNodeData(inRdid, inPosition, inConnecting, inLevel))
            {
                return;
            }

            // Populate NodeData with data from text boxes
            NodeData data = new NodeData(nextNodeID.ToString(), inRdid, inPosition, inConnecting, inLevel, inIsAccessible.ToString(), GenNodeImage(inPosition));

            int lvl = Int32.Parse(inLevel);
            // Add data to dictionary
            if (!nodeData.ContainsKey(lvl))
            {
                nodeData[lvl] = new List<NodeData>();
            }
            nodeData[lvl].Add(data);

            customConsole.Text += "Success: New node has been created with id " + nextNodeID.ToString() + ".\n";
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
            // Add data to list
            roomData.Add(new RoomData(nextRDID.ToString(), inAltname, inRoomname, inDesc));

            customConsole.Text += "Success: New room desc has been created with id " + nextRDID.ToString() + ".\n";
            nextRDID++;
        }

        private void EditNodeSubmit(object sender, RoutedEventArgs e)
        {
            string inId = ((TextBox)FindName("txbEditID_Value")).Text;
            string inRdid = ((TextBox)FindName("txbEditRDID_Value")).Text;
            string inPosition = ((TextBox)FindName("txbEditPosition_Value")).Text;
            string inConnecting = ((TextBox)FindName("txbEditConnecting_Value")).Text;
            string inLevel = ((TextBox)FindName("txbEditLevel")).Text;
            bool? inIsAccessible = ((RadioButton)FindName("rdbEditAccessible_True")).IsChecked; // If true then it is accessible, if not then false
            if (inIsAccessible == null)
            {
                customConsole.Text += "Error: inIsAccessible is null. Aborting.\n";
                return;
            }

            if (!ValidateNodeData(inRdid, inPosition, inConnecting, inLevel))
            {
                return;
            }

            // If level was changed then node needs to be reinserted into the dictionary
            if(currentEditNode.Level != inLevel)
            {
                // Rm
                nodeData[Int32.Parse(currentEditNode.Level)].Remove(currentEditNode);

                // Add
                int lvl = Int32.Parse(inLevel);
                if (!nodeData.ContainsKey(lvl))
                {
                    nodeData[lvl] = new List<NodeData>();
                }
                nodeData[lvl].Add(currentEditNode);
            }

            Image img = currentEditNode.Img;
            String[] parsePositions = inPosition.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            // Safe to parse here because already checked in validation
            Canvas.SetLeft(img, Int32.Parse(parsePositions[0]));
            Canvas.SetTop(img, Int32.Parse(parsePositions[1]));

            currentEditNode.Id = inId;
            currentEditNode.Rdid = inRdid;
            currentEditNode.Position = inPosition;
            currentEditNode.Connecting = inConnecting;
            currentEditNode.Level = inLevel;
            currentEditNode.IsAccessible = inIsAccessible.ToString();

            UpdateNodeVisibility();
            customConsole.Text += "Success: Edited Node with id " + currentEditNode.Id + ".\n";
            EditNodeBC(sender, e);
        }

        private void EditRoomDescSubmit(object sender, RoutedEventArgs e)
        {
            string inAltname = ((TextBox)FindName("txbEditAltName_Value")).Text;
            string inRoomname = ((TextBox)FindName("txbEditRoomName_Value")).Text;
            string inDesc = ((TextBox)FindName("txbEditDescription_Value")).Text;

            if (!ValidateRDData(inAltname, inRoomname, inDesc))
            {
                return;
            }

            currentEditRoomDesc.Altname = inAltname;
            currentEditRoomDesc.Roomname = inRoomname;
            currentEditRoomDesc.Description = inDesc;

            customConsole.Text += "Success: Edited Room Desc with id " + currentEditRoomDesc.Id + ".\n";
            EditRoomDescBC(sender, e);

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
                customConsole.Text += "Error: RDID must be a number and greater than or equal to 0. Aborting.\n";
                return false;
            }

            // Get position data and validate
            string[] posSplit = inPosition.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (posSplit.Length != 2)
            {
                customConsole.Text += "Error: Incorrect number of coordinates, should be two in format (x,y). Aborting.\n";
                return false;
            }
            foreach (string pos in posSplit)
            {
                if (!Int32.TryParse(pos, out int posResult))
                {
                    customConsole.Text += "Error: Position x and y must be a number, use format (x,y). Aborting.\n";
                    return false;
                }
            }

            // Get connecting data and validate
            string[] icSplit = inConnecting.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (icSplit.Length <= 0)
            {
                customConsole.Text += "Warning: This node does not have any connections!\n";
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
                    customConsole.Text += "Error: Each connection must be an ID of a node, which is a number greater than or equal to 0. Aborting.\n";
                    return false;
                }
            }
            
            if(Int32.TryParse(inLevel, out int levelResult))
            {
                if (levelResult < BASE_IMAGE_INDEX || levelResult > TOTAL_IMAGES)
                {
                    customConsole.Text += "Error: Level must be a number between " + BASE_IMAGE_INDEX + " and " + TOTAL_IMAGES + " inclusively. Aborting.\n";
                    return false;
                }
            }
            else
            {
                customConsole.Text += "Error: Level must be a number between " + BASE_IMAGE_INDEX + " and " + TOTAL_IMAGES + " inclusively. Aborting.\n";
                return false;
            }
            return true;
        }

        private bool ValidateRDData(string inAltname, string inRoomname, string inDesc)
        {
            if (inAltname.Length <= 0)
            {
                customConsole.Text += "Warning: This room desc does not have any altname!\n";
            }

            if (inRoomname.Length <= 0)
            {
                customConsole.Text += "Warning: This room desc does not have any roomname!\n";
            }

            if (inDesc.Length <= 0)
            {
                customConsole.Text += "Warning: This room desc does not have any description!\n";
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
            if(index < 0)
            {
                // Changed image, just ignore visiblity change
                return;
            }

            // Hide all grids
            foreach (Grid g in grids)
            {
                g.Visibility = Visibility.Hidden;
            }

            // Unhide requested grid
            grids[index].Visibility = Visibility.Visible;
        }

    }
}
