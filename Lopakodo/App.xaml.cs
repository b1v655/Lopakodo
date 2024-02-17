using System;
using System.Windows;
using Lopakodo.Model;
using Lopakodo.View;
using Lopakodo.ViewModel;
using System.ComponentModel;
using Lopakodo.Windows.Lopakodo.Persistence;
using System.Linq;
using Lopakodo.Persistence;
namespace Lopakodo
{
    public partial class App : Application
    {
        private GameModel _model;
        private LopakodoViewModel _viewModel;

        private LoadWindow _loadWindow;
        private SaveWindow _saveWindow;
        private MainWindow _view;
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            
            //_model = new GameModel(new LopakodoFileDataAccess(AppDomain.CurrentDomain.BaseDirectory));
            _model = new GameModel(new LopakodoDbDataAccess("name=LopakodoModel"));
            _model.GameOver += new EventHandler<LopakodoGameArgs>(Model_GameOver);
           
            _viewModel = new LopakodoViewModel(_model);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGameOpen += new EventHandler(ViewModel_LoadGameOpen);
            _viewModel.LoadGameClose += new EventHandler<String>(ViewModel_LoadGameClose);
            _viewModel.SaveGameOpen += new EventHandler(ViewModel_SaveGameOpen);
            _viewModel.SaveGameClose += new EventHandler<String>(ViewModel_SaveGameClose);
            
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); 
            _view.Show();

        }
        private void ViewModel_LoadGameOpen(Object sender, EventArgs e)
        {
            _model.setTimerOff();
            _viewModel.SelectedGame = null;

            _loadWindow = new LoadWindow(); 
            _loadWindow.DataContext = _viewModel;
            _loadWindow.ShowDialog();
            _model.setTimerOn();
        }
        
        private async void ViewModel_LoadGameClose(object sender, String name)
        {
            if (name != null)
            {
                try
                {
                    await _model.LoadGame(name);
                }
                catch
                {
                    MessageBox.Show("Játék betöltése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _loadWindow.Close(); 
        }
        
        private void ViewModel_SaveGameOpen(Object sender, EventArgs e)
        {
            _model.setTimerOff();
            _viewModel.SelectedGame = null; 
            _viewModel.NewName = String.Empty;

            _saveWindow = new SaveWindow(); 
            _saveWindow.DataContext = _viewModel;
            _saveWindow.ShowDialog(); 

            _model.setTimerOn();
        }
        private async void ViewModel_SaveGameClose(object sender, String name)
        {
            if (name != null)
            {
                try
                {
                    var games = await _model.ListGamesAsync();
                    if (games.All(g => g.Name != name) ||
                        MessageBox.Show("Biztos, hogy felülírja a meglévő mentést?", "Lopakodo",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await _model.SaveGame(name);
                    }
                }
                catch
                {
                    MessageBox.Show("Játék mentése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _saveWindow.Close();
        }
        private void View_Closing(object sender, CancelEventArgs e)
        {

            _model.setTimerOff();
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Lopakodo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; 

                
            }
            _model.setTimerOn();
        }              
        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); 
        }
        private void Model_GameOver(Object sender, Model.LopakodoGameArgs e)
        {
            if (e.IsWon == true)
                MessageBox.Show("Győzelem!", "Játék vége!");
            else
                MessageBox.Show("Elkaptak.", "Játék vége!");
            _viewModel.NewGame(_model.TableSize);

        }
      
    }
}
