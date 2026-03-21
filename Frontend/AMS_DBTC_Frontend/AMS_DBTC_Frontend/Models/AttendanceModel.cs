namespace AMS_DBTC_Frontend.Models
{
    public class AttendanceModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string Label => Status switch
        {
            "P" => "Present",
            "A" => "Absent",
            "L" => "Late",
            _ => "—"
        };

        public string BadgeClass => Status switch
        {
            "P" => "bdg-p",
            "A" => "bdg-a",
            "L" => "bdg-l",
            _ => "bdg-n"
        };
    }
}
