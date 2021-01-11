using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models
{
    public class LetterByteArray
    {
        public List<char> Letters { get; private set; }

        public  Dictionary<char, byte[]> LettersAsByteArrays { get; private set; }

        public LetterByteArray()
        {
            Letters = Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList();

            LettersAsByteArrays = new Dictionary<char, byte[]>();

            for (int i = 0; i < Letters.Count; i++) {
                var bytes = new byte[AppSettings.OutputVectorLength];

                for (int j = 0; j < bytes.Length; j++) {
                    if (j == i)
                        bytes[j] = 1;
                    else
                        bytes[j] = 0;
                }

                LettersAsByteArrays.Add(Letters[i], bytes);
            }
        }
    }
}
