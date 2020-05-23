using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RecordsGenerator.Internal;

namespace RecordsGenerator
{
    public static class RecordsSequenceGeneratorFactory
    {
        public static IRecordsSequenceGenerator CreateSingleThreadGenerator()
        {
            return new RecordsSequenceSingleThreadGenerator();
        }

        public static IRecordsSequenceGenerator CreateMultiThreadGenerator()
        {
            return RecordsSequenceMultiThreadGenerator.CreateByProcessorCount();
        }

        public static IRecordsSequenceGenerator CreateSingleProducerConsumerGenerator()
        {
            return RecordsSequenceMultiThreadGenerator.CreateByProcessorCount();
        }

    }
}
