using NetProcGame.Config;

namespace NetProcGame.Data.Model
{
    public class PRCoil : CoilConfigFileEntry
    {
        public string Id { get; set; }
    }

    public class PRLamp : LampConfigFileEntry
    {
        public string Id { get; set; }
    }

    public class PRSwitch : SwitchConfigFileEntry
    {
        public string Id { get; set; }        
    }

    public enum PRItemType
    {
        Bumper,
        Coil,        
        Flipper,        
        Lamp,
        Switch,        
    }
}
