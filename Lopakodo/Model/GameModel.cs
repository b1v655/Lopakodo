using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Lopakodo.Windows.Lopakodo.Persistence;
using System.Collections.Generic;
namespace Lopakodo.Model
{

    
   
    public enum GameDifficulty { Easy, Medium, Hard }
    public class GameModel
    {
        #region declare
        //table az inicializáláshoz szükséges segédtömb;
        private Int32[,] table;
        private Coord[] walls;
        private Random random = new Random();
        private Coord exit;
        private Creatures[] guards;
        private GameDifficulty gameDifficulty;
        private Creatures player;
        private Int32 tableSize=15;
        private Int32 gameTime = 0;
        private DispatcherTimer Timer;
        private ILopakodoDataAccess _dataAccess;
        #endregion
        #region Difficulty constants
        private const Int32 smallTableSize = 9;
        private const Int32 middleTableSize = 12;
        private const Int32 greatTableSize = 15;
        #endregion
        #region Properties

        //Amikor meghal a játékos vége a játéknak.
        public Boolean IsGameOver { get { return (!player.status || (player.cord.x == exit.x && player.cord.y == exit.y)); } }

        public Coord[] Walls { get { return walls; } }
        public Coord Exit { get { return exit; } }
        public Creatures[] Guards { get { return guards; } }

        public Creatures Player { get { return player; } }
        public Int32 TableSize { get { return tableSize; } }

        public GameDifficulty GameDifficulty { get { return gameDifficulty; } set { gameDifficulty = value; } }

        #endregion
        public GameModel(ILopakodoDataAccess dataA)
        {
            _dataAccess = dataA;
            table = new Int32[greatTableSize, greatTableSize];
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += new EventHandler(OnTimedEvent);
            Timer.Start();

        }
        #region Eventhandlers
        public event EventHandler<LopakodoGameArgs> GameOver;
        public event EventHandler GuardsMovedEvent;
        public event EventHandler PlayerMovedEvent;
        public event EventHandler GeneratedTable;
        public void pause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }
        public void RegisterMove(string key)
        {

            if (key.ToLower() == "w") MovePlayer(Way.Up);
            if (key.ToLower() == "s") MovePlayer(Way.Down);
            if (key.ToLower() == "a") MovePlayer(Way.Left);
            if (key.ToLower() == "d") MovePlayer(Way.Right);

        }
        #endregion
        #region init Table
        public void NewGame(int mode)
        {
            gameDifficulty = (GameDifficulty)mode;


            guards = new Creatures[3];
            switch (gameDifficulty) // nehézségfüggő beállítása a táblának
            {
                case GameDifficulty.Easy:

                    tableSize = greatTableSize;
                    walls = new Coord[tableSize / 3];
                    table = new Int32[tableSize, tableSize];
                    GenerateObjects();

                    break;
                case GameDifficulty.Medium:
                    tableSize = middleTableSize;
                    walls = new Coord[tableSize / 3];
                    table = new Int32[tableSize, tableSize];
                    GenerateObjects();
                    break;
                case GameDifficulty.Hard:
                    tableSize = smallTableSize;
                    walls = new Coord[tableSize / 3];
                    table = new Int32[tableSize, tableSize];
                    GenerateObjects();
                    break;
            }
        }
        private bool IsEmpty(Int32 x, Int32 y)
        {
            return table[x, y] == 0;
        }
        private void GenerateObjects()
        {

            for (Int32 i = 0; i < tableSize; i++)
                for (Int32 j = 0; j < tableSize; j++)
                    table[i, j] = 0;
            //játékos elhelyezése
            player = new Creatures();
            player.cord.x = tableSize - 1;
            player.cord.y = tableSize - 1;
            player.status = true;
            table[player.cord.x, player.cord.y] = 1;
            //kijárat elhelyezése
            exit.x = random.Next(tableSize - 3);
            exit.y = random.Next(tableSize - 3);
            table[exit.x, exit.y] = 1;
            //őrök elhelyezése
            for (Int32 i = 0; i < 3; i++)
            {
                Int32 x, y;
                do
                {
                    x = random.Next(tableSize - 3);
                    y = random.Next(tableSize - 3);
                }
                while (!IsEmpty(x, y));
                table[x, y] = 1;
                guards[i] = new Creatures();
                guards[i].cord.x = x;
                guards[i].cord.y = y;
            }
            //falak elhelyezése
            for (Int32 i = 0; i < tableSize / 3; i++)
            {
                Int32 x, y;
                do
                {
                    x = random.Next(tableSize);
                    y = random.Next(tableSize);
                }
                while (!IsEmpty(x, y));
                table[x, y] = 2;
                walls[i].x = x;
                walls[i].y = y;
            }
            Timer.Start();

            if (GeneratedTable != null)
                GeneratedTable(this, new EventArgs());
        }
        #endregion
        #region Move


