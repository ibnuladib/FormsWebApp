using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FormsWebApplication.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Template")]
        public int TemplateId { get; set; }
        public Template Template { get; set; }

        [Required, ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
