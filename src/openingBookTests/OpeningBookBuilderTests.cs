using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using openingBook;

namespace openingBookTests
{
    [TestFixture]
    public class OpeningBookBuilderTests
    {
        [Test]
        [TestCase(@"
[Event ""11""]
[Site ""Greenville ch-USA""]
[Date ""1980.??.??""]
[Round ""-""]
[White ""Zaltsman V""]
[Black ""Christiansen Larry M (USA)""]
[Result ""1/2-1/2""]
[ECO ""E11""]

1.d4 Nf6 2.c4 e6 3.Nf3 Bb4+ 4.Bd2 Bxd2+ 5.Qxd2 O-O 6.Nc3 d6 7.g3 Qe7 8.Bg2
Nc6 9.O-O Bd7 10.e4 e5 11.h3 exd4 12.Nxd4 Nxd4 13.Qxd4 Qe5 14.Rad1 Rfe8
15.f4 Qxd4+ 16.Rxd4 Bc6 17.Rc1 Rac8 18.Kf2 a5 19.Bf3 h6 20.Nd5 Kf8 21.b3
Nxd5 22.cxd5 Bd7 23.Bg4 Bxg4 24.hxg4 Ke7 25.Ra4 Kd7 26.Ke3 Ra8 27.Rac4
Rac8 28.Kd4 f6 29.R4c2 Re7 30.Rc3 Rf7 31.Rh1 Re7 32.Rh5 Rce8 33.Re3 Ra8
34.Rf5 Rf8 35.Rh5 Ra8 36.Rh2 Rb8 37.Rhe2 c5 38.dxc6 bxc6 39.Rc2 Rb5 40.Rec3
Rb4+ 41.Rc4 Ke6 42.Kc3 c5 43.a3 Rb5 44.Kb2 Reb7 45.R2c3 Rb8 46.Kc2 Rh8
47.Ra4 h5 48.gxh5 Rxh5 49.Kd3 Rh1 50.Kc4 Rb7 51.Rxa5 Rd1 52.Re3 Rc1+ 53.Rc3
Rd1 54.Re3   1/2-1/2


[Event ""?""]
[Site ""Los Polvorines""]
[Date ""1980.??.??""]
[Round ""-""]
[White ""Torres""]
[Black ""Smyslov Vasily""]
[Result ""0-1""]
[ECO ""E11""]

1.d4 Nf6 2.c4 e6 3.Nf3 Bb4+ 4.Bd2 a5 5.Nc3 b6 6.e3 Bb7 7.Bd3 d6 8.Qe2 Nbd7
9.O-O O-O 10.e4 e5 11.d5 Nc5 12.Bc2 Qd7 13.h3 Ba6 14.Nh4 Bxc3 15.Bxc3 c6
16.f4 cxd5 17.exd5 Nxd5 18.Qh5 Nf6 19.Qe2 Qe6 20.fxe5 Bxc4 21.Qf3 dxe5
22.Rf2 Bd5 23.Qe3 Nfe4 24.Rf5 f6 25.Qe2   0-1", false,
            new string[] { "d4", "Nf6", "c4", "e6", "Nf3", "Bb4+" }, "Bd2", 2,
            new string[] { "Bxd2+", "a5" })]
        public void obb_test1(string pgn, bool isFile, string[] singleChildPath,
            string firstMultiChildPly, int childrenCount, string[] firstMultiChildren)
        {
            OpeningBookBuilder obb = new OpeningBookBuilder();
            obb.BuildFromPGN(pgn, isFile);

            Assert.AreEqual(1, obb.Root.Children.Count);

            // tree should be single-child until we reach Bd2 which should have Bxd2+ and a5 children
            var node = obb.Root.Children.First().Value;
            foreach (var ply in singleChildPath)
            {
                Assert.AreEqual(1, node.Children.Count);
                Assert.AreEqual(ply, node.Ply);
                node = node.Children.First().Value;
            }

            // now check the first non-single-child node and its children
            Assert.AreEqual(firstMultiChildPly, node.Ply);
            Assert.AreEqual(childrenCount, node.Children.Count);

            // make sure all the expected children are found
            var children = node.Children.Values.ToList();
            foreach (var child in children)
            {
                bool found = false;
                foreach (var expectedChild in firstMultiChildren)
                {
                    if (child.Ply == expectedChild)
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found);
            }
        }

        [Test]
        [TestCase(@"d:\tmp\E11 Bogo-Indian defence.pgn")]
        [TestCase(@"d:\tmp\NimzoDefense.pgn")]
        [TestCase(@"d:\tmp\QGSym-Baltic.pgn")]
        public void obb_test2(string pgn)
        {
            try
            {
                OpeningBookBuilder obb = new OpeningBookBuilder();
                obb.BuildFromPGN(pgn);
                Assert.Pass();
            }
            catch (SuccessException)
            {
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Test]
        // [TestCase(@"d:\tmp\pgn")]
        [TestCase(@"D:\dev\cach\src\pgn")]
        public void obb_test3(string pgnDir)
        {
            try
            {
                OpeningBookBuilder obb = new OpeningBookBuilder();

                var files = Directory.EnumerateFileSystemEntries(pgnDir, "*.pgn");
                foreach (var file in files)
                {
                    TestContext.Out.WriteLine($"[{DateTime.Now.ToLongTimeString()}] File: {file}");
                    obb.BuildFromPGN(file);
                }

                Assert.Pass();
            }
            catch (SuccessException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        [TestCase(@"D:\dev\cach\src\pgn", @"d:\tmp\cach_openingbook.txt")]
        public void obb_write_test(string pgnDir, string obPath)
        {
            try
            {
                OpeningBookBuilder obb = new OpeningBookBuilder();

                var files = Directory.EnumerateFileSystemEntries(pgnDir, "*.pgn");
                foreach (var file in files)
                {
                    obb.BuildFromPGN(file);
                }

                obb.Write(obPath);
                Assert.Pass();
            }
            catch (SuccessException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        [TestCase(@"d:\tmp\cach_openingbook.txt")]
        public void obb_read_test(string obPath)
        {
            try
            {
                OpeningBookBuilder obb = new OpeningBookBuilder();
                obb.Read(obPath);

                Assert.Pass();
            }
            catch (SuccessException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}