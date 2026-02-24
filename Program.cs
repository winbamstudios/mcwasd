using System;
using System.IO;

namespace MCWASD
{
    internal class Program
    {
        public static Ngram[] ngrams;
        public static string[] trainingFile;
        public static int coolerWordInc = 0;
        public static int temperature = 3; // chance of using second or third option
        public static char[] charSeparators = { ' ', '.', ',', ':', ';' };
        static void Main(string[] args)
        {
            try
            {
                string argtest = args[0];
            }
            catch
            {
                Console.WriteLine("Please supply an argument. Use --help for help.");
                Environment.Exit(1);
            }
            if (args[0] != "--help")
            {
                try
                {
                    temperature = Int32.Parse(args[1]);
                }
                catch
                {
                    temperature = 3;
                }
                if (temperature < 0 || temperature > 10)
                {
                    Console.WriteLine("Temperature value out of bounds! Must be between 0 and 10.");
                }
                else
                {
                    try
                    {
                        trainingFile = File.ReadAllLines(args[0]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("File loading failed! See exception below:");
                        if (ex.Message.Contains("Index was outside the bounds of the array"))
                        {
                            Console.WriteLine("No file specified.");
                        }
                        else
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Environment.Exit(1);
                    }
                    foreach (string line in trainingFile)
                    {
                        string[] coolerSplitWords = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string word in coolerSplitWords)
                        {
                            coolerWordInc++;
                        }
                    }
                    ngrams = new Ngram[coolerWordInc];
                    try
                    {
                        Console.WriteLine("Training model, please wait...");
                        Ngram.Train(trainingFile, ngrams);
                        Console.WriteLine("Training succeeded! Temperature is " + temperature.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Training failed. Error: " + ex.Message);
                        Environment.Exit(1);
                    }
                    int asdf = 0;
                    foreach (Ngram n in ngrams)
                    {
                        try
                        {
                            n.UsagePercent = (decimal)n.TimesUsed / (decimal)coolerWordInc;
                            asdf++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    Console.Write("Enter a word\n>");
                    string starter = Console.ReadLine();
                    Console.Write("How many words should be generated?\n>");
                    int howMany = 10;
                    string nextOneIGuess = starter;
                    string cool;
                    try
                    {
                        howMany = Int32.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("That's not a number, using default of 10...");
                    }
                    Console.Write(starter + " ");
                    for (int i = 0; i < howMany; i++)
                    {
                        cool = Ngram.FindNext(nextOneIGuess, ngrams, temperature);
                        nextOneIGuess = cool;
                        Console.Write(cool + " ");
                    }
                    Console.WriteLine();
                } 
            }
            else
            {
                Console.WriteLine("markov chain with a sunny disposition");
                Console.WriteLine("usage: mcwasd [file] [temperature value]");
                Console.WriteLine("temperature values can vary from 0 to 10, if none is specified 3 is used");
            }
        }
    }
    public class Ngram
    {
        public string Sequence1 { get; }
        public string Sequence2 { get; }
        public decimal UsagePercent { get; set; }
        public int TimesUsed { get; set; }

        public Ngram(string p1, string p2)
        {
            this.Sequence1 = p1;
            this.Sequence2 = p2;
            this.TimesUsed = 1;
        }

        public static void Train(string[] trainingData, Ngram[] ngramData)
        {
            int trainInc = 0;
            int wordInc = 0;
            foreach (string line in trainingData)
            {
                string[] splitWords = line.Split(Program.charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                for (int i = 0; i < splitWords.Length - 1; i++)
                {
                    try
                    {
                        ngramData[wordInc] = new Ngram(splitWords[i], splitWords[i + 1]);
                    }
                    catch
                    {
                        break;
                    }
                    wordInc++;
                }
            }
            Array.Resize(ref ngramData, wordInc);
            string[] words1 = new string[Program.coolerWordInc];
            string[] words2 = new string[Program.coolerWordInc];
            /*
            int coolererWordInc = 0;
            foreach (Ngram n in ngramData)
            {
                try
                {
                    words1[coolererWordInc] = n.Sequence1;
                    Console.WriteLine("doing something in the coolerer loop :3");
                    words2[coolererWordInc] = n.Sequence2;
                }
                catch
                {
                    break;
                }
                int coolerererWordInc = 0;
                foreach (string s in words1)
                {
                    if (s == n.Sequence1 && coolerererWordInc != coolererWordInc)
                    {
                        int coolererererWordInc = 0;
                        Console.WriteLine("doing something in the coolererer loop :3");
                        foreach (string b in words2)
                        {
                            if (b == n.Sequence2 && coolererererWordInc != coolerererWordInc)
                            {
                                n.TimesUsed++;
                            }
                        }
                    }
                    coolerererWordInc++;
                }
                coolererWordInc++;
            }
            */
            int uniqueCount = 0;

            foreach (string line in trainingData)
            {
                string[] splitWords = line.Split(
                    Program.charSeparators,
                    StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < splitWords.Length - 1; i++)
                {
                    string w1 = splitWords[i];
                    string w2 = splitWords[i + 1];

                    bool found = false;

                    for (int j = 0; j < uniqueCount; j++)
                    {
                        if (ngramData[j].Sequence1 == w1 &&
                            ngramData[j].Sequence2 == w2)
                        {
                            ngramData[j].TimesUsed++;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        ngramData[uniqueCount] = new Ngram(w1, w2);
                        uniqueCount++;
                    }
                }
            }

            Array.Resize(ref ngramData, uniqueCount);
        }

        public static string FindNext(string inputWord, Ngram[] ngramData, int temperature)
        {
            int possibleNgrams = 0;
            int[] possibleNgramIDs = new int[Program.coolerWordInc];
            int ngramInc = 0;
            foreach (Ngram n in ngramData)
            {
                //Console.WriteLine(n.Sequence1 + " " + n.Sequence2 + " " + n.UsagePercent.ToString());
                try
                {
                    if (n.Sequence1.ToLower() == inputWord.ToLower())
                    {
                        possibleNgramIDs[possibleNgrams] = ngramInc;
                        possibleNgrams++;
                    }
                    ngramInc++;
                }
                catch
                {
                    break;
                }
            }
            Ngram[] possibleNextNgrams = new Ngram[possibleNgrams];
            for (int m = 0; m < possibleNgrams; m++)
            {
                possibleNextNgrams[m] = ngramData[possibleNgramIDs[m]];
            }
            Random random = new Random();
            int ifRandomDoThisOne = 0;
            if (possibleNgrams != 0)
            {
                ifRandomDoThisOne = random.Next(0, possibleNgrams);
            }
            int okayButShouldIDoRandomTho = random.Next(0, 10);
            string returnThisOne = "";
            if (okayButShouldIDoRandomTho < temperature)
            {
                try
                {
                    returnThisOne = possibleNextNgrams[ifRandomDoThisOne].Sequence2;
                }
                catch
                {
                    returnThisOne = Program.trainingFile[random.Next(0, Program.trainingFile.Length - 1)];
                }
            }
            else
            {
                int mostLikelyToBeUsedInSpaghettiCode = 0;
                decimal likelihoodOfThatOne = 0.0m;
                int coolerNgramInc = 0;
                foreach (Ngram n in possibleNextNgrams)
                {
                    if (n.UsagePercent > likelihoodOfThatOne)
                    {
                        mostLikelyToBeUsedInSpaghettiCode = coolerNgramInc;
                        likelihoodOfThatOne = n.UsagePercent;
                    }
                    coolerNgramInc++;
                }
                try
                {
                    returnThisOne = possibleNextNgrams[mostLikelyToBeUsedInSpaghettiCode].Sequence2;
                }
                catch
                {
                    returnThisOne = Program.trainingFile[random.Next(0, Program.trainingFile.Length - 1)];
                }
            }
            return returnThisOne;
        }
    }
}