// Models/BookingAgent.cs
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingAgentApp.Models
{
    public class BookingAgent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Параметры агента (жестко заданные)
        public int? Arch { get; set; }          // 1 - X86_64, 2 - ARM
        public bool TokenS { get; set; }
        public int? TokenECP { get; set; }       // 0 - не нужен, 1 - Рутокен ЭЦП, 2 - JaCarta
        public bool Vscode { get; set; }
        public bool Sublime { get; set; }
        public bool Notif { get; set; }          // Возможность уведомлений
        public bool Copy { get; set; }            // Возможность резервного копирования

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = "Свободен";
        public string? BookedBy { get; set; }
        public string? BookingTime { get; set; }
        public Instant? StartDate { get; set; }
        public Instant? EndDate { get; set; }

        public string ConnectionCommand { get; set; } = "git/testo/virt-qa-stand/xtesto s12 u3";

        // Вспомогательный метод для проверки соответствия опциям пользователя
        public bool MatchesRequest(AgentRequest request)
        {
            // Проверка архитектуры
            if (Arch != request.Arch)
                return false;

            // Проверка TokenECP
            if (TokenECP != request.TokenECP)
                return false;

            // Примечание: TokenS, Vscode, Sublime, Notif, Copy - 
            // это возможности агента, а не требования пользователя
            // Поэтому они не участвуют в фильтрации

            return true;
        }
    }

    public class AgentRequest
    {
        public int? Arch { get; set; }

        public bool TokenS { get; set; }

        public int? TokenECP { get; set; }

        public bool Vscode { get; set; }

        public bool Sublime { get; set; }

        [Required(ErrorMessage = "Введите вашу почту")]
        public string UserMail { get; set; } = string.Empty;

        // ИЗМЕНЕНО: установлены значения по умолчанию true
        public bool Copy { get; set; } = true;

        public bool Notif { get; set; } = true;

        [Required(ErrorMessage = "Выберите дату начала")]
        [Display(Name = "Начало бронирования")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Выберите дату окончания")]
        [Display(Name = "Конец бронирования")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Больше не нужно, агент выбирается автоматически
        // public int? AgentId { get; set; }
    }
}