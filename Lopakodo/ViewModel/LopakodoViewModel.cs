using System;
using Lopakodo.Model;
using System.Collections.ObjectModel;
using Lopakodo.Windows.Lopakodo.Persistence;
namespace Lopakodo.ViewModel
{
    class LopakodoViewModel : ViewModelBase
    {
        private GameModel _model;
        private SaveEntry _selectedgame;
        private String _newName = String.Empty;
        public DelegateCommand EasyGameCommand { get; private set; }
        public DelegateCommand MiddleGameCommand { get; private set; }
        public DelegateCommand HardGameCommand { get; private set; }
        public DelegateCommand LoadGameOpenCommand { get; private set; }
        public DelegateCommand LoadGameCloseCommand { get; private set; }
        public DelegateCommand SaveGameOpenCommand { get; private set; }
        public DelegateCommand SaveGameCloseCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand stepCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public ObservableCollection<LopakodoField> Fields { get; set; }
        public ObservableCollection<SaveEntry> Games { get; set; }
        public SaveEntry SelectedGame
        {
            get { return _selectedgame; }
            set
            {
                _selectedgame = value;
                if (_selectedgame != null)
                    NewName = String.Copy(_selectedgame.Name);

                OnPropertyChanged();
                LoadGameCloseCommand.RaiseCanExecuteChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }
        public String NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;

                OnPropertyChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }

       
        public LopakodoViewModel(GameModel model)
        {

            // játék csatlakoztatása
            _model = model;
            _model.PlayerMovedEvent += new EventHandler(PlayerMovedEvent);
            _model.GuardsMovedEvent += new EventHandler(Game_GuardsMovedEvent);
            _model.GeneratedTable += new EventHandler(Game_GeneratedTable);
            
            // parancsok kezelése
            stepCommand = new DelegateCommand(param => _model.RegisterMove(param.ToString()));
            EasyGameCommand = new DelegateCommand(param => EasyGame());
            MiddleGameCommand = new DelegateCommand(param => MiddleGame());
            HardGameCommand = new DelegateCommand(param => HardGame());

            LoadGameOpenCommand = new DelegateCommand(async param =>
            {
                Games = new ObservableCollection<SaveEntry>(await _model.ListGamesAsync());
                OnLoadGameOpen();
            });
            LoadGameCloseCommand = new DelegateCommand(
                param => SelectedGame != null, // parancs végrehajthatóságának feltétele
                param => { OnLoadGameClose(SelectedGame.Name); }
            );
            SaveGameOpenCommand = new DelegateCommand(async param =>
            {
                Games = new ObservableCollection<SaveEntry>(await _model.ListGamesAsync());
                OnSaveGameOpen();
            });
            SaveGameCloseCommand = new DelegateCommand(
                param => NewName.Length > 0, // parancs v
                param => { OnSaveGameClose(NewName); }
            );

            ExitCommand = new DelegateCommand(param => OnExitGame());
            PauseCommand = new DelegateCommand(param => Pause());
            // játéktábla létrehozása
            NewGame(15);
        }
        public void NewGame(Int32 size)
        {
            GenerateTable(size);
            if (size == 15) _model.NewGame(0);
            if (size == 12) _model.NewGame(1);
            if (size == 9) _model.NewGame(2);
            //SetTimer();
        }

        private void GenerateTable(Int32 size)
        {
            Fields = new ObservableCollection<LopakodoField>();
            for (Int32 i = 0; i < _model.TableSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.TableSize; j++)
                {
                    Fields.Add(new LopakodoField
                    {
                        IsLocked = true,
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * _model.TableSize + j,

                    });
                }
            }
            OnPropertyChanged("Fields");
        }

