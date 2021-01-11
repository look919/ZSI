using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models.NewNeural
{
    [Serializable]
    public class Neurone
    {
        public double[] Weights { get; set; }   //w
        public double[] Values { get; set; }    //u
        public double[] LastWeights { get; set; } //u-1

        public Neurone(int inputs)
        {
            Weights = new double[inputs + 1];
            Values = new double[inputs + 1];

            LastWeights = new double[inputs + 1];

            AssignWeights(); //2
            Values[0] = 1;
        }

        public double Delta { get; set; }

        public double WeightedSum {
            get {
                double sum = 0;
                for (int i = 0; i < Weights.Length; i++) {
                    sum += Weights[i] * Values[i];
                }
                return sum;
            }
        }

        public double Output {
            get {
                return 1 / (1 + Math.Exp(-WeightedSum));
            }
        }

        private void AssignWeights()
        {
            for (int i = 0; i < Weights.Length; i++) {
                Weights[i] = RandomWeights.NextDouble(-0.5, 0.5);
            }
        }
    }
}
