using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.NLP
{
    public static class PartsOfSpeech
    {
        private static List<string> ConjunctiveAdverbs { get; set; }

        private static Random random = new Random();

        static PartsOfSpeech()
        {
            ConjunctiveAdverbs = new List<string>()
            { "Then we have", "Then,", "There there's", "Then comes", "Next,", "Up next,", "Up next we have",
                "Then it's", "Next it's", "Next is"
            };
        }

        public static string GetConjunctiveAdverb()
        {
            int r = random.Next(0, 9);
            return ConjunctiveAdverbs[r];
        }
    }
}