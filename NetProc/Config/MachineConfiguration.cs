using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace NetProc
{
    /// <summary>
    /// Represents the switch config entry in memory
    /// </summary>
    public class SwitchConfigFileEntry
    {
        public string Name { get; set; }        
        public SwitchType Type { get; set; }
        public string Number { get; set; }

        public SwitchConfigFileEntry()
        {
            this.Type = SwitchType.NO;
        }
    }

    /// <summary>
    /// Represents a coil config entry in memory
    /// </summary>
    public class CoilConfigFileEntry
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public int PulseTime { get; set; }
        public string Bus { get; set; }
        public bool Polarity { get; set; }
        public CoilConfigFileEntry()
        {
            PulseTime = 30;
        }
    }

    /// <summary>
    /// Represents a lamp's configuration in memory
    /// </summary>
    public class LampConfigFileEntry
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Bus { get; set; }
        public bool Polarity { get; set; }
    }

    /// <summary>
    /// Represents a GI string list element in memory
    /// </summary>
    public class GIConfigFileEntry
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }

    public class PRDriverGlobalsEntry
    {
        public ushort lamp_matrix_strobe_time { get; set; }
        public ushort watchdog_time { get; set; }
        public bool use_watchdog { get; set; }

        public PRDriverGlobalsEntry()
        {
            lamp_matrix_strobe_time = 200;
            watchdog_time = 1000;
            use_watchdog = true;
        }
    }

    public class DriverAliasEntry
    {
        public string expr { get; set; }
        public string repl { get; set; }
    }

    public class BallSaveConfigFileEntry
    {
        public List<string> PulseCoils { get; set; }
        public Dictionary<string, string> ResetSwitches { get; set; }
        public Dictionary<string, string> StopSwitches { get; set; }
        public BallSaveConfigFileEntry()
        {
            PulseCoils = new List<string>();
            ResetSwitches = new Dictionary<string, string>();
            StopSwitches = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Represents the lower-level game configuration in memory
    /// </summary>
    public class GameConfigFileEntry
    {
        public MachineType MachineType { get; set; }
        public int numBalls { get; set; }
        public bool displayMonitor { get; set; }
    }

    /// <summary>
    /// Aggregate class representation of all elements in the configuration file in their deserialized form
    /// </summary>
    public class MachineConfiguration
    {
        public GameConfigFileEntry PRGame { get; set; }
        public List<string> PRFlippers { get; set; } = new List<string>();
        public List<string> PRFlipperLeft { get; set; } = new List<string>();
        public List<string> PRFlipperRight { get; set; } = new List<string>();
        public List<string> PRBumpers { get; set; } = new List<string>();
        public List<SwitchConfigFileEntry> PRSwitches { get; set; } = new List<SwitchConfigFileEntry>();
        public List<CoilConfigFileEntry> PRCoils { get; set; } = new List<CoilConfigFileEntry>();
        public List<LampConfigFileEntry> PRLamps { get; set; } = new List<LampConfigFileEntry>();
        public List<GIConfigFileEntry> PRGI { get; set; } = new List<GIConfigFileEntry>();
        public BallSaveConfigFileEntry PRBallSave { get; set; } = new BallSaveConfigFileEntry();
        public PRDriverGlobalsEntry PRDriverGlobals { get; set; } = new PRDriverGlobalsEntry();
        public List<DriverAliasEntry> PRDriverAliases { get; set; } = new List<DriverAliasEntry>();
        public List<LampConfigFileEntry> PRLeds { get; set; } = new List<LampConfigFileEntry>();

        public MachineConfiguration() { }

        /// <summary>
        /// Add a switch to the configuration
        /// </summary>
        /// <param name="Name">Switch Name</param>
        /// <param name="Number">Pretty unencoded switch number</param>
        /// <param name="Type">NO = Normally Open (leaf), NC = Normally Closed (optos)</param>
        public void AddSwitch(string Name, string Number, SwitchType Type = SwitchType.NO)
        {
            SwitchConfigFileEntry se = new SwitchConfigFileEntry();
            se.Name = Name;
            se.Type = Type;
            se.Number = Number;
            PRSwitches.Add(se);
        }

        /// <summary>
        /// Add a coil to the configuration
        /// </summary>
        /// <param name="Name">Coil name</param>
        /// <param name="Number">Pretty undecoded coil number</param>
        /// <param name="pulseTime">Default pulse time of this coil (30ms by default)</param>
        public void AddCoil(string Name, string Number, int pulseTime = 30)
        {
            CoilConfigFileEntry ce = new CoilConfigFileEntry();
            ce.Name = Name;
            ce.Number = Number;
            ce.PulseTime = pulseTime;
            PRCoils.Add(ce);
        }

        /// <summary>
        /// Add a lamp to the configuration
        /// </summary>
        /// <param name="Name">Lamp Name</param>
        /// <param name="Number">Pretty undecoded lamp number</param>
        public void AddLamp(string Name, string Number)
        {
            LampConfigFileEntry le = new LampConfigFileEntry();
            le.Name = Name;
            le.Number = Number;
            PRLamps.Add(le);
        }

        /// <summary>
        /// Add an led to the configuration
        /// </summary>
        /// <param name="Name">Lamp Name</param>
        /// <param name="Number">Pretty undecoded lamp number</param>
        public void AddLED(string Name, string Number)
        {
            LampConfigFileEntry le = new LampConfigFileEntry();
            le.Name = Name;
            le.Number = Number;
            PRLeds.Add(le);
        }

        /// <summary>
        /// Add a GI strand to the configuration
        /// </summary>
        /// <param name="Name">GI strand name</param>
        /// <param name="Number">Pretty undecoded strand number</param>
        public void AddGI(string Name, string Number)
        {
            GIConfigFileEntry ge = new GIConfigFileEntry();
            ge.Name = Name;
            ge.Number = Number;
            PRGI.Add(ge);
        }

        /// <summary>
        /// Convert the entire MachineConfiguration to JSON code
        /// </summary>
        /// <returns>Pretty formatted JSON code</returns>
        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Convert the entire MachineConfiguration to XML code and save to a file
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void SaveAsXML(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MachineConfiguration));
            TextWriter textWriter = new StreamWriter(filename, false);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        /// <summary>
        /// Initialize configuration from a string of JSON code
        /// </summary>
        /// <param name="JSON">JSON serialized MachineConfiguration data</param>
        /// <returns>A deserialized MachineConfiguration object</returns>
        public static MachineConfiguration FromJSON(string JSON)
        {
            return JsonSerializer.Deserialize<MachineConfiguration>(JSON, new JsonSerializerOptions() 
            { WriteIndented=true, ReadCommentHandling = JsonCommentHandling.Skip , AllowTrailingCommas=true});
        }

        /// <summary>
        /// Initialize configuration from a JSON file on disk
        /// </summary>
        /// <param name="PathToFile">The file to deserialize</param>
        /// <returns>A MachineConfiguration object deserialized from the specified JSON file</returns>
        public static MachineConfiguration FromFile(string PathToFile)
        {
            StreamReader streamReader = new StreamReader(PathToFile);
            string text = streamReader.ReadToEnd();
            return FromJSON(text);
        }
    }
}
