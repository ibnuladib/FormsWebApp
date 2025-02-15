using System.ComponentModel.DataAnnotations.Schema;

namespace FormsWebApplication.Models
{
    namespace FormsWebApplication.Models
    {
        public class TemplateTag
        {
            public int TemplateId { get; set; }
            [ForeignKey("TemplateId")]
            public Template Template { get; set; }

            public int TagId { get; set; }
            [ForeignKey("TagId")]
            public Tag Tag { get; set; }
        }
    }
}