        private void CheckVisibility()
        {
            for (Int32 i = 0; i < 3; i++)
            {
                if (guards[i].cord.x + 1 >= player.cord.x && guards[i].cord.x - 1 <= player.cord.x && guards[i].cord.y + 1 >= player.cord.y && guards[i].cord.y - 1 <= player.cord.y)
                {
                    player.status = false;
                }
                else if (guards[i].cord.x + 2 >= player.cord.x && guards[i].cord.x - 2 <= player.cord.x && guards[i].cord.y + 2 >= player.cord.y && guards[i].cord.y - 2 <= player.cord.y)
                {
                    if (player.cord.x == guards[i].cord.x - 2)
                    {
                        if (player.cord.y == guards[i].cord.y - 2 && table[guards[i].cord.x - 1, guards[i].cord.y - 1] != 2) player.status = false;
                        if (player.cord.y == guards[i].cord.y - 1 && (table[guards[i].cord.x - 1, guards[i].cord.y] != 2 || table[guards[i].cord.x - 1, guards[i].cord.y - 1] != 2)) player.status = false;
                        if (player.cord.y == guards[i].cord.y && table[guards[i].cord.x - 1, guards[i].cord.y] != 2) player.status = false;
                        if (player.cord.y == guards[i].cord.y + 1 && (table[guards[i].cord.x - 1, guards[i].cord.y] != 2 || table[guards[i].cord.x - 1, guards[i].cord.y + 1] != 2)) player.status = false;
                        if (player.cord.y == guards[i].cord.y + 2 && table[guards[i].cord.x - 1, guards[i].cord.y + 1] != 2) player.status = false;
                    }
                    if (player.cord.x == guards[i].cord.x + 2)
                    {
                        if (player.cord.y == guards[i].cord.y - 2 && table[guards[i].cord.x + 1, guards[i].cord.y - 1] != 2) player.status = false;
                        if (player.cord.y == guards[i].cord.y - 1 && (table[guards[i].cord.x + 1, guards[i].cord.y] != 2 || table[guards[i].cord.x + 1, guards[i].cord.y - 1] != 2)) player.status = false;
                        if (player.cord.y == guards[i].cord.y && table[guards[i].cord.x + 1, guards[i].cord.y] != 2) player.status = false;
                        if (player.cord.y == guards[i].cord.y + 1 && (table[guards[i].cord.x + 1, guards[i].cord.y] != 2 || table[guards[i].cord.x + 1, guards[i].cord.y + 1] != 2)) player.status = false;
                        if (player.cord.y == guards[i].cord.y + 2 && table[guards[i].cord.x + 1, guards[i].cord.y + 1] != 2) player.status = false;
                    }
                    if (player.cord.y == guards[i].cord.y + 2)
                    {
                        if (player.cord.x == guards[i].cord.x - 1 && (table[guards[i].cord.x, guards[i].cord.y + 1] != 2 || table[guards[i].cord.x - 1, guards[i].cord.y + 1] != 2)) player.status = false;
                        if (player.cord.x == guards[i].cord.x && table[guards[i].cord.x, guards[i].cord.y + 1] != 2) player.status = false;
                        if (player.cord.x == guards[i].cord.x + 1 && (table[guards[i].cord.x, guards[i].cord.y + 1] != 2 || table[guards[i].cord.x + 1, guards[i].cord.y + 1] != 2)) player.status = false;
                    }
                    if (player.cord.y == guards[i].cord.y - 2)
                    {
                        if (player.cord.x == guards[i].cord.x - 1 && (table[guards[i].cord.x, guards[i].cord.y - 1] != 2 || table[guards[i].cord.x - 1, guards[i].cord.y - 1] != 2)) player.status = false;
                        if (player.cord.x == guards[i].cord.x && table[guards[i].cord.x, guards[i].cord.y - 1] != 2) player.status = false;
                        if (player.cord.x == guards[i].cord.x + 1 && (table[guards[i].cord.x, guards[i].cord.y - 1] != 2 || table[guards[i].cord.x + 1, guards[i].cord.y - 1] != 2)) player.status = false;
                    }
                }
            }
        }
        private bool IsOk(Creatures guard)
        {
            switch (guard.actualWay)
            {
                case Way.Down:
                    if (guard.cord.y + 1 == tableSize)
                    {
                        guard.actualWay = (Way)random.Next(4);
                        return false;
                    }
                    for (Int32 i = 0; i < tableSize / 3; i++)
                    {
                        if (guard.cord.y + 1 == walls[i].y && guard.cord.x == walls[i].x)
                        {
                            guard.actualWay = (Way)random.Next(4);
                            return false;
                        }
                    }
                    break;
                case Way.Right:
                    if (guard.cord.x + 1 == tableSize)
                    {
                        guard.actualWay = (Way)random.Next(4);
                        return false;
                    }
                    for (Int32 i = 0; i < tableSize / 3; i++)
                    {
                        if (guard.cord.x + 1 == walls[i].x && guard.cord.y == walls[i].y)
                        {
                            guard.actualWay = (Way)random.Next(4);
                            return false;
                        }
                    }
                    break;
                case Way.Up:
                    if (guard.cord.y == 0)
                    {
                        guard.actualWay = (Way)random.Next(4);
                        return false;
                    }
                    for (Int32 i = 0; i < tableSize / 3; i++)
                    {
                        if (guard.cord.y - 1 == walls[i].y && guard.cord.x == walls[i].x)
                        {
                            guard.actualWay = (Way)random.Next(4);
                            return false;
                        }
                    }
                    break;
                case Way.Left:
                    if (guard.cord.x == 0)
                    {
                        guard.actualWay = (Way)random.Next(4);
                        return false;
                    }
                    for (Int32 i = 0; i < tableSize / 3; i++)
                    {
                        if (guard.cord.x - 1 == walls[i].x && guard.cord.y == walls[i].y)
                        {
                            guard.actualWay = (Way)random.Next(4);
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }
        private void MovePlayer(Way way)
        {
            if (!Timer.IsEnabled) return;
            player.actualWay = way;
            if (IsOk(player))
            {
                switch (player.actualWay)
                {
                    case Way.Right:
                        player.cord.x++;
                        break;
                    case Way.Left:
                        player.cord.x--;
                        break;
                    case Way.Up:
                        player.cord.y--;
                        break;
                    case Way.Down:
                        player.cord.y++;
                        break;
                }
                CheckVisibility();
                PlayerMoved();
                if (IsGameOver) OnGameOver();
            }
        }
        private void MoveGuards()
        {
            for (int i = 0; i < 3; i++)
            {
                while (!IsOk(guards[i])) ;
                switch (guards[i].actualWay)
                {
                    case Way.Right:
                        guards[i].cord.x++;
                        break;
                    case Way.Left:
                        guards[i].cord.x--;
                        break;
                    case Way.Up:
                        guards[i].cord.y--;
                        break;
                    case Way.Down:
                        guards[i].cord.y++;
                        break;
                }
            }
            CheckVisibility();
            GuardsMoved_signal();
            if (IsGameOver) OnGameOver();
        }

        #endregion

        private void OnTimedEvent(object source, EventArgs e)
        {
            gameTime++;
            MoveGuards();

        }
        private void GuardsMoved_signal()
        {
            if (GuardsMovedEvent != null)
                GuardsMovedEvent(this, new EventArgs());
        }
        private void PlayerMoved()
        {
            if (PlayerMovedEvent != null)
                PlayerMovedEvent(this, new EventArgs());
        }
        private void OnGameOver()
        {


            Timer.Stop();
            GameOver(this, new LopakodoGameArgs(player.status, (int)this.gameTime));
        }

        public async Task LoadGame(String path)
        {
            Timer.Stop();
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            Tuple<Int32, Creatures, Creatures[], Coord,Coord[]> Loaded_data = await _dataAccess.LoadAsync(path);
            tableSize = Loaded_data.Item1;
            if (tableSize == 15) NewGame(0);
            if (tableSize == 12) NewGame(1);
            if (tableSize == 9) NewGame(2);
            player = Loaded_data.Item2;
            player.status = true;
            guards = Loaded_data.Item3;
            exit = Loaded_data.Item4;
            walls = Loaded_data.Item5;
            for (Int32 i = 0; i < tableSize / 3; i++)
            {
                table[walls[i].x, walls[i].y] = 2;
            }
            if (GeneratedTable != null)
                GeneratedTable(this, new EventArgs());
            Timer.Start();
        }
        public bool isWall(Int32 x, Int32 y) { return table[x, y] == 2; }
        public void setTimerOff() { Timer.Stop(); }
        public void setTimerOn() { Timer.Start(); }
        public async Task SaveGame(String path)
        {
            Timer.Stop();
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, Tuple.Create(tableSize, player, guards, exit, walls));
            Timer.Start();

        }
        public async Task<ICollection<SaveEntry>> ListGamesAsync()
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            return await _dataAccess.ListAsync();
        }
    }

}
