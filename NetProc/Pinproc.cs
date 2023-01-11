// The MIT License
// 
// Copyright (c) 2010 Adam Preble
// 
// Permission is hereby granted, free of charge, to any person obtaining a Copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, Copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

//
// C# wrapper for libpinproc (http://github.com/preble/libpinproc)
// Initial version written 10/31/2010 by Adam Preble
// 
// To compile and run with Mono:
// 
//   gmcs pinproc.cs testmain.cs
//   mono pinproc.exe
//

using NetProc.Json;
using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace NetProc
{
    public enum EventType
    {
        Invalid = 0,
        SwitchClosedDebounced = 1,
        SwitchOpenDebounced = 2,
        SwitchClosedNondebounced = 3,
        SwitchOpenNondebounced = 4,
        DMDFrameDisplayed = 5,
        None = 6
    };

    public enum LogLevel
    {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    };

    [JsonConverter(typeof(StringEnumConverter<MachineType>))]
    public enum MachineType
    {
        Invalid = 0,
        Custom = 1,
        WPCAlphanumeric = 2,
        WPC = 3,
        WPC95 = 4,
        SternWhitestar = 5,
        SternSAM = 6,
        PDB = 7
    };

    public enum Result { Success = 1, Failure = 0 };

    [JsonConverter(typeof(StringEnumConverter<SwitchType>))]
    public enum SwitchType
    {
        NO = 0,
        NC = 1
    };
    // Status: Partial test passed (only checked DeHighCycles)
    [StructLayout(LayoutKind.Sequential, Size = 60), Serializable]
    public struct DMDConfig
    {
        public byte NumRows;
        public UInt16 NumColumns;
        public byte NumSubFrames;
        public byte NumFrameBuffers;
        public bool AutoIncBufferWrPtr;
        public bool EnableFrameEvents;
        public bool Enable;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] RclkLowCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] LatchHighCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
        public UInt16[] DeHighCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] DotclkHalfPeriod;

        public DMDConfig(int columns, int rows)
        {
            this.AutoIncBufferWrPtr = true;
            this.NumRows = (byte)rows;
            this.NumColumns = (UInt16)columns;
            this.NumSubFrames = 4;
            this.NumFrameBuffers = 3;
            this.Enable = true;
            this.EnableFrameEvents = true;
            this.RclkLowCycles = new byte[8];
            this.LatchHighCycles = new byte[8];
            this.DeHighCycles = new UInt16[8];
            this.DotclkHalfPeriod = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                this.RclkLowCycles[i] = 15;
                this.LatchHighCycles[i] = 15;
                this.DotclkHalfPeriod[i] = 1;
            }
            // 60 fps timing:
            this.DeHighCycles[0] = 90;
            this.DeHighCycles[1] = 190;
            this.DeHighCycles[2] = 50;
            this.DeHighCycles[3] = 377;
        }
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct DriverGlobalConfig
    {
        public bool EnableOutputs;
        public bool GlobalPolarity;
        public bool UseClear;
        public bool StrobeStartSelect;
        public byte StartStrobeTime;
        public byte MatrixRowEnableIndex1;
        public byte MatrixRowEnableIndex0;
        public bool ActiveLowMatrixRows;
        public bool EncodeEnables;
        public bool TickleSternWatchdog;
        public bool WatchdogExpired;
        public bool WatchdogEnable;
        public UInt16 WatchdogResetTime;
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct DriverGroupConfig
    {
        public byte GroupNum;
        public UInt16 SlowTime;
        public byte EnableIndex;
        public byte RowActivateIndex;
        public byte RowEnableSelect;
        public bool Matrixed;
        public bool Polarity;
        public bool Active;
        public bool DisableStrobeAfter;
    };

    [StructLayout(LayoutKind.Sequential, Size = 28), Serializable]
    public struct DriverState
    {
        public UInt16 DriverNum;
        public byte OutputDriveTime;
        public bool Polarity;
        public bool State;
        public bool WaitForFirstTimeSlot;
        public UInt32 Timeslots;
        public byte PatterOnTime;
        public byte PatterOffTime;
        public bool PatterEnable;
        public bool futureEnable;

        public override string ToString() { return string.Format("DriverState num={0}", DriverNum); }
    };

    // Status: Tested good.
    [StructLayout(LayoutKind.Sequential, Size = 12), Serializable]
    public struct Event
    {
        public EventType Type;
        public UInt32 Value;
        public UInt32 Time;

        public override string ToString() { return string.Format("Event type={0} value={1}", Type, Value); }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PRLED
    {
        public byte boardAddr;
        public byte LEDIndex;
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct PRLEDRGB
    {
        public PRLED pRedLED { get; set; }
        public PRLED pGreenLED { get; set; }
        public PRLED pBlueLED { get; set; }
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SwitchConfig
    {
        public bool Clear;
        public bool HostEventsEnable;
        public bool UseColumn9;
        public bool UseColumn8;
        public byte DirectMatrixScanLoopTime;
        public byte PulsesBeforeCheckingRX;
        public byte InactivePulsesAfterBurst;
        public byte PulsesPerBurst;
        public byte PulseHalfPeriodTime;
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SwitchRule
    {
        public bool ReloadActive;
        public bool NotifyHost;
    };

    /// <summary>
    /// Container for libpinproc (via P/Invoke) methods.
    /// </summary>
    public abstract class PinProc
    {
        /// <summary>
        /// When running on Linux or macOS, the runtime will try prepending lib and appending the canonical shared library extension
        /// </summary>
        public const string DLLName = @"lib/libpinproc";

        public const int kPRDriverCount = 256;

        public const int kPRSwitchCount = 256;

        // Constant
        public const int kPRSwitchPhysicalFirst = 0;
        public const int kPRSwitchPhysicalLast = 223;
        public const int kPRSwitchRulesCount = (kPRSwitchCount << 2);
        public const int kPRSwitchVirtualFirst = 224;
        public const int kPRSwitchVirtualLast = 255;
        // General Methods //

        /// <summary>
        ///  Create a new P-ROC device handle.  Only one handle per device may be created. This handle must be destroyed with PRDelete() when it is no longer needed.  Returns #kPRHandleInvalid if an error occurred.
        /// </summary>
        /// <param name="machineType"></param>
        /// <returns></returns>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PRCreate(MachineType machineType);

        /// <summary>
        /// @brief Converts a coil, lamp, switch, or GI string into a P-ROC driver number.
        /// The following formats are accepted: Cxx (coil), Lxx (lamp), Sxx (matrix switch), SFx (flipper grounded switch), or SDx (dedicated grounded switch).
        /// If the string does not match this format it will be converted into an integer using atoi().<param name="machineType"></param>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 PRDecode(MachineType machineType, string str);

        /// <summary>
        /// Destroys an existing P-ROC device handle.
        /// </summary>
        /// <param name="handle"></param>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDelete(IntPtr handle);

        // Status: UNTESTED
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDMDDraw(IntPtr handle, byte[] dots);

        // DMD Methods //
        // Status: Good
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDMDUpdateConfig(IntPtr handle, ref DMDConfig config);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverFuturePulse(IntPtr handle, byte driverNum, UInt16 milliseconds, UInt16 futureTime);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverGetGroupConfig(IntPtr handle, byte groupNum, ref DriverGroupConfig driverGroupConfig);

        // Status: Soft tested
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverGetState(IntPtr handle, byte driverNum, ref DriverState driverState);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverGroupDisable(IntPtr handle, byte groupNum);

        // Status: UNTESTED
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStateDisable(ref DriverState state);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStateFuturePulse(ref DriverState state, UInt16 milliseconds, UInt16 futureTime);

        /// <summary>
        /// todo: untested Changes the given #PRDriverState to reflect a pitter-patter schedule state.
        /// Assigns a pitter-patter schedule (repeating on/off) to the given driver.
        /// @note The driver state structure must be applied using PRDriverUpdateState() or linked to a switch rule using PRSwitchUpdateRule() to have any effect.<param name="state"></param>
        /// Use originalOnTime to pulse the driver for a number of milliseconds before the pitter-patter schedule begins.
        /// </summary>
        /// <param name="millisecondsOn"></param>
        /// <param name="millisecondsOff"></param>
        /// <param name="originalOnTime"></param>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStatePatter(ref DriverState state, UInt16 millisecondsOn, UInt16 millisecondsOff, UInt16 originalOnTime);

        /// <summary>
        /// todo: untested Changes the given #PRDriverState to reflect a pulse state.
        /// @param milliseconds Number of milliseconds to pulse the driver for.
        /// @note The driver state structure must be applied using PRDriverUpdateState() or linked to a switch rule using PRSwitchUpdateRule() to have any effect.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="milliseconds"></param>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStatePulse(ref DriverState state, byte milliseconds);

        /// <summary>
        /// todo: untested @brief Changes the given #PRDriverState to reflect a pitter-patter schedule state.
        /// Just like the regular Patter above, but PulsePatter only drives the patter</summary>
        /// scheduled for the given number of milliseconds before disabling the driver.<param name="handle"></param>
        /// <param name="driverNum"></param>
        /// <param name="milliseconds"></param>
        /// <param name="futureTime"></param>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStatePulsedPatter(ref DriverState state, UInt16 millisecondsOn, UInt16 millisecondsOff, UInt16 patterTime);

        /// <summary>
        /// todo: untested Changes the given #PRDriverState to reflect a scheduled state.
        /// Assigns a repeating schedule to the given driver.
        /// @note The driver state structure must be applied using PRDriverUpdateState() or linked to a switch rule using PRSwitchUpdateRule() to have any effect.<param name="state"></param>
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="cycleSeconds"></param>
        /// <param name="now"></param>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRDriverStateSchedule(ref DriverState state, UInt32 schedule, byte cycleSeconds, bool now);

        // Status: UNTESTED
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverUpdateGlobalConfig(IntPtr handle, ref DriverGlobalConfig driverGlobalConfig);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverUpdateGroupConfig(IntPtr handle, ref DriverGroupConfig driverGroupConfig);

        // Status: Soft tested
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverUpdateState(IntPtr handle, ref DriverState driverState);

        // Status: Presumed good
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRDriverWatchdogTickle(IntPtr handle);

        /// <summary>
        /// Flush all pending write data out to the P-ROC. 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRFlushWriteData(IntPtr handle);

        /// <summary>
        /// Get all of the available events that have been received.
        /// return Number of events returned; -1 if an error occurred.</summary>
        /// <param name="handle"></param>
        /// <param name="events"></param>
        /// <param name="maxEvents"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PRGetEvents(IntPtr handle, [In, Out] Event[] events, int maxEvents);

        // Status: Good
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string PRGetLastErrorText();

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PRGetVersionInfo(ref UInt16 version, ref UInt16 revision, ref UInt16 combined);

        /// <summary>
        ///  Sets the color of a given PRLED. 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDColor(IntPtr handle, ref PRLED pLED, uint color);

        /// <summary>
        /// Sets the fade color and rate on a given PRLED.Note: The rate will apply to any future PRLEDFadeColor or PRLEDRGBFadeColor calls on the same PD-LED board.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="fadeColor"></param>
        /// <param name="fadeRate"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDFade(IntPtr handle, ref PRLED pLED, byte fadeColor, ushort fadeRate);

        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDFadeColor(IntPtr handle, ref PRLED pLED, byte fadeColor);

        /// <summary>
        /// Sets the fade color on a given PRLED. 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="fadeColor"></param>
        /// <returns></returns>
        /// <summary>
        /// Sets the fade rate on a given board.  Note: The rate will apply to any future PRLEDFadeColor or PRLEDRGBFadeColor calls on the same PD-LED board.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="boardAddr"></param>
        /// <param name="fadeRate"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDFadeRate(byte boardAddr, ushort fadeRate);

        /// <summary>
        /// Sets the color of a given PRLEDRGB.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="color"></param>
        /// <returns></returns>

        [System.Runtime.InteropServices.DllImportAttribute(DLLName)]
        public static extern Result PRLEDRGBColor(IntPtr handle, ref PRLEDRGB pLED, ushort color);

        [System.Runtime.InteropServices.DllImportAttribute(DLLName)]
        public static extern Result PRLEDRGBColorFull(IntPtr handle, byte rAdd, byte rIndex, byte gAdd, byte gIndex, byte bAdd, byte bIndex, uint color);

        /// <summary>
        /// Sets the fade color and rate on a given PRLEDRGB.  Note: The rate will apply to any future PRLEDFadeColor or PRLEDRGBFadeColor calls on any of the referenced PD-LED boards. 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="fadeColor"></param>
        /// <param name="fadeRate"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDRGBFade(IntPtr handle, ref PRLEDRGB pLED, uint fadeColor, ushort fadeRate);

        /// <summary>
        /// Sets the fade color on a given PRLEDRGB.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pLED"></param>
        /// <param name="fadeColor"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRLEDRGBFadeColor(IntPtr handle, ref PRLEDRGB pLED, uint fadeColor);
        
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PRLogSetLevel(IntPtr handle, LogLevel level);

        /// <summary>
        ///  Read data from the P-ROC.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="moduleSelect"></param>
        /// <param name="startingAddr"></param>
        /// <param name="numReadWords"></param>
        /// <param name="readBuffer"></param>
        /// <returns></returns>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRReadData(IntPtr handle, UInt32 moduleSelect, UInt32 startingAddr,
            Int32 numReadWords, ref UInt32 readBuffer);

        /// <summary>
        /// Resets internally maintained driver and switch rule structures.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="flags">resetFlags Specify #kPRResetFlagDefault to only reset the configuration in host memory.  #kPRResetFlagUpdateDevice will write the default configuration to the device, effectively disabling all drivers and switch rules.</param>
        /// <returns></returns>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRReset(IntPtr handle, UInt32 flags);

        /// <summary>
        /// todo: untested. Returns a list of PREventTypes describing the states of the requested number of switches
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="switchStates"></param>
        /// <param name="numSwitches"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRSwitchGetStates(IntPtr handle, [In, Out] EventType[] switchStates, UInt16 numSwitches);

        /// <summary>
        /// Update the switch controller configuration registers
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="switchConfig"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRSwitchUpdateConfig(IntPtr handle, ref SwitchConfig switchConfig);

        /// <summary>
        /// todo: untested. Configures the handling of switch rules within P-ROC.
        /// P-ROC's switch rule system allows the user to decide which switch events are returned to software,
        /// as well as optionally linking one or more driver state changes to rules to create immediate feedback (such as in pop bumpers). <para/>
        /// For instance, P-ROC can provide debounced switch events for a flipper button so software can apply lange change behavior.
        /// This is accomplished by configuring the P-ROC with a switch rule for the flipper button and then receiving the events via the PRGetEvents() call.</summary>
        /// The same switch can also be configured with a non-debounced rule to fire a flipper coil.<param name="handle"></param>
        /// Multiple driver changes can be tied to a single switch state transition to create more complicated effects: a slingshot
        /// switch that fires the slingshot coil, a flash lamp, and a score event. <para/>
        /// P-ROC holds four different switch rules for each switch: closed to open and open to closed, each with a debounced and non-debounced versions:
        /// #kPREventTypeSwitchOpenDebounced, kPREventTypeSwitchClosedDebounced, kPREventTypeSwitchOpenNondebounced, kPREventTypeSwitchClosedNondebounced
        /// <param name="eventType"></param>
        /// <param name="switchNum"></param>
        /// <param name="rule"></param>
        /// <param name="linkedDrivers"></param>
        /// <param name="numDrivers"></param>
        /// <param name="drive_outputs_now"></param>
        /// <returns></returns>
        [DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result PRSwitchUpdateRule(IntPtr handle, byte switchNum, EventType eventType, ref SwitchRule rule, DriverState[] linkedDrivers, int numDrivers, bool drive_outputs_now);

        /// <summary>
        ///  Write data out to the P-ROC immediately (does not require a call to PRFlushWriteData).
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="moduleSelect"></param>
        /// <param name="startingAddr"></param>
        /// <param name="numWriteWords"></param>
        /// <param name="writeBuffer"></param>
        /// <returns></returns>
		[DllImport(DLLName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Result PRWriteData (IntPtr handle, UInt32 moduleSelect, UInt32 startingAddr, 
			Int32 numWriteWords, ref UInt32 writeBuffer);
        
    }
}

