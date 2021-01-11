using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSI.Projekt.Models.DataAccess;
using ZSI.Projekt.Models.NewNeural;

namespace ZSI.Projekt.Models
{
    public class NeuralNetworkHandler
    {
        private readonly NeuralNetwork _network;
        private TrainingVectorsHolder _trainingVectorsHolder;
        private NeuralNetworkHolder _neuralNetworkHolder;
        private readonly LetterByteArray _mapper;

        private double[][] _trainingInputs;
        private double[][] _trainingOutputs;

        private bool _isCanceled;

        public double Error { get; private set; }

        public int IterationsNumber { get; private set; }

        public NeuralNetworkHandler()
        {
            _neuralNetworkHolder = new NeuralNetworkHolder();

            _network = _neuralNetworkHolder.Read();
            _mapper = new LetterByteArray();
            _isCanceled = false;

            InitTrainingVectors();

            IterationsNumber = 0;
        }

        //public void SaveNetworkState()
        //{
        //    _neuralNetworkHolder.Save(_network);
        //}

        private void InitTrainingVectors()
        {
            _trainingVectorsHolder = new TrainingVectorsHolder();

            var length = _trainingVectorsHolder.TrainingVectors.Count;      //liczba dodanych wek trenujacych

            _trainingInputs = new double[length][]; 
            _trainingOutputs = new double[length][];

            for (int i = 0; i < length; i++) {
                _trainingInputs[i] = new double[AppSettings.InputVectorLength];
                _trainingOutputs[i] = new double[AppSettings.OutputVectorLength];

                for (int j = 0; j < AppSettings.InputVectorLength; j++) {
                    _trainingInputs[i][j] = _trainingVectorsHolder.TrainingVectors[i].InputVector[j];
                }
                for (int j = 0; j < AppSettings.OutputVectorLength; j++) {
                    _trainingOutputs[i][j] = _trainingVectorsHolder.TrainingVectors[i].OutputVector[j];
                }
            }
        }

        public async Task TrainNetworkAsync()
        {
            await Task.Run(() => TrainNetwork());
        }

        public void TrainNetwork()
        {
            InitTrainingVectors();

            var length = _trainingVectorsHolder.TrainingVectors.Count;

            while (true) {
                if (_isCanceled) {
                    _isCanceled = false;
                    break;
                }
                int index = RandomWeights.Next(0, length);
                _network.Train(_trainingInputs[index], _trainingOutputs[index]);
                IterationsNumber++;
            }

            _neuralNetworkHolder.Save(_network);
        }

        public async Task<string> ComputeAsync(Bitmap bitmap)
        {
            return await Task.Run(() => Compute(bitmap));
        }

        public string Compute(Bitmap bitmap)
        {
            var letters = CutBitmap.SegmentEntireBitmap(bitmap);

            var result = string.Empty;

            var lettersAsDoubleArrays = new List<double[]>();
            foreach (byte[] l in letters) {
                lettersAsDoubleArrays.Add(Array.ConvertAll(l, item => (double)item));
            }

            foreach(double[] l in lettersAsDoubleArrays) {
                var networkOutput = _network.Compute(l);
                var networkOutputAsByteArray = Array.ConvertAll(networkOutput, item => (byte)Math.Round(item));
                var letter = _mapper.LettersAsByteArrays.SingleOrDefault(x => x.Value.SequenceEqual(networkOutputAsByteArray));
                result += letter.Equals(new KeyValuePair<char, byte[]>()) ? '-' : letter.Key;                
            }

            return result;
        }

        public void CancelTraining()
        {
            _isCanceled = true;
        }
    }
}
