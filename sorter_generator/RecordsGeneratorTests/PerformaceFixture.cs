using System.IO;
using System.Threading;
using NUnit.Framework;
using RecordsCore;
using RecordsGenerator;

namespace RecordsGeneratorTests
{
    public enum ImplementationModel
    {
        HardcodedGeneratorSolution,     // in this case generator produces same value without cahing, mode is just for comparsion performace
        SingleThreadSolution,
        SingleProducerConsumerSolution,
        MultiThreadSolution 
    }

    [TestFixture]
    public partial class PerformaceFixture
    {
        private string _testFile;

        [SetUp]
        public void SetUp()
        {
            _testFile = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testFile))
            {
                File.Delete(_testFile);
            }
        }

        private void RunTest(
            long itemsCount, 
            double duplicatesPercentage, 
            ImplementationModel implementationModel,
            IRecordsOutput output)
        {
             var builder = new RecordGeneratorBuilder();

            IRecordsSequenceGenerator recordsSequenceGenerator = null;
            IValueGenerator<Record> recordGenerator = null;

            switch (implementationModel)
            {
                case ImplementationModel.HardcodedGeneratorSolution:
                    recordGenerator = new StaticRecordTestGenerator();
                    recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateSingleThreadGenerator();
                    break;

                case ImplementationModel.SingleThreadSolution:
                    recordGenerator = builder.BuildRecordGenerator((int)(itemsCount * duplicatesPercentage));
                    recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateSingleThreadGenerator();
                    break;

                case ImplementationModel.SingleProducerConsumerSolution:
                    recordGenerator = builder.BuildRecordGenerator((int)(itemsCount * duplicatesPercentage));
                    recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateSingleProducerConsumerGenerator();
                    break;

                case ImplementationModel.MultiThreadSolution:
                    recordGenerator = builder.BuildRecordGenerator((int)(itemsCount * duplicatesPercentage));
                    recordsSequenceGenerator = RecordsSequenceGeneratorFactory.CreateMultiThreadGenerator();
                    break;
            }

            using (output)
            {
                recordsSequenceGenerator?.Generate(itemsCount, recordGenerator, output, CancellationToken.None);
            }
        }

        [TestCase(100, 1.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100, 1.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100, 1.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100, 1.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(100, 10.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100, 10.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100, 10.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100, 10.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000, 1.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000, 1.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000, 1.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000, 1.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000, 5.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000, 5.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000, 5.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000, 5.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(100000, 10.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100000, 10.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100000, 10.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100000, 10.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.SingleThreadSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.MultiThreadSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.HardcodedGeneratorSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.SingleThreadSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.SingleProducerConsumerSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.MultiThreadSolution)]
        public void TestFullPerformanceOnDiffCountsWithoutProducerConsumer(long itemsCount, double duplicatesPercentage, ImplementationModel implementationModel)
        {
            var builder = new RecordGeneratorBuilder();

            RunTest(itemsCount, duplicatesPercentage, implementationModel, builder.BuildRecordsOutput(_testFile));
        }

        [TestCase(100, 1.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100, 1.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100, 1.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100, 1.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(100, 10.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100, 10.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100, 10.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100, 10.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000, 1.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000, 1.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000, 1.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000, 1.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000, 5.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000, 5.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000, 5.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000, 5.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(100000, 10.0, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(100000, 10.0, ImplementationModel.SingleThreadSolution)]
        [TestCase(100000, 10.0, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(100000, 10.0, ImplementationModel.MultiThreadSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.SingleThreadSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(1000000, 0.01, ImplementationModel.MultiThreadSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.HardcodedGeneratorSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.SingleThreadSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.SingleProducerConsumerSolution)]
        [TestCase(10000000, 0.0001, ImplementationModel.MultiThreadSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.HardcodedGeneratorSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.SingleThreadSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.SingleProducerConsumerSolution)]
        //[TestCase(100000000, 0.00001, ImplementationModel.MultiThreadSolution)]
        public void TestGenerationPerformanceWithoutSavingOntoDisk(long itemsCount, double duplicatesPercentage, ImplementationModel implementationModel)
        {
            RunTest(itemsCount, duplicatesPercentage, implementationModel, new DummyRecordsOutput());
        }
    }
}
