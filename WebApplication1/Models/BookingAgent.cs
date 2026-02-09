// Models/BookingAgent.cs
using System.ComponentModel.DataAnnotations;

namespace BookingAgentApp.Models
{
    public class BookingAgent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } = "Свободен";
        public string? BookedBy { get; set; }
        public DateTime? BookingTime { get; set; }
        public string ConnectionCommand { get; set; } = "git/testo/virt-qa-stand/xtesto s12 u3";
    }

    public class AgentRequest
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Выберите агента")]
        public int AgentId { get; set; }
    }
}