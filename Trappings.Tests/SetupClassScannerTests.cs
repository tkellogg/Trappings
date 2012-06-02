using Should;
using Xunit;

namespace Trappings.Tests
{
    #region Types used for testing
    public class SetupClass : IGlobalSetup
    {
        public void SetupOnce()
        {
            SetupClassScannerTests.SetupClassWasCalled = true;
        }
    }
    #endregion

    public class SetupClassScannerTests
    {
        public static bool SetupClassWasCalled;

        [Fact]
        public void It_finds_public_top_level_types()
        {
            new SetupClassScanner().ScanForSetupTypes();
            SetupClassWasCalled.ShouldBeTrue();
        }
    }
}
