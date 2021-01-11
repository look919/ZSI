using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ZSI.Projekt.Models;
using ZSI.Projekt.Models.DataAccess;

namespace ZSI.Projekt.ViewModels
{
    public class AddTrainingVectorViewModel : BaseViewModel
    {
        private readonly TrainingVectorsHolder _holder;
        private readonly LetterByteArray _mapper;

        public string Message { get; private set; }

        public List<char> Letters {
            get => _mapper.Letters;
        }

        public AddTrainingVectorViewModel()
        {
            _holder = new TrainingVectorsHolder();
            _mapper = new LetterByteArray();

            Message = String.Empty;
        }

        public Bitmap Bitmap { get; set; }

        public Bitmap BitmapToShow { get; set; }

        private ICommand _selectImageCommand;

        public ICommand SelectImageCommand {
            get {
                if (_selectImageCommand == null) {
                    _selectImageCommand = new RelayCommand(
                        param => {
                            SetBitmap();
                        });
                }
                return _selectImageCommand;
            }
        }


        private ICommand _addNewTrainingVectorCommand;

        public ICommand AddNewTrainingVectorCommand {
            get {
                if (_addNewTrainingVectorCommand == null) {
                    _addNewTrainingVectorCommand = new RelayCommand(
                        param => {
                            var letterAsByteArray = CutBitmap.SegmentLetterAndGetAsByteArray(Bitmap);
                            _holder.AddTrainingVector(letterAsByteArray, _mapper.LettersAsByteArrays[(char)param]);
                            SetStateAfterVectorAdded();
                        }, param => Bitmap != null && param != null);
                }
                return _addNewTrainingVectorCommand;
            }
        }

        private void SetStateAfterVectorAdded()
        {
            Bitmap = null;
            BitmapToShow = null;
            OnPropertyChanged(nameof(Bitmap));
            OnPropertyChanged(nameof(BitmapToShow));
            OnPropertyChanged(nameof(BitmapImage));
            Message = "Dodano nowy wektor trenujący";
            OnPropertyChanged(nameof(Message));
        }

        private void SetBitmap()
        {
            var dialog = new OpenFileDialog {
                Filter = "Zdjęcia (.png, .jpg, .jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog().Value) {
                Bitmap = new Bitmap(dialog.FileName);
                BitmapToShow = CutBitmap.SegmentLetter(Bitmap);
                OnPropertyChanged(nameof(BitmapImage));
            }
        }

        public BitmapImage BitmapImage {
            get {
                if (BitmapToShow != null) {
                    using (MemoryStream memory = new MemoryStream()) {
                        BitmapToShow.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                        memory.Position = 0;
                        BitmapImage bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = memory;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();

                        return bitmapimage;
                    }
                }
                return null;
            }
        }
    }
}
