using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models
{
    [Serializable]
    public class TrainingVector
    {
        public byte[] InputVector { get; private set; }
        public byte[] OutputVector { get; private set; }

        public TrainingVector()
        {
            InputVector = new byte[AppSettings.InputVectorLength];
            OutputVector = new byte[AppSettings.OutputVectorLength];
        }
    }
}
