using System;
using Lopakodo.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Lopakodo.Windows.Lopakodo.gametest
{
    [TestClass]
    public class ModelTest
    {
        private GameModel _model;

        [TestInitialize]
        public void Initialize()
        {
            _model = new GameModel(new DataMock());
        }


        [TestMethod]
        public void HardGame()
        {
            _model.NewGame(2);

            Assert.AreEqual(8, _model.Player.cord.x);
            Assert.AreEqual(8, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
        }

        [TestMethod]
        public void MiddleGame()
        {
            _model.NewGame(1);

            Assert.AreEqual(11, _model.Player.cord.x);
            Assert.AreEqual(11, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
        }
        [TestMethod]
        public void EasyGame()
        {
            _model.NewGame(0);

            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
        }



        [TestMethod]
        public void MoveTest()
        {
            _model.NewGame(0);
            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);


            _model.RegisterMove("s"); //Nem engedi lépni
            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
            _model.RegisterMove("d"); //Nem engedi lépni
            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
            _model.RegisterMove("m"); //más char Nem engedi lépni
            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            Assert.AreEqual(true, _model.Player.status);
            _model.RegisterMove("w"); //felfele egyet
            if (_model.isWall(14, 13))
            {
                Assert.AreEqual(14, _model.Player.cord.x);
                Assert.AreEqual(14, _model.Player.cord.y);
            }
            else
            {
                Assert.AreEqual(14, _model.Player.cord.x);
                Assert.AreEqual(13, _model.Player.cord.y);
            }
            Assert.AreEqual(true, _model.Player.status);
            _model.RegisterMove("s"); //vissza lép
            Assert.AreEqual(14, _model.Player.cord.x);
            Assert.AreEqual(14, _model.Player.cord.y);
            _model.RegisterMove("a"); //balra egyet
            if (_model.isWall(14, 13))
            {
                Assert.AreEqual(14, _model.Player.cord.x);
                Assert.AreEqual(14, _model.Player.cord.y);
            }
            else
            {
                Assert.AreEqual(13, _model.Player.cord.x);
                Assert.AreEqual(14, _model.Player.cord.y);
                Assert.AreEqual(true, _model.Player.status);
            }


        }
        [TestMethod]
        public void LoadGameTest()
        {
            _model.LoadGame("");
            Assert.AreEqual(9, _model.TableSize);
            Assert.AreEqual(8,_model.Player.cord.x);
            for (Int32 i = 0; i < 3; i++) Assert.AreEqual(i + 3, _model.Walls[i].x);
            for (Int32 i = 0; i < 3; i++) Assert.AreEqual(i, _model.Guards[i].cord.x);
        }

    }
}
