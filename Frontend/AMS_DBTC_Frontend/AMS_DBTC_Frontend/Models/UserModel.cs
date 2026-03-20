namespace AMS_DBTC_Frontend.Models
{
    public class UserModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string Role { get; set; } = "teacher";
        public string Coruse { get; set; } = "BSIT-1";

        public string Initials =>
            string.Concat(
                Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(n => n[0].ToString().ToUpper())
            );

        public string DisplayRole =>
            Role == "admin" ? "Administrator" : Coruse;
    }
}
