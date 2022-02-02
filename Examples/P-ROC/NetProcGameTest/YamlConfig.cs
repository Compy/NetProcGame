using System;
using System.Collections.Generic;
using System.Yaml.Serialization;

namespace NetProcGameTest
{
    public class YamlConfig
    {
        public Dictionary<string, string> PRGame = new Dictionary<string, string>();
        public List<string> PRFlippers = new List<string>();
        public List<string> PRBumpers = new List<string>();
        public Dictionary<string, Dictionary<string, string>> PRSwitches = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> PRCoils = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> PRLamps = new Dictionary<string, Dictionary<string, string>>();

        public static YamlConfig CreateFromFile(string pathToYaml)
        {
            YamlSerializer serializer = new YamlSerializer();
            // var objectRestored = serializer.DeserializeFromFile(@"C:\Users\Jimmy\Documents\Pinball\demo_man\config\dm.yaml")[0];
            object[] results = serializer.DeserializeFromFile(@"C:\Users\Jimmy\Documents\Pinball\demo_man\config\dm.yaml", Type.GetType("string"));

            YamlConfig resultConfig = new YamlConfig();

            Dictionary<object, object> cfg = (Dictionary<object, object>)results[0];

            Dictionary<object, object> tmpDict = (Dictionary<object, object>)cfg["PRGame"];
            Dictionary<object, object> tmpDict2;


            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRGame config data
            ///////////////////////////////////////////////////////////////////////////////
            foreach (object key in tmpDict.Keys)
            {
                resultConfig.PRGame.Add(key.ToString(), tmpDict[key].ToString());
            }

            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRBumpers config data
            ///////////////////////////////////////////////////////////////////////////////
            foreach (object o in (object[])cfg["PRBumpers"])
                resultConfig.PRBumpers.Add(o.ToString());

            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRFlippers config data
            ///////////////////////////////////////////////////////////////////////////////
            foreach (object o in (object[])cfg["PRFlippers"])
                resultConfig.PRFlippers.Add(o.ToString());

            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRSwitches config data
            ///////////////////////////////////////////////////////////////////////////////
            tmpDict = (Dictionary<object, object>)cfg["PRSwitches"];
            foreach (object key1 in tmpDict.Keys)
            {
                tmpDict2 = (Dictionary<object, object>)tmpDict[key1];
                resultConfig.PRSwitches.Add(key1.ToString(), new Dictionary<string, string>());
                foreach (object key2 in tmpDict2.Keys)
                {
                    resultConfig.PRSwitches[key1.ToString()].Add(key2.ToString(), tmpDict2[key2].ToString());
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRCoils config data
            ///////////////////////////////////////////////////////////////////////////////
            tmpDict = (Dictionary<object, object>)cfg["PRCoils"];
            foreach (object key1 in tmpDict.Keys)
            {
                tmpDict2 = (Dictionary<object, object>)tmpDict[key1];
                resultConfig.PRCoils.Add(key1.ToString(), new Dictionary<string, string>());
                foreach (object key2 in tmpDict2.Keys)
                {
                    resultConfig.PRCoils[key1.ToString()].Add(key2.ToString(), tmpDict2[key2].ToString());
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            /// Process PRLamps config data
            ///////////////////////////////////////////////////////////////////////////////
            tmpDict = (Dictionary<object, object>)cfg["PRLamps"];
            foreach (object key1 in tmpDict.Keys)
            {
                tmpDict2 = (Dictionary<object, object>)tmpDict[key1];
                resultConfig.PRLamps.Add(key1.ToString(), new Dictionary<string, string>());
                foreach (object key2 in tmpDict2.Keys)
                {
                    resultConfig.PRLamps[key1.ToString()].Add(key2.ToString(), tmpDict2[key2].ToString());
                }
            }

            ///////////////////////////////////////////////////////////////////////////////
            /// TODO: Process PRBallSave config data. Introduce new structs/classes
            /// as needed for use in the overall Config class
            ///////////////////////////////////////////////////////////////////////////////


            

            Console.WriteLine("Parsing file...");
            return resultConfig;
        }
    }
}
