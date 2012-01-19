using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NetProcGame;
using NetProcGame.tools;
using NetProcGame.game;

namespace PCSDiagnostics
{
    public enum TriggerType
    {
        Bumper,
        Flipper
    }
    public class Trigger
    {
        public string SwitchName { get; set; }
        public List<string> CoilsTriggered { get; set; }
        public TriggerType Type { get; set; }

        public Trigger()
        {
            this.Type = TriggerType.Flipper;
            CoilsTriggered = new List<string>();
        }
    }

    public class SwitchEntry
    {
        public string Name { get; set; }
        public SwitchType Type { get; set; }
        public string Number { get; set; }
        public SwitchEntry()
        {
            Type = SwitchType.NO;
        }
    }

    public class DriverEntry
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public ushort PulseTime { get; set; }
        public DriverEntry()
        {
            PulseTime = 30;
        }
    }

    public class Configuration
    {
        public MachineType MachineType { get; set; }
        public int TotalBallsInTrough { get; set; }
        public List<Trigger> SwitchTriggers { get; set; }
        public List<SwitchEntry> Switches { get; set; }
        public List<DriverEntry> Lamps { get; set; }
        public List<DriverEntry> Coils { get; set; }
        public List<DriverEntry> Flashers { get; set; }

        public Configuration()
        {
            SwitchTriggers = new List<Trigger>();
            Switches = new List<SwitchEntry>();
            Lamps = new List<DriverEntry>();
            Coils = new List<DriverEntry>();
            Flashers = new List<DriverEntry>();
        }

        public void SaveAsXML(string path_to_file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
            TextWriter textWriter = new StreamWriter(path_to_file, false);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }
    }
}
