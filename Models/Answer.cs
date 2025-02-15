using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FormsWebApplication.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Template")]
        public int TemplateId { get; set; }  // Link to the specific form submission

        public Template Template { get; set; } // Navigation property to the Form

        [Required, ForeignKey("User")]
        public required string UserId { get; set; } // The user who submitted the form

        public required ApplicationUser User { get; set; } // Navigation property to User

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public bool CustomString1State { get; set; }
        public string? CustomString1Answer { get; set; }

        public bool CustomString2State { get; set; } = false;
        public string? CustomString2Answer { get; set; }

        public bool CustomString3State { get; set; } = false;
        public string? CustomString3Answer { get; set; }

        public bool CustomString4State { get; set; } = false;
        public string? CustomString4Answer { get; set; }

        public bool CustomMultiLine1State { get; set; } = false;
        public string? CustomMultiLine1Answer { get; set; }

        public bool CustomMultiLine2State { get; set; } = false;
        public string? CustomMultiLine2Answer { get; set; }

        public bool CustomMultiLine3State { get; set; } = false;
        public string? CustomMultiLine3Answer { get; set; }

        public bool CustomMultiLine4State { get; set; } = false;
        public string? CustomMultiLine4Answer { get; set; }

        public bool CustomInt1State { get; set; } = false;
        public int? CustomInt1Answer { get; set; }

        public bool CustomInt2State { get; set; } = false;
        public int? CustomInt2Answer { get; set; }

        public bool CustomInt3State { get; set; } = false;
        public int? CustomInt3Answer { get; set; }

        public bool CustomInt4State { get; set; } = false;
        public int? CustomInt4Answer { get; set; }

        public bool CustomCheckbox1State { get; set; } = false;
        public string? CustomCheckbox1Answer { get; set; }

        public bool CustomCheckbox2State { get; set; } = false;
        public string? CustomCheckbox2Answer { get; set; }

        public bool CustomCheckbox3State { get; set; } = false;
        public string? CustomCheckbox3Answer { get; set; }

        public bool CustomCheckbox4State { get; set; } = false;
        public string? CustomCheckbox4Answer { get; set; }

    }
}
