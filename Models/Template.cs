using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FormsWebApplication.Models.FormsWebApplication.Models;

namespace FormsWebApplication.Models
{
    public class Template
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required, ForeignKey("Author")]
        public required string AuthorId { get; set; }

        [Required]
        public required ApplicationUser Author { get; set; }

        public string? Description { get; set; }

        public TemplateVisibility Visibility { get; set; } = TemplateVisibility.Public;

        public List<ApplicationUser>? AllowedUsers { get; set; } = new();

        public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // **Custom Single-Line Text Questions**
        public bool CustomString1State { get; set; } = false;
        public string? CustomString1Question { get; set; }

        public bool CustomString2State { get; set; } = false;
        public string? CustomString2Question { get; set; }

        public bool CustomString3State { get; set; } = false;
        public string? CustomString3Question { get; set; }

        public bool CustomString4State { get; set; } = false;
        public string? CustomString4Question { get; set; }

        // **Custom Multi-Line Text Questions**
        public bool CustomMultiLine1State { get; set; } = false;
        public string? CustomMultiLine1Question { get; set; }

        public bool CustomMultiLine2State { get; set; } = false;
        public string? CustomMultiLine2Question { get; set; }

        public bool CustomMultiLine3State { get; set; } = false;
        public string? CustomMultiLine3Question { get; set; }

        public bool CustomMultiLine4State { get; set; } = false;
        public string? CustomMultiLine4Question { get; set; }

        // **Custom Positive Integer Questions**
        public bool CustomInt1State { get; set; } = false;
        public string? CustomInt1Question { get; set; }

        public bool CustomInt2State { get; set; } = false;
        public string? CustomInt2Question { get; set; }

        public bool CustomInt3State { get; set; } = false;
        public string? CustomInt3Question { get; set; }

        public bool CustomInt4State { get; set; } = false;
        public string? CustomInt4Question { get; set; }

        // **Custom Checkbox Questions**
        public bool CustomCheckbox1State { get; set; } = false;
        public string? CustomCheckbox1Question { get; set; }

        public bool CustomCheckbox2State { get; set; } = false;
        public string? CustomCheckbox2Question { get; set; }

        public bool CustomCheckbox3State { get; set; } = false;
        public string? CustomCheckbox3Question { get; set; }

        public bool CustomCheckbox4State { get; set; } = false;     
        public string? CustomCheckbox4Question { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        [NotMapped]
        public int AnswerCount { get; set; }

    }
}
