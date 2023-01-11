using NetProc;

namespace NetProc.Data.Model
{
    public class Machine : GameConfigFileEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
    }
}
