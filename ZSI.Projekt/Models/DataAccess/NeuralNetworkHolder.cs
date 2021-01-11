using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ZSI.Projekt.Models.NewNeural;

namespace ZSI.Projekt.Models.DataAccess
{
    public class NeuralNetworkHolder
    {
        private readonly string _path;

        public NeuralNetworkHolder()
        {
            _path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, AppSettings.NeuralNetworkPath);
        }

        public void Save(NeuralNetwork network)
        {
            using (var stream = File.Create(_path)) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

        public NeuralNetwork Read()
        {
            if (!File.Exists(_path))
                File.Create(_path);

            NeuralNetwork network = null;

            using (var stream = File.OpenRead(_path)) {
                var formatter = new BinaryFormatter();

                if (stream.Length != 0) {
                    network = (NeuralNetwork)formatter.Deserialize(stream);
                }
            }

            return network ?? new NeuralNetwork();
        }
    }
}
