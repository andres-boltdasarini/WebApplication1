// Models/BookingAgent.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace BookingAgentApp.Models
{
    public class BookingAgent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = "Свободен";

        public string? BookedBy { get; set; }

        public string? BookingTime { get; set; }
        
        // Используем Instant для timestamp with time zone
        public Instant? StartDate { get; set; }
        public Instant? EndDate { get; set; }

        public string ConnectionCommand { get; set; } = "git/testo/virt-qa-stand/xtesto s12 u3";
    }

    public class AgentRequest
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите агента")]
        public int AgentId { get; set; }
        
        [Required(ErrorMessage = "Выберите дату начала")]
        [Display(Name = "Начало бронирования")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "Выберите дату окончания")]
        [Display(Name = "Конец бронирования")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}