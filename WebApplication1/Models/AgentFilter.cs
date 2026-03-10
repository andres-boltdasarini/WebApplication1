// Models/BookingAgent.cs
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingAgentApp.Models
{
    public class AgentFilter
    {
        public int? Arch { get; set; }
        public bool? TokenS { get; set; }
        public int? TokenECP { get; set; }
        public bool? Vscode { get; set; }
        public bool? Sublime { get; set; }
        public bool? Notif { get; set; }
        public bool? Copy { get; set; }
        public string? Status { get; set; }
    }
}