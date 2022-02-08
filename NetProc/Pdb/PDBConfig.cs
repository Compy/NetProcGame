using System;
using System.Collections.Generic;

namespace NetProc.Pdb
{
    public class PDBConfig
    {
        public List<DriverAlias> aliases;
        public List<object> indexes;
        public IProcDevice proc;
        private ushort lamp_matrix_strobe_time;
        private bool use_watchdog;
        private ushort watchdog_time;
        public PDBConfig(IProcDevice proc, MachineConfiguration config)
        {
            this.proc = proc;
            this.GetGlobals(config);

            // Initialize some lists for data collection
            List<int> coil_bank_list = new List<int>();
            List<int> lamp_source_bank_list = new List<int>();
            List<PDBLampEntry> lamp_list = new List<PDBLampEntry>();
            List<PDBLampListIndexEntry> lamp_list_for_index = new List<PDBLampListIndexEntry>();
            this.aliases = new List<DriverAlias>();
            this.indexes = new List<object>();

            if (config.PRDriverAliases != null)
            {
                foreach (DriverAliasEntry alias in config.PRDriverAliases)
                {
                    aliases.Add(new DriverAlias(alias.expr, alias.repl));
                }
            }
            // Make a list of unique coil banks
            foreach (CoilConfigFileEntry coil in config.PRCoils)
            {
                Coil coilObj = new Coil(this, coil.Number);
                if (!coil_bank_list.Contains(coilObj.bank()))
                    coil_bank_list.Add(coilObj.bank());
            }

            // Make a list of lamp source banks. The P-ROC only supports 2
            foreach (LampConfigFileEntry lamp in config.PRLamps)
            {
                Lamp lampObj = new Lamp(this, lamp.Number);

                // Catalog PDB banks
                // Dedicated lamps don't use PDB banks, they're direct driver pins.
                if (lampObj.lamp_type == "dedicated") continue;
                else if (lampObj.lamp_type == "pdb")
                {
                    if (!lamp_source_bank_list.Contains(lampObj.SourceBank()))
                        lamp_source_bank_list.Add(lampObj.SourceBank());

                    // Create dicts of unique sink banks. The source index is needed when 
                    // setting up the driver groups
                    PDBLampEntry lamp_dict = new PDBLampEntry()
                    {
                        source_index = lamp_source_bank_list.IndexOf(lampObj.SourceBank()),
                        sink_bank = lampObj.SinkBank(),
                        source_output = lampObj.SourceOutput()
                    };
                    PDBLampListIndexEntry lamp_dict_for_index = new PDBLampListIndexEntry()
                    {
                        source_board = lampObj.SourceBoard(),
                        sink_bank = lampObj.SinkBank(),
                        source_output = lampObj.SourceOutput()
                    };

                    if (!lamp_list.Contains(lamp_dict))
                    {
                        lamp_list.Add(lamp_dict);
                        lamp_list_for_index.Add(lamp_dict_for_index);
                    }
                }
            }
            // Create a list of indexes. The PDB banks will be mapped into this list.
            // The index of the bank is used to calculate the P-ROC driver number for
            // each driver.
            int num_proc_banks = PinProc.kPRDriverCount / 8;
            for (int i = 0; i < num_proc_banks; i++)
                indexes.Add(99);

            this.InitializeDrivers(proc);

            // Set up dedicated driver groups (groups 0-3)
            int group_ctr = 0;
            for (group_ctr = 0; group_ctr < 4; group_ctr++)
            {
                bool enable = coil_bank_list.Contains(group_ctr);
                proc.driver_update_group_config(
                    (byte)group_ctr,
                    0,
                    (byte)group_ctr,
                    0,
                    0,
                    false,
                    true,
                    enable,
                    true
                );
            }
            group_ctr++;

            // Process lamps first. The P-ROC can only control so many drivers directly.
            // Since software won't have the speed to control lamp matrixes, map the lamps
            // first. If there aren't enough P-ROC driver groups for coils, the overflow
            // coils can be controlled by software via VirtualDrivers (which should get set up
            // automagically here)
            for (int i = 0; i < lamp_list.Count; i++)
            {
                PDBLampEntry lamp_dict = lamp_list[i];
                if (group_ctr >= num_proc_banks || lamp_dict.sink_bank >= 16)
                    Console.WriteLine("Lamp matrix banks can't be mapped to index {0} because thats outside of the banks that P-ROC can control.", lamp_dict.sink_bank);
                else
                {
                    Console.WriteLine("Driver group {0}: slow_time={1} enable_index={2} row_activate_index={3} row_enable_index={4} matrix={5}", group_ctr, lamp_matrix_strobe_time, lamp_dict.sink_bank, lamp_dict.source_output, lamp_dict.source_index);
                    this.indexes[group_ctr] = lamp_list_for_index[i];
                    proc.driver_update_group_config((byte)group_ctr,
                        (byte)lamp_matrix_strobe_time,
                        (byte)lamp_dict.sink_bank,
                        (byte)lamp_dict.source_output,
                        (byte)lamp_dict.source_index,
                        true,
                        true,
                        true,
                        true);
                    group_ctr++;
                }
            }

            for (int i = 0; i < coil_bank_list.Count; i++)
            {
                // If the bank is 16 or higher, the P-ROC can't control it directly. Software
                // will have to do the driver logic and write any changes to the PDB bus.
                // Therefore, map these banks to indexes above the P-ROC's driver count,
                // which will force the drivers to be created as VirtualDrivers.
                // Appending the bank avoids conflicts when the group counter (group_ctr) gets too high.
                if (group_ctr >= num_proc_banks || coil_bank_list[i] >= 16)
                {
                    Console.WriteLine("Driver group {0} mapped to driver index outside of P-ROC control. These drivers will become VirtualDrivers. Note, the index will not match the board/bank number; so software will need to request those values before updating the drivers.", coil_bank_list[i]);
                    indexes.Add(coil_bank_list[i]);
                }
                else
                {
                    Console.WriteLine("Driver group {0}: slow_time={1} Enable Index={2}", group_ctr, 0, coil_bank_list[i]);
                    indexes[group_ctr] = coil_bank_list[i];
                    proc.driver_update_group_config((byte)group_ctr,
                        0,
                        (byte)coil_bank_list[i],
                        0,
                        0,
                        false,
                        true,
                        true,
                        true);

                    group_ctr++;
                }
            }

            for (int i = group_ctr; i < 26; i++)
            {
                Console.WriteLine("Driver group {0} disabled", i);
                proc.driver_update_group_config((byte)i,
                    lamp_matrix_strobe_time,
                    0,
                    0,
                    0,
                    false,
                    true,
                    false,
                    true);
            }
            // Make sure there are two indexes. If not, fill them in
            while (lamp_source_bank_list.Count < 2) lamp_source_bank_list.Add(0);

            // Now set up globals. First disable them to allow the P-ROC to set up the
            // polarities on the drivers, then enable them.

            ConfigureGlobals(proc, lamp_source_bank_list, false);
            ConfigureGlobals(proc, lamp_source_bank_list, true);
        }

