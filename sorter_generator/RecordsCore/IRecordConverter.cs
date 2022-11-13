namespace RecordsCore
{
    public interface IRecordConverter
    {
        string ToString(Record record);
        Record FromString(string textRecord);
    }
}
