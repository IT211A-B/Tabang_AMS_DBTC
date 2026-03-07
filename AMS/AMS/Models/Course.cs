using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{ }
            public int Id { get; set; }

            [Required]
            public string CourseCode { get; set; }  

            public string CourseName { get; set; }  

            public int YearLevel { get; set; }     

            public string Section { get; set; }     

            public ICollection<Student> Students { get; set; }
        }
    }
}
}
