using NetProc.Pdb;
using Xunit;

namespace NetProc.Tests
{
    public class PdbAddressConfigTests
    {
        [Fact]
        public void DecodePdbAddressTests()
        {
            var address = PDBFunctions.DecodePdbAddress("A0:R0-B1-G2");
            Assert.True(address.board == 0);
        }
    }
}
