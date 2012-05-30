using Xunit;

namespace Trappings.Tests
{
    public class TrappingsAcceptanceTests
    {
        [Fact]
        public void It_wraps_code_in_a_using_statement()
        {
            using (new Trappings())
            {
                
            }
        }
    }
}
