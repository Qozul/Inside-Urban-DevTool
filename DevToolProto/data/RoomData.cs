
namespace DevToolProto.data
{
    class RoomData
    {
        public string Id { get; set; }
        public string Altname { get; set; }
        public string Roomname { get; set; }
        public string Description { get; set; }

        public RoomData(string id, string alt, string room, string desc)
        {
            Id = id;
            Altname = alt;
            Roomname = room;
            Description = desc;
        }

        override public string ToString()
        {
            return "RoomData: " + $"{Id},{Altname},{Roomname},{Description}";
        }
    }
}
