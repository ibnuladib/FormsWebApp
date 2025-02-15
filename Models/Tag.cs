using System.ComponentModel.DataAnnotations;
using FormsWebApplication.Models.FormsWebApplication.Models;

namespace FormsWebApplication.Models
{

    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string TagName { get; set; }
        public ICollection<TemplateTag> TemplateTags { get; set; } = new List<TemplateTag>();
    }
}
