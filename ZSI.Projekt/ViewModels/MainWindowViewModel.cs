using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ZSI.Projekt.ViewModels
{
    public class MainWindowViewModel : ContainerViewModel
    {
        private readonly BaseViewModel _mainViewModel;

        public MainWindowViewModel()
        {
            _mainViewModel = new MainViewModel();
            (_mainViewModel as MainViewModel).AddTrainingVectorCommandClicked += OnAddTrainingVectorCommandClicked;

            CurrentViewModel = _mainViewModel;
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private ICommand _goToMainViewModelCommand;

        public ICommand GoToMainViewModelCommand {
            get {
                if (_goToMainViewModelCommand == null) {
                    _goToMainViewModelCommand = new RelayCommand(
                        param => {
                            CurrentViewModel = _mainViewModel;
                            OnPropertyChanged(nameof(CurrentViewModel));
                        });
                }
                return _goToMainViewModelCommand;
            }
        }

        private void OnAddTrainingVectorCommandClicked(object sender, EventArgs args)
        {
            CurrentViewModel = new AddTrainingVectorViewModel();
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
