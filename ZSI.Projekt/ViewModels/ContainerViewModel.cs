using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZSI.Projekt.ViewModels
{
    public class ContainerViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel { get; set; }

        private ICommand _changeCurrentViewModelCommand;

        public ICommand ChangeCurrentViewModelCommand {
            get {
                if (_changeCurrentViewModelCommand == null) {
                    _changeCurrentViewModelCommand = new RelayCommand(
                        param => {
                            CurrentViewModel = param as BaseViewModel;
                            OnPropertyChanged(nameof(CurrentViewModel));
                        },
                        param => param != null);
                }
                return _changeCurrentViewModelCommand;
            }
        }
    }
}
