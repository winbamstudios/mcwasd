using System;
using System.IO;

namespace MCWASD
{
    internal class Program
    {
        public static bool isTrigram = true;
        public static Ngram[] ngrams;
        public static string[] trainingFile;
        public static int coolerWordInc = 0;
        public static int temperature = 3; // chance of using second or third option
        public static char[] charSeparators = { ' ', '.', ',', ':', ';' };
        public static bool isNpt = false;
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
            if (args[0] == "--help")
            {
                Console.WriteLine("markov chain with a sunny disposition");
                Console.WriteLine("usage: mcwasd [file] [temperature value] [amount of n grams]");
                Console.WriteLine("input files can be either txt files to train on startup or npt files for pretrained");
                Console.WriteLine("temperature values can vary from 0 to 10, if none is specified 3 is used");
                Console.WriteLine("n-gram amount is by default 3, or trigram");
                Console.WriteLine("");
                Console.WriteLine("options:");
                Console.WriteLine("--help    : this command");
                Console.WriteLine("--pretrain: use it like \"mcwasd --pretrain [training data] [output file] [amount of n grams]\"");
            }
            else if (args[0] == "--pretrain")
            {
                try
                {
                    string argtest1 = args[1];
                }
                catch
                {
                    Console.WriteLine("Provide an input and output file. Use --help for assistance.");
                    Environment.Exit(1);
                }
                try
                {
                    string argtest2 = args[2];
                }
                catch
                {
                    Console.WriteLine("Provide an output file. Use --help for assistance.");
                    Environment.Exit(1);
                }
                try
                {
                    if (Int32.Parse(args[3]) == 2)
                    {
                        isTrigram = false;
                    }
                    else
                    {
                        isTrigram = true;
                    }
                }
                catch
                {
                    isTrigram = true;
                }
                
                try
                {
                    trainingFile = File.ReadAllLines(args[1]);
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
                    Console.WriteLine("Training succeeded! Saving to " + args[2]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Training failed. Error: " + ex.Message);
                    Environment.Exit(1);
                }
                Console.WriteLine("Calculating usage percentages...");
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
                Console.WriteLine("Calculation succeeded!");
                string[] outputNPT = Pretrainer.ExportNPT(ngrams);
                try
                {
                    File.WriteAllLines(args[2], outputNPT);
                    Console.WriteLine("Successfully written NPT data to " + args[2] + "!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Writing failed! Exception: " + ex);
                }
            }
            else
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
                    if (trainingFile[0].StartsWith("NPT"))
                    {
                        // unchanged because i'm really cool
                        isNpt = true;
                        Ngram[] importedNPT = Pretrainer.Pretrain(trainingFile);
                        ngrams = importedNPT;
                        if (trainingFile[0].EndsWith('3'))
                        {
                            isTrigram = true;
                        }
                        else
                        {
                            isTrigram = false;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (Int32.Parse(args[2]) == 2)
                            {
                                isTrigram = false;
                                Console.WriteLine("Operating in bigram mode");
                            }
                            else
                            {
                                isTrigram = true;
                                Console.WriteLine("Operating in trigram mode");
                            }
                        }
                        catch
                        {
                            isTrigram = true;
                            Console.WriteLine("Operating in trigram mode");
                        }
                        if (isNpt)
                        {
                            if (trainingFile[0].EndsWith('3'))
                            {
                                isTrigram = true;
                            }
                            else
                            {
                                isTrigram = false;
                            }
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
                    }
                    string starter1 = "To";
                    string starter2 = "be";
                    string starter = "The";
                    if (isTrigram)
                    {
                        Console.Write("Enter two words\n>");
                        starter1 = Console.ReadLine();
                        Console.Write(">");
                        starter2 = Console.ReadLine();
                    }
                    else
                    {
                        Console.Write("Enter a word\n>");
                        starter = Console.ReadLine();
                    }
                    Console.Write("How many words should be generated?\n>");
                    int howMany = 10;
                    string nextWord = "";
                    string nextWord1 = "";
                    string nextWord2 = "";
                    string nextFound = "";
                    try
                    {
                        howMany = Int32.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("That's not a number, using default of 10...");
                    }
                    if (!isTrigram)
                    {
                        Console.Write(starter + " ");
                    }
                    else
                    {
                        Console.Write(starter1 + " ");
                    }
                    if (!isTrigram)
                    {
                        nextWord = starter;
                    }
                    else
                    {
                        nextWord1 = starter1;
                        nextWord2 = starter2;
                    }
                    for (int i = 0; i < howMany; i++)
                    {
                        if (!isTrigram)
                        {
                            nextFound = Ngram.FindNext(nextWord, ngrams, temperature);
                            nextWord = nextFound;
                            Console.Write(nextFound + " ");
                        }
                        else
                        {
                            nextFound = Ngram.FindNext(nextWord1, ngrams, temperature, nextWord2);
                            string[] nextFoundSplit = nextFound.Split(' ');
                            nextWord1 = nextFoundSplit[0];
                            try
                            {
                                nextWord2 = nextFoundSplit[1];
                            }
                            catch
                            {
                                nextWord2 = "the";
                            }
                            Console.Write(nextFoundSplit[0] + " ");
                        }
                    }
                    Console.WriteLine();
                } 
            }
        }
    }
    public class Ngram
    {
        public string Sequence1 { get; }
        public string Sequence2 { get; }
        public string Sequence3 { get; }
        public decimal UsagePercent { get; set; }
        public int TimesUsed { get; set; }

        public Ngram(string p1, string p2, string p3 = "")
        {
            this.Sequence1 = p1;
            this.Sequence2 = p2;
            this.Sequence3 = p3;
            this.TimesUsed = 1;
        }

        public static void Train(string[] trainingData, Ngram[] ngramData)
        {
            int trainInc = 0;
            int wordInc = 0;
            if (!Program.isTrigram)
            {
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
            else
            {
                foreach (string line in trainingData)
                {
                    string[] splitWords = line.Split(Program.charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    for (int i = 0; i < splitWords.Length - 2; i++)
                    {
                        try
                        {
                            ngramData[wordInc] = new Ngram(splitWords[i], splitWords[i + 1], splitWords[i + 2]);
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
                string[] words3 = new string[Program.coolerWordInc];
                int uniqueCount = 0;

                foreach (string line in trainingData)
                {
                    string[] splitWords = line.Split(
                        Program.charSeparators,
                        StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < splitWords.Length - 2; i++)
                    {
                        string w1 = splitWords[i];
                        string w2 = splitWords[i + 1];
                        string w3 = splitWords[i + 2];

                        bool found = false;
                        for (int j = 0; j < uniqueCount; j++)
                        {
                            if (ngramData[j].Sequence1 == w1 && ngramData[j].Sequence2 == w2 && ngramData[j].Sequence3 == w3)
                            {
                                ngramData[j].TimesUsed++;
                                found = true;
                                break;
                            }
                        }
                        if (uniqueCount >= ngramData.Length)
                        {
                            Array.Resize(ref ngramData, ngramData.Length + 1);
                        }

                        ngramData[uniqueCount] = new Ngram(w1, w2, w3);
                        uniqueCount++;
                    }
                }
            }
        }

        public static string FindNext(string inputWord, Ngram[] ngramData, int temperature, string inputWord2 = "")
        {
            if (!Program.isTrigram)
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
                        // npt safe version because the npt unsafe version is funnier but breaks on npt
                        if (Program.isNpt)
                        {
                            int integer = random.Next(0, Program.ngrams.Length - 1);
                            if (Program.ngrams[integer] == null)
                            {
                                integer = random.Next(0, Program.ngrams.Length - 1);
                            }
                            else
                            {
                                returnThisOne = Program.ngrams[integer].Sequence2;
                            }
                        }
                        else
                        {
                            returnThisOne = Program.trainingFile[random.Next(0, Program.trainingFile.Length - 1)];
                        }
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
                        if (Program.isNpt)
                        {
                            int integer = random.Next(0, Program.ngrams.Length - 1);
                            if (Program.ngrams[integer] == null)
                            {
                                integer = random.Next(0, Program.ngrams.Length - 1);
                            }
                            else
                            {
                                returnThisOne = Program.ngrams[integer].Sequence2;
                            }
                        }
                        else
                        {
                            returnThisOne = Program.trainingFile[random.Next(0, Program.trainingFile.Length - 1)];
                        }
                    }
                }
                return returnThisOne;
            }
            else
            {
                int possibleNgrams = 0;
                int[] possibleNgramIDs = new int[Program.coolerWordInc];
                int ngramInc = 0;
                foreach (Ngram n in ngramData)
                {
                    //Console.WriteLine(n.Sequence1 + " " + n.Sequence2 + " " + n.UsagePercent.ToString());
                    try
                    {
                        if (n.Sequence1.ToLower() == inputWord.ToLower() && n.Sequence2.ToLower() == inputWord2.ToLower())
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
                        returnThisOne = possibleNextNgrams[ifRandomDoThisOne].Sequence2 + " " + possibleNextNgrams[ifRandomDoThisOne].Sequence2;
                    }
                    catch
                    {
                        // npt safe version because the npt unsafe version is funnier but breaks on npt
                        int integer = random.Next(0, Program.ngrams.Length - 1);
                        if (Program.ngrams[integer] == null)
                        {
                            integer = random.Next(0, Program.ngrams.Length - 1);
                        }
                        else
                        {
                            returnThisOne = Program.ngrams[integer].Sequence2 + " " + Program.ngrams[integer].Sequence3;
                        }
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
                        returnThisOne = possibleNextNgrams[mostLikelyToBeUsedInSpaghettiCode].Sequence2 + " " + possibleNextNgrams[mostLikelyToBeUsedInSpaghettiCode].Sequence3;
                    }
                    catch
                    {
                        // npt safe version again
                        int integer = random.Next(0, Program.ngrams.Length - 1);
                        if (Program.ngrams[integer] == null)
                        {
                            integer = random.Next(0, Program.ngrams.Length - 1);
                        }
                        else
                        {
                            returnThisOne = Program.ngrams[integer].Sequence2 + " " + Program.ngrams[integer].Sequence3;
                        }
                    }
                }
                return returnThisOne;
            }
        }
    }
    public class Pretrainer
    {
        public static string[] ExportNPT(Ngram[] ngramData)
        {
            string[] pretrainOutput = new string[ngramData.Length];
            if (!Program.isTrigram)
            {
                pretrainOutput[0] = "NPT2";
            }
            else
            {
                pretrainOutput[0] = "NPT3";
            }
            if (Program.isTrigram)
            {
                for (int g = 1; g < ngramData.Length; g++)
                {
                    try
                    {
                        if (ngramData[g] == null)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    pretrainOutput[g] = ngramData[g].Sequence1 + "," + ngramData[g].Sequence2 + "," + ngramData[g].Sequence3 + "," + ngramData[g].UsagePercent.ToString();
                }
                return pretrainOutput;
            }
            else
            {
                for (int g = 1; g < ngramData.Length; g++)
                {
                    if (ngramData[g] == null)
                    {
                        break;
                    }
                    pretrainOutput[g] = ngramData[g].Sequence1 + "," + ngramData[g].Sequence2 + "," + ngramData[g].UsagePercent.ToString();
                }
                return pretrainOutput;
            }
        }
        public static Ngram[] Pretrain(string[] nptData)
        {
            if (Program.trainingFile[0] == "NPT3")
            {
                Program.isTrigram = true;
            }
            else
            {
                Program.isTrigram = false;
            }
            Ngram[] ngramOutput = new Ngram[nptData.Length];
            Ngram ngramToExport;
            
            for (int g = 1; g < nptData.Length; g++)
            {
                if (nptData[g] == null || nptData[g] == "")
                {
                    break;
                }
                string[] splitNgram = nptData[g].Split(',');
                if (!Program.isTrigram)
                {
                    ngramToExport = new Ngram(splitNgram[0], splitNgram[1]);
                }
                else
                {
                    ngramToExport = new Ngram(splitNgram[0], splitNgram[1], splitNgram[2]);
                }
                if (!Program.isTrigram)
                {
                    try
                    {
                        ngramToExport.UsagePercent = Decimal.Parse(splitNgram[2]);
                    }
                    catch
                    {
                        Console.WriteLine("Unable to parse decimal!");
                    }
                }
                else
                {
                    try
                    {
                        ngramToExport.UsagePercent = Decimal.Parse(splitNgram[3]);
                    }
                    catch
                    {
                        Console.WriteLine("Unable to parse decimal!");
                    }
                }
                ngramOutput[g] = ngramToExport;
            }
            return ngramOutput;
        }
    }
}