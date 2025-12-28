using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication1.Models
{
public class Message
{
    public int Id { get; set; }
    public int ToUserId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
}
}