        private void Game_GeneratedTable(Object sender, EventArgs e)
        {
            GenerateTable(_model.TableSize);
            Fields[_model.Exit.x + _model.Exit.y * _model.TableSize].Text = "E";
            Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
            for (Int32 i = 0; i < _model.TableSize / 3; i++)
            {
                Fields[_model.Walls[i].x + _model.Walls[i].y * _model.TableSize].Text = "W";
            }
            for (Int32 i = 0; i < 3; i++)
            {
                Fields[_model.Guards[i].cord.x + _model.Guards[i].cord.y * _model.TableSize].Text = "G";
            }
        }
        private void Game_PlayerMovedEvent(Object sender, EventArgs e)
        {
            switch (_model.Player.actualWay)
            {
                case Way.Down:
                    Fields[_model.Player.cord.x + (_model.Player.cord.y - 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Up:
                    Fields[_model.Player.cord.x + (_model.Player.cord.y + 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Left:
                    Fields[_model.Player.cord.x + 1 + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Right:
                    Fields[_model.Player.cord.x - 1 + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;

            }
        }
        private void Game_GuardsMovedEvent(Object sender, EventArgs e)
        {
            for (Int32 i = 0; i < 3; i++)
            {
                switch (_model.Guards[i].actualWay)
                {
                    case Way.Down:
                        Fields[_model.Guards[i].cord.x + (_model.Guards[i].cord.y - 1) * _model.TableSize].Text = "";

                        break;
                    case Way.Up:
                        Fields[_model.Guards[i].cord.x + (_model.Guards[i].cord.y + 1) * _model.TableSize].Text = "";
                        break;
                    case Way.Left:
                        Fields[(_model.Guards[i].cord.x + 1) + _model.Guards[i].cord.y * _model.TableSize].Text = "";
                        break;
                    case Way.Right:
                        Fields[(_model.Guards[i].cord.x - 1) + _model.Guards[i].cord.y * _model.TableSize].Text = "";

                        break;

                }
            }
            for (Int32 i = 0; i < 3; i++) Fields[_model.Guards[i].cord.x + _model.Guards[i].cord.y * _model.TableSize].Text = "G";
            Fields[_model.Exit.x + _model.Exit.y * _model.TableSize].Text = "E";
        }
        private void PlayerMovedEvent(Object sender, EventArgs e)
        {
            switch (_model.Player.actualWay)
            {
                case Way.Down:
                    Fields[_model.Player.cord.x + (_model.Player.cord.y - 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Up:
                    Fields[_model.Player.cord.x + (_model.Player.cord.y + 1) * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Left:
                    Fields[(_model.Player.cord.x + 1) + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;
                case Way.Right:
                    Fields[(_model.Player.cord.x - 1) + _model.Player.cord.y * _model.TableSize].Text = "";
                    Fields[_model.Player.cord.x + _model.Player.cord.y * _model.TableSize].Text = "P";
                    break;

            }
        }
        private void Pause()
        {
            _model.pause();
        }
        private void MiddleGame()
        {
            NewGame(12);


        }
        private void HardGame()
        {
            NewGame(9);
        }

        private void EasyGame()
        {
            NewGame(15);
        }

        public event EventHandler ExitGame;

        
        public event EventHandler LoadGameOpen;

        public event EventHandler<String> LoadGameClose;

        public event EventHandler SaveGameOpen;

        public event EventHandler<String> SaveGameClose;


        private void OnLoadGameOpen()
        {
            if (LoadGameOpen != null)
                LoadGameOpen(this, EventArgs.Empty);
        }
        
        private void OnLoadGameClose(String name)
        {
            if (LoadGameClose != null)
                LoadGameClose(this, name);
        }
        
        private void OnSaveGameOpen()
        {
            if (SaveGameOpen != null)
                SaveGameOpen(this, EventArgs.Empty);
        }

        private void OnSaveGameClose(String name)
        {
            if (SaveGameClose != null)
                SaveGameClose(this, name);
        }
            private void OnExitGame()
            {
                if (ExitGame != null)
                    ExitGame(this, EventArgs.Empty);
            }
    }
}
