namespace AMS.Models
{
    public class Attendance
    {

            public int StudentId { get; set; }

            public DateTime Date { get; set; }

            public string Status { get; set; }

            public Student Student { get; set; }
    }
}
