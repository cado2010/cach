using NUnit.Framework;
using cachCore.models;
using cachCore.enums;

namespace cachCoreTests
{
    [TestFixture]
    public class PieceTests
    {
        [Test]
        public void test_movement_of_king_1()
        {
            Piece p = new King(ItemColor.Black, new Position(0, 0));
            Movement m = p.GetMovement();
            Assert.AreEqual(m.Paths.Count, 3);

            Position[] checkPos =
            {
                new Position(0, 1),
                new Position(1, 1),
                new Position(1, 0)
            };
            Assert.IsTrue(CheckKingMovement(m, checkPos), "Some path incorrect");
        }

        [Test]
        public void test_movement_of_king_2()
        {
            Piece p = new King(ItemColor.Black, new Position(1, 1));
            Movement m = p.GetMovement();
            Assert.AreEqual(m.Paths.Count, 8);

            Position[] checkPos =
            {
                new Position(0, 0),
                new Position(0, 1),
                new Position(0, 2),
                new Position(1, 0),
                new Position(1, 2),
                new Position(2, 0),
                new Position(2, 1),
                new Position(2, 2)
            };
            Assert.IsTrue(CheckKingMovement(m, checkPos), "Some path incorrect");
        }

        private bool CheckKingMovement(Movement m, Position[] checkPos)
        {
            bool[] check = new bool[checkPos.Length];
            for (int i = 0; i < m.Paths.Count; i++)
            {
                Assert.AreEqual(m.Paths[i].Count, 1);
                Position pos = m.Paths[i][0];

                for (int j = 0; j < checkPos.Length; j++)
                {
                    check[i] = pos.IsSame(checkPos[j]);
                    if (check[i])
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < check.Length; i++)
            {
                if (!check[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
