using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_SophieBakes.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Foreign Key
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // Rating from 1 to 5
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}