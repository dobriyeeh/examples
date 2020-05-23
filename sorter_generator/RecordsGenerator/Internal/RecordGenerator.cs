using RecordsCore;

namespace RecordsGenerator.Internal
{
    internal sealed class RecordGenerator : IValueGenerator<Record>
    {
        private readonly IValueGenerator<long> _numberGenerator;
        private readonly IValueGenerator<string> _stringGenerator;

        public RecordGenerator(IValueGenerator<long> numberGenerator, IValueGenerator<string> stringGenerator)
        {
            _numberGenerator = numberGenerator;
            _stringGenerator = stringGenerator;
        }

        public Record Next()
        {
            return new Record(_numberGenerator.Next(), _stringGenerator.Next());
        }
    }
}