        public void ConfigureGlobals(IProcDevice proc, List<int> lamp_source_bank_list, bool enable = true)
        {
            if (enable)
            {
                Console.WriteLine(String.Format("Configuring PDB driver globals: polarity = {0}  matrix column index 0 = {1}  matrix column index 1 = {2}", true, lamp_source_bank_list[0], lamp_source_bank_list[1]));
            }

            proc.driver_update_global_config(
                enable, // Don't enable outputs yet
                true, // Polarity
                false, // N/A
                false, // N/A
                1, // N/A
                (byte)lamp_source_bank_list[0],
                (byte)lamp_source_bank_list[1],
                false, // Active low rows? nope
                false, // N/A
                false, // Stern? Heck no
                false, // Reset watchdog trigger
                this.use_watchdog, // Use the watchdog
                this.watchdog_time);

            // Now set up globals
            proc.driver_update_global_config(
                true, // Don't enable outputs yet
                true, // Polarity
                false, // N/A
                false, // N/A
                1, // N/A
                (byte)lamp_source_bank_list[0],
                (byte)lamp_source_bank_list[1],
                false, // Active low rows? nope
                false, // N/A
                false, // Stern? Heck no
                false, // Reset watchdog trigger
                this.use_watchdog, // Use the watchdog
                this.watchdog_time);
        }

        public void GetGlobals(MachineConfiguration config)
        {
            if (config.PRDriverGlobals != null)
            {
                lamp_matrix_strobe_time = config.PRDriverGlobals.lamp_matrix_strobe_time;
                watchdog_time = config.PRDriverGlobals.watchdog_time;
                use_watchdog = config.PRDriverGlobals.use_watchdog;
            }
            else
            {
                lamp_matrix_strobe_time = 200;
                watchdog_time = 1000;
                use_watchdog = true;
            }
        }

        public int GetProcNumber(string section, string number_str)
        {
            int index, bank, num;
            if (section == "PRCoils")
            {
                Coil coil = new Coil(this, number_str);
                bank = coil.bank();
                if (bank == -1) return -1;
                index = this.indexes.IndexOf(bank);
                num = index * 8 + coil.output();
                return num;
            }
            if (section == "PRLamps")
            {
                Lamp lamp = new Lamp(this, number_str);
                if (lamp.lamp_type == "unknown") return -1;
                else if (lamp.lamp_type == "dedicated")
                    return lamp.DedicatedOutput();

                PDBLampListIndexEntry lamp_dict_for_index = new PDBLampListIndexEntry()
                {
                    source_board = lamp.SourceBoard(),
                    sink_bank = lamp.SinkBank(),
                    source_output = lamp.SourceOutput()
                };

                if (!this.indexes.Contains(lamp_dict_for_index)) return -1;
                index = indexes.IndexOf(lamp_dict_for_index);
                num = index * 8 + lamp.SinkOutput();
                return num;
            }
            if (section == "PRSwitches")
            {
                SwitchPdb sw = new SwitchPdb(number_str);
                num = sw.ProcNum();
                return num;
            }
            return -1;
        }

        public void InitializeDrivers(IProcDevice proc)
        {
            // Loop through all of the drivers, initializing them with the polarity
            for (ushort i = 0; i < 208; i++)
            {
                DriverState state = new DriverState();
                state.DriverNum = i;
                state.OutputDriveTime = 0;
                state.Polarity = true;
                state.State = false;
                state.WaitForFirstTimeSlot = false;
                state.Timeslots = 0;
                state.PatterOnTime = 0;
                state.PatterOffTime = 0;
                state.PatterEnable = false;
                state.futureEnable = false;

                proc.driver_update_state(ref state);
            }
        }
    }

    public class PDBLampEntry
    {
        public int sink_bank { get; set; }
        public int source_index { get; set; }
        public int source_output { get; set; }
    }

    public class PDBLampListIndexEntry
    {
        public int sink_bank { get; set; }
        public int source_board { get; set; }
        public int source_output { get; set; }
    }
}
