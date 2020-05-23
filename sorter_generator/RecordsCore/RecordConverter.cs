using System;

namespace RecordsCore
{
    /// <summary>
    /// this converter is for "Number. String" 
    /// </summary>
    public class RecordConverter : IRecordConverter
    {
        public string ToString(Record record)
        {
            return $"{record.Number}. {record.Text}";
        }

        public Record FromString(string textRecord)
        {
            string[] parts = textRecord.Split(new string [] { ". "}, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new ArgumentException("Text string has wrong format: " + textRecord);
            }

            return new Record(long.Parse(parts[0]), parts[1]);
        }
    }
}
