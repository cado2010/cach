using System;
using System.IO;
using NUnit.Framework;
using cachCore.models;

namespace cachCoreTests
{
    [TestFixture]
    public class BoardSaveLoadTests
    {
        [Test]
        public void test_board_save()
        {
            try
            {
                var path = Path.GetTempFileName();
                Board board = new Board();
                board.WriteToFile(path);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed, exception: " + ex.Message);
            }
            Assert.Pass();
        }

        //[Test]
        public void test_board_load()
        {
            try
            {
                var path = Path.GetTempFileName();
                Board board = new Board();
                board.WriteToFile(path);

                Board b2 = Board.ReadFromFile(path);
                Assert.IsNotNull(b2);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed, exception: " + ex.Message);
            }
        }
    }
}
