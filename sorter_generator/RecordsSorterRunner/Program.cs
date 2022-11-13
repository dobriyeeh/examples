using System;
using RecordsSorter;

namespace RecordsSorterRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RecordsSorterRunner <path to original file> <path to output file>");
                return;
            }

            string originalFilePath = args[0];
            string sortedFilePath = args[1];

            try
            {
                var sortingStrategy = new SortingStrategy(new SortingEnviromentRules());
                var sorter = sortingStrategy.ChooseApproachSortMethod(originalFilePath);
                sorter.SortFile(originalFilePath, sortedFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = 1;
            }
        }
    }
}
