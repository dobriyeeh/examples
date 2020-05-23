using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RecordsCore
{
    public class RecordsFileSource : IRecordsSource
    {
        private const int BufferSize = 0xFFFF;

        private readonly string _filePath;
        private readonly IRecordConverter _recordConverter;

        private FileStream _fileStream;
        private StreamReader _streamReader;

        public RecordsFileSource(string filePath, IRecordConverter recordConverter)
        {
            _filePath = filePath;
            _recordConverter = recordConverter;

            _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None, BufferSize);
            _streamReader = new StreamReader(_fileStream, Encoding.UTF8, false, BufferSize);
        }

        public IEnumerable<Record> GetRecords()
        {
            while (!_streamReader.EndOfStream)
            {
                var line = _streamReader.ReadLine();

                yield return _recordConverter.FromString(line);
            }
        }

        public Record GetNextRecord()
        {
            if (_streamReader.EndOfStream)
                return null;

            var line = _streamReader.ReadLine();

            return _recordConverter.FromString(line);
        }

        public IEnumerable<Record> GetNextRecords(int recordsCount)
        {
            int counter = recordsCount;

            while (!_streamReader.EndOfStream && (--counter >= 0))
            {
                var line = _streamReader.ReadLine();

                yield return _recordConverter.FromString(line);
            }
        }

        public IEnumerable<Record> GetNextRecordsBySize(long chunkSize)
        {
            long readBytes = 0;

            while (!_streamReader.EndOfStream && (readBytes <= chunkSize))
            {
                var line = _streamReader.ReadLine();
                var record = _recordConverter.FromString(line);

                // simple calculation like sizeof(Number) + sizeof(Text content)
                readBytes += (sizeof(long) + sizeof(char) * record.Text.Length);

                yield return record;
            }
        }


        public void Dispose()
        {
            _streamReader?.Dispose();
            _streamReader = null;
            _fileStream?.Dispose();
            _fileStream = null;
        }
    }
}
