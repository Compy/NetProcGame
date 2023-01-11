using System;

namespace NetProc.Data.Model
{
    public class Setting
    {
        public string Id { get; set; }        
        public string Value { get; set; }
        public SettingType Type { get; set; }
        /// <summary>
        /// should be short named, keyed if using for multi languages
        /// </summary>
        public string Info { get; set; }
        public string Parent { get; set; }
        public string Options { get; set; }
    }

    [Flags]
    public enum SettingType
    {
        Game = 0,
        System = 1 << 0, //4
        Pricing = 1 << 1,//8
        Scores = 1 << 2//16
    }
}
