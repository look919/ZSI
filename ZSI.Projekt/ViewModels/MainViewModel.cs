using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Input;
using ZSI.Projekt.Models;

namespace ZSI.Projekt.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        enum NetworkState
        {
            NoAction,
            Training,
            Computing
        }

        private readonly NeuralNetworkHandler _handler;

        private NetworkState _state;

        public MainViewModel()
        {
            _handler = new NeuralNetworkHandler();


            _state = NetworkState.NoAction;
        }

        public string Message {
            get {
                var message = String.Empty;

                switch(_state) {
                    case NetworkState.NoAction: message = string.Empty;
                        break;
                    case NetworkState.Training: message = "Trwa trenowanie sieci...";
                        break;
                    case NetworkState.Computing: message = "Trwa rozpoznawanie...";
                        break;
                }

                return message;
            }
        }

        public string IterationsNumberInfo {
            get {
                return $"Iteracje: {_handler.IterationsNumber}";
            }
        }

        public string ErrorInfo {
            get {
                return $"Błąd: {Math.Round(_handler.Error, 3)}";
            }
        }

        public Bitmap Bitmap { get; set; }

        public string Result { get; private set; }

        private ICommand _trainNetworkCommand;

        public ICommand TrainNetworkCommand {
            get {
                if (_trainNetworkCommand == null) {
                    _trainNetworkCommand = new RelayCommand(
                        param => {
                            TrainAndUpdateState();
                        }, param => _state != NetworkState.Training && _state != NetworkState.Computing);
                }
                return _trainNetworkCommand;
            }
        }

        private ICommand _loadBitmapAndComputeCommand;

        public ICommand LoadBitmapAndComputeCommand {
            get {
                if (_loadBitmapAndComputeCommand == null) {
                    _loadBitmapAndComputeCommand = new RelayCommand(
                        param => {
                            SetBitmap();
                            ComputeAndUpdateState();
                        }, param => _state != NetworkState.Training && _state != NetworkState.Computing);
                }
                return _loadBitmapAndComputeCommand;
            }
        }

        private ICommand _addTrainingVectorCommand;

        public ICommand AddTrainingVectorCommand {
            get {
                if (_addTrainingVectorCommand == null) {
                    _addTrainingVectorCommand = new RelayCommand(
                        param => {
                            OnAddTrainingVectorCommandClicked();
                        }, param => _state != NetworkState.Training && _state != NetworkState.Computing);
                }
                return _addTrainingVectorCommand;
            }
        }

        public delegate void EventHandler(object sender, EventArgs args);

        public event EventHandler AddTrainingVectorCommandClicked;

        protected void OnAddTrainingVectorCommandClicked()
        {
            AddTrainingVectorCommandClicked?.Invoke(this, EventArgs.Empty);
        }

        private ICommand _cancelTrainingCommand;

        public ICommand CancelTrainingCommand {
            get {
                if (_cancelTrainingCommand == null) {
                    _cancelTrainingCommand = new RelayCommand(
                        param => {
                            _handler.CancelTraining();
                        }, param => _state == NetworkState.Training);
                }
                return _cancelTrainingCommand;
            }
        }

        private void SetBitmap()
        {
            var dialog = new OpenFileDialog {
                Filter = "Zdjęcia (.png, .jpg, .jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog().Value) {
                Bitmap = new Bitmap(dialog.FileName);
            }
        }

        private async void ComputeAndUpdateState()
        {
            if (Bitmap != null) {
                Result = String.Empty;
                _state = NetworkState.Computing;
                OnPropertyChanged(nameof(Result));
                OnPropertyChanged(nameof(Message));

                Result = await _handler.ComputeAsync(Bitmap);
                _state = NetworkState.NoAction;
                OnPropertyChanged(nameof(Result));
                OnPropertyChanged(nameof(Message));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async void TrainAndUpdateState()
        {
            _state = NetworkState.Training;
            OnPropertyChanged(nameof(Message));

            await _handler.TrainNetworkAsync();

            _state = NetworkState.NoAction;
            OnPropertyChanged(nameof(Message));
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged(nameof(IterationsNumberInfo));
        }

        private void OnErrorChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(nameof(ErrorInfo));
        }

        private void OnIterationsNumberChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(nameof(IterationsNumberInfo));
        }
    }
}
