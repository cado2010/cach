using System;
using NUnit.Framework;
using cachCore.models;

namespace cachCoreTests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void test_board_save()
        {
            try
            {
                Board board = new Board();
                board.WriteToFile("e:\\tmp\\tmpBoard.cach");
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed, exception: " + ex.Message);
            }
            Assert.Pass();
        }
    }
}
