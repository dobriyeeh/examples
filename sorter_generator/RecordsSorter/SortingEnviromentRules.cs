using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordsSorter
{
    public class SortingEnviromentRules
    {
        private const long DefaultMaxChunkSize = 0xF000000;
        private const int DefaultMaxConcurrency = 8;

        public SortingEnviromentRules(long maxChunkSize)
        {
            MaxChunkSize = maxChunkSize;
            MaxConcurrency = DefaultMaxConcurrency;
        }

        public SortingEnviromentRules(long maxChunkSize, int maxConcurrency)
        {
            MaxChunkSize = maxChunkSize;
            MaxConcurrency = Math.Min(maxConcurrency, Environment.ProcessorCount);
        }

        public SortingEnviromentRules() : this(DefaultMaxChunkSize, DefaultMaxConcurrency)
        {
        }

        public long MaxChunkSize { get; }

        public int MaxConcurrency { get; }
    }
}
