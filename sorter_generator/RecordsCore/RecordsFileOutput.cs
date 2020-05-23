using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RecordsCore
{
    public sealed class RecordsFileOutput : IRecordsOutput
    {
        private const int BufferSize = 0xFFFF;

        private readonly IRecordConverter _recordConverter;

        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        public RecordsFileOutput(string filePath, IRecordConverter recordConverter)
        {
            _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BufferSize);
            _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8, BufferSize);
            _recordConverter = recordConverter;
        }

        public void Write(Record record)
        {
            _streamWriter.WriteLine(_recordConverter.ToString(record));
        }

        public void Write(IEnumerable<Record> records)
        {
            foreach (var currRecord in records)
            {
                _streamWriter.WriteLine(_recordConverter.ToString(currRecord));
            }
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
            _fileStream?.Dispose();
            _fileStream = null;
        }
    }
}
