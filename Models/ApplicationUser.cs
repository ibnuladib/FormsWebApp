using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FormsWebApplication.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [PersonalData]
    [Column(TypeName ="nvarchar(100)")]
    public string FirstName { get; set; }

    [Required]
    [PersonalData]
    [Column(TypeName ="nvarchar(100)")]
    public string LastName { get; set; }

    [JsonIgnore]
    public ICollection<Template> Templates { get; set; } = new List<Template>();
    [JsonIgnore]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]
    public ICollection<Like> Likes { get; set; } = new List<Like>();

}

