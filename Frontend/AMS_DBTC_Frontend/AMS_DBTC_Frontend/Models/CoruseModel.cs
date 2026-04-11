namespace AMS_DBTC_Frontend.Models
{
    public class CoruseModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? TeacherId { get; set; }
        public string Color { get; set; } = "#4a6fa5";
    }
}
