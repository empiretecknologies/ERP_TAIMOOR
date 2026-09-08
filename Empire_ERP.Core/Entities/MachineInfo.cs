namespace Empire_ERP.Core.Entities
{
    public class MachineInfo
    {
        public int? TRAN_ID { get; set; }
        public string? Machine_Code { get; set; }
        public string? Machine_Name { get; set; }
        public string? IP_ADDRESS { get; set; }
        public string? IP_PORT { get; set; }
        public string? SERVER_NAME { get; set; }
        public string? BRANCH { get; set; }
        public string? S_USER_ID { get; set; }
        public string? S_PASSWORD { get; set; }
        public string? DB_NAME { get; set; }
        public string? QUERY { get; set; }
    }
}