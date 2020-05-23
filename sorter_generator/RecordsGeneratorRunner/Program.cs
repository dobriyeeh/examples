using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecordsGenerator;

namespace RecordsGeneratorRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RecordsGeneratorRunner <path to file> <records count> [frequncy]");
                Console.WriteLine("Usage: frequncy is optional parameter in range 1..0xFFFFF");
                return;
            }

            try
            {
                const int MaxFrequncy = 0xFFFFF;

                string filePath = args[0];
                long itemsCount = long.Parse(args[1]);
                int frequncy = (int) ((itemsCount / 2) % MaxFrequncy);

                if (args.Length > 2)
                {
                    frequncy = int.Parse(args[2]);
                }

                var builder = new RecordGeneratorBuilder();
                var recordGenerator = builder.BuildRecordGenerator(frequncy);
                var recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateMultiThreadGenerator();

                using (var output = builder.BuildRecordsOutput(filePath))
                {
                    recordsSequenceGenerator?.Generate(itemsCount, recordGenerator, output, CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = 1;
            }


        }
    }
}
