namespace CMSController
{
    public enum DataStatus
    {
        UnknownStatus,
        ConnectionError,
        StatusUpdating,
        NeedToUpdate,
        DataUpdating,
        Synced,
    }

    public delegate void DataStatusHandler(DataStatus dataStatus);
}