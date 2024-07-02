using HomeSchool.Core.Reporting.Violations.Domain;
using Liminal.Auth.Models;

namespace HomeSchool.Core.Identity;

public class ApplicationUser : AbstractUser
{
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<Report> Complaints { get; set; } = new List<Report>();
}