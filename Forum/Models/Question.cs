using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Question
    {
        public int Id { get; set; }

        [RegularExpression(@"[0-0a-zåæøÅÆØ.\-]{2,100}", ErrorMessage = "The Title must be number or " +
            "letters and between 5 and 100 characters")]
        [Display(Name = "Question title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]     
        public string Description { get; set; }

        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }

        public List<Answer>? Answers { get; set; }

    }
}
