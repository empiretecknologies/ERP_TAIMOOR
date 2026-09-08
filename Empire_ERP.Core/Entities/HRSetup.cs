namespace Empire_ERP.Core.Entities
{
    public class HRSetup
    {
        public List<HolidayDetails>? Holiday { get; set; }
    }
    public class HolidayDetails
    {
        public int GROUP_CODE { get; set; }
        public int ASTATUS { get; set; }
        public string GROUP_NAME { get; set; }
    }
}