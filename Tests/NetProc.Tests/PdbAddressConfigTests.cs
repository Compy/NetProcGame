using NetProc.Pdb;
using Xunit;

namespace NetProc.Tests
{
    public class PdbAddressConfigTests
    {
        [Theory]
        [InlineData("A0:R0-B1-G2")]
        public void DecodePdb_LED_AddressTests(string address)
        {
            var decodedAddress = PDBFunctions.DecodePdbAddress(address);
            Assert.True(decodedAddress.board == 0);
        }

        [Theory]
        [InlineData("0/0/0")]//board 0, bankA (0) , output 0
        [InlineData("0/1/0")]//board 0, bankB (1) , output 0
        public void DecodePdb_COIL_AddressTests(string address)
        {
            var decodedAddress = PDBFunctions.DecodePdbAddress(address);
            Assert.True(decodedAddress.board == 0);
        }
    }
}
