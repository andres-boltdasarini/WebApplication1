using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication1.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public int UnreadMessages { get; set; }
    }
}
