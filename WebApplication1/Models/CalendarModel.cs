// Models/CalendarModel.cs
using NodaTime;

namespace BookingAgentApp.Models
{
    public class CalendarModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDay> Days { get; set; } = new();
        public Instant? SelectedStartDate { get; set; }
        public Instant? SelectedEndDate { get; set; }
        public int? SelectedAgentId { get; set; }
    }

    public class CalendarDay
    {
        public LocalDate Date { get; set; }
        public int DayNumber { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsSelected { get; set; }
        public bool IsInRange { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? BookerName { get; set; }
    }
}