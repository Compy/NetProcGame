using NetProcGame;
using System;
using Xunit;

namespace NetProc.Tests
{
    /// <summary>
    /// Integration testing a connected P3-PROC. <para/>
    /// todo: xunit not running test. can try to run test with 64 or 32 dotnet. dotnet test --runtime win10-x86
    /// </summary>
    public class ProcDeviceTests
    {
        const MachineType MACHINE_TYPE = 
            MachineType.PDB;

        public ProcDeviceTests()
        {
            
        }

        [Fact]
        public void Test1()
        {
            ProcDevice proc = null;
            try
            {
                proc = new ProcDevice(MACHINE_TYPE);
                System.Threading.Thread.Sleep(100);
                proc.Reset(1);
                proc?.Close();
                Assert.True(true); //got this far to pass as device is initialized in ProcDevice constructor
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                proc?.Close();
                throw;
            }
        }
    }
}
