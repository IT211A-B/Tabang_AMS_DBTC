namespace AMS.Models
{
        public class Student
        {
            public int Id { get; set; }

            [Required]
            public string StudentNumber { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            // Foreign Key
            public int CourseId { get; set; }

            public Course Course { get; set; }
        }
}