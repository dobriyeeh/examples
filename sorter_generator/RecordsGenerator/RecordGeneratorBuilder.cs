using RecordsCore;
using RecordsGenerator.Internal;

namespace RecordsGenerator
{
    public class RecordGeneratorBuilder
    {
        public IValueGenerator<Record> BuildRecordGenerator(int duplatesFrequncy)
        {
            var randomGenerator = new RandomValueGenerator(duplatesFrequncy);

            var rawNumberGenerator = new NumberValueGeneratorDecorator();
            var cachedNumberGenerator = new CachedValueGeneratorDecorator<long>(rawNumberGenerator, randomGenerator, duplatesFrequncy);

            var stringGenerator = new GuidBasedStringValueGeneratorDecorator();
            var cachedStringGenerator = new CachedValueGeneratorDecorator<string>(stringGenerator, randomGenerator, duplatesFrequncy);

            var recordGenerator = new RecordGenerator(cachedNumberGenerator, cachedStringGenerator);

            return recordGenerator;
        }

        public IRecordsOutput BuildRecordsOutput(string filePath)
        {
            return new RecordsFileOutput(filePath, new RecordConverter());
        }
    }
}
