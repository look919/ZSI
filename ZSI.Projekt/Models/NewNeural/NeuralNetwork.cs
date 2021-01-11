using System;

namespace ZSI.Projekt.Models.NewNeural
{
    [Serializable]
    public class NeuralNetwork
    {
        private readonly double rho = 0.4; //1

        public Neurone[] HiddenLayer { get; private set; }
        public Neurone[] OutputLayer { get; private set; }

        public NeuralNetwork()
        {
            var inputLength = AppSettings.InputVectorLength;
            var hiddenLength = AppSettings.HiddenVectorLength;
            var outputLength = AppSettings.OutputVectorLength;

            HiddenLayer = new Neurone[hiddenLength];
            for (int i = 0; i < hiddenLength; i++) {
                HiddenLayer[i] = new Neurone(inputLength);
            }

            OutputLayer = new Neurone[outputLength];
            for (int i = 0; i < outputLength; i++) {
                OutputLayer[i] = new Neurone(hiddenLength);
            }
        }

        // klasyfikacja na podstawie podanego wektora sygnałów wejściowych, 3b
        public double[] Compute(double[] signals)
        {
            // hidden layer forward propagation
            for (int i = 0; i < HiddenLayer.Length; i++) {
                for (int j = 0; j < signals.Length; j++) {
                    HiddenLayer[i].Values[j + 1] = signals[j];
                }
            }

            // output layer forward propagation
            for (int i = 0; i < OutputLayer.Length; i++) {
                for (int j = 0; j < HiddenLayer.Length; j++) {
                    OutputLayer[i].Values[j + 1] = HiddenLayer[j].Output;
                }
            }

            var outputs = new double[OutputLayer.Length];

            for (int i = 0; i < OutputLayer.Length; i++) {
                outputs[i] = OutputLayer[i].Output;
            }

            return outputs;
        }

        //wykonanie propagacji wstecznej
        public void Train(double[] vector, double[] expectedOutput)
        {
            Compute(vector);
            
            // output layer backward propagation
            for (int i = 0; i < OutputLayer.Length; i++) {
                var output = OutputLayer[i].Output;
                var derivative = output * (1 - output);
                OutputLayer[i].Delta = (expectedOutput[i] - output) * derivative;

                for (int j = 0; j < OutputLayer[i].Weights.Length; j++) {
                    OutputLayer[i].LastWeights[j] = OutputLayer[i].Weights[j];

                    OutputLayer[i].Weights[j] += rho * OutputLayer[i].Delta * OutputLayer[i].Values[j];

                    OutputLayer[i].Weights[j] += 0.9 * (OutputLayer[i].Weights[j] - OutputLayer[i].LastWeights[j]);
                }
            }

            // hidden layer backward propagation
            PropagateBackwardMiddleLayer(HiddenLayer, OutputLayer);
        }

        private void PropagateBackwardMiddleLayer(Neurone[] layer, Neurone[] nextLayer)
        {
            for (int i = 0; i < layer.Length; i++) {
                var output = layer[i].Output;
                var derivative = output * (1 - output);
                double sum = 0;
                for (int j = 0; j < nextLayer.Length; j++) {
                    sum += nextLayer[j].Weights[i + 1] * nextLayer[j].Delta;
                }
                layer[i].Delta = sum * derivative;

                for (int j = 0; j < layer[i].Weights.Length; j++) {
                    layer[i].LastWeights[j] = layer[i].Weights[j];

                    layer[i].Weights[j] += rho * layer[i].Delta * layer[i].Values[j];

                    layer[i].Weights[j] += 0.9 * (layer[i].Weights[j] - layer[i].LastWeights[j]);
                }
            }
        }
    }
}
