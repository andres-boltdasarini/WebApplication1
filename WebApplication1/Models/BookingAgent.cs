
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

        
        public int? Arch { get; set; }        
        public bool TokenS { get; set; }
        public int? TokenECP { get; set; }  
        public bool Vscode { get; set; }
        public bool Sublime { get; set; }
        public bool Notif { get; set; }         
        public bool Copy { get; set; }            

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = "Свободен";
        public string? BookedBy { get; set; }
        public string? BookingTime { get; set; }
        public Instant? StartDate { get; set; }
        public Instant? EndDate { get; set; }

        public string ConnectionCommand { get; set; } = "git/testo/virt-stand/xteso a1 u3";

      
        public bool MatchesRequest(AgentRequest request)
        {
          
            if (Arch != request.Arch)
                return false;

           
            if (TokenECP != request.TokenECP)
                return false;


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

     
    }
}