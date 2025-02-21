using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FormsWebApplication.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Template")]
        public int TemplateId { get; set; }
        public required Template Template { get; set; }

        [Required, ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public required ApplicationUser User { get; set; }
    }
}
