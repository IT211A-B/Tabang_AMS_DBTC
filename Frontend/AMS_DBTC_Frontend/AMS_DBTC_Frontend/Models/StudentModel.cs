namespace AMS_DBTC_Frontend.Models
{
    public class StudentModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#4a6fa5";
        public int P { get; set; }
        public int A { get; set; }
        public int L { get; set; }

        public int Total => P + A + L;
        public int Rate => Total > 0 ? (int)Math.Round((double)P / Total * 100) : 100;
        public bool IsAtRisk => Total > 0 && Rate < 75;

        public string Initials =>
            string.Concat(
                Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(n => n[0].ToString().ToUpper())
            );
    }
}
