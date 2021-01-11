using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models
{
    public static class AppSettings
    {
        public static string TrainingVectorsPath {
            get => GetByKey("trainingVectorsPath");
        }

        public static string NeuralNetworkPath {
            get => GetByKey("neuralNetworkPath");
        }

        public static int InputVectorLength {
            get => int.Parse(GetByKey("inputVectorLength"));
        }

        public static int HiddenVectorLength {
            get => int.Parse(GetByKey("hiddenVectorLength"));
        }

        public static int OutputVectorLength {
            get => int.Parse(GetByKey("outputVectorLength"));
        }

        public static int BitmapSideLength {
            get => int.Parse(GetByKey("bitmapSideLength"));
        }

        private static string GetByKey(string key)
        {
            return ConfigurationSettings.AppSettings[key];
        }
    }
}
