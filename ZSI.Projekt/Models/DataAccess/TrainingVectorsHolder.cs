using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models.DataAccess
{
    public class TrainingVectorsHolder
    {
        public List<TrainingVector> TrainingVectors { get; private set; }

        private readonly string _path;

        public TrainingVectorsHolder()
        {
            _path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, AppSettings.TrainingVectorsPath);

            ReadFromFile();
        }

        public void AddTrainingVector(byte[] inputVector, byte[] outputVector)
        {
            var newTrainingVector = new TrainingVector();

            if (inputVector.Length != newTrainingVector.InputVector.Length)
                throw new IncorrectVectorLengthException(inputVector.Length, newTrainingVector.InputVector.Length);

            if (outputVector.Length != newTrainingVector.OutputVector.Length)
                throw new IncorrectVectorLengthException(outputVector.Length, newTrainingVector.OutputVector.Length);

            for (int i = 0; i < inputVector.Length; i++) {
                newTrainingVector.InputVector[i] = inputVector[i];
            }

            for (int i = 0; i < outputVector.Length; i++) {
                newTrainingVector.OutputVector[i] = outputVector[i];
            }

            TrainingVectors.Add(newTrainingVector);

            SaveToFile();
        }

        private void SaveToFile()
        {
            using (var stream = File.Create(_path)) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, TrainingVectors);
            }
        }

        private void ReadFromFile()
        {
            if (!File.Exists(_path))
                File.Create(_path);

            using (var stream = File.OpenRead(_path)) {
                var formatter = new BinaryFormatter();

                if (stream.Length != 0) {
                    TrainingVectors = (List<TrainingVector>)formatter.Deserialize(stream);
                }
            }

            if (TrainingVectors == null)
                TrainingVectors = new List<TrainingVector>();
        }
    }

    public class IncorrectVectorLengthException : Exception
    {
        //layer length error
        public IncorrectVectorLengthException(int provided, int expected)
        : base($"Sizes do not match. The expected length is {expected}, but the provided is {provided}.")
        {

        }
    }
}
