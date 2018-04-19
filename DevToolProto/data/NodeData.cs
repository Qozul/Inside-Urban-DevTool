
using System.Windows.Controls;

namespace DevToolProto.data
{
    class NodeData
    {
        public string Id { get; set; }
        public string Rdid { get; set; }
        public string Position { get; set; }
        public string Connecting { get; set; }
        public string Level { get; set; }
        public string IsAccessible { get; set; }
        public Image Img { get; set; }

        public NodeData(string id, string rdid, string pos, string con, string lev, string access, Image img)
        {
            Id = id;
            Rdid = rdid;
            Position = pos;
            Connecting = con;
            Level = lev;
            IsAccessible = access;
            Img = img;
        }

        override public string ToString()
        {
            return "NodeData: " + $"{Id},{Rdid},{Position},{Connecting},{Level},{IsAccessible}";
        }
    }
}
