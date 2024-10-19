using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Content { get; set; }

        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey("QuestionId")]

        public Question? Question { get; set; }

    }
}
