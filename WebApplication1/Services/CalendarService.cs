// Services/CalendarService.cs
using BookingAgentApp.Models;
using NodaTime;

namespace BookingAgentApp.Services
{
    public class CalendarService
    {
        public CalendarModel GenerateCalendar(int? agentId, int year, int month, Instant? startDate = null, Instant? endDate = null)
        {
            var calendar = new CalendarModel
            {
                Year = year,
                Month = month,
                SelectedStartDate = startDate,
                SelectedEndDate = endDate,
                SelectedAgentId = agentId
            };

            // Конвертируем Instant в LocalDate для календаря
            LocalDate? localStartDate = startDate?.InUtc().Date;
            LocalDate? localEndDate = endDate?.InUtc().Date;

            // Создаем первый день месяца
            var firstDayOfMonth = new LocalDate(year, month, 1);
            
            // Определяем первый день для отображения (понедельник)
            var firstDisplayDay = firstDayOfMonth;
            while (firstDisplayDay.DayOfWeek != IsoDayOfWeek.Monday)
            {
                firstDisplayDay = firstDisplayDay.PlusDays(-1);
            }

            // Генерируем 42 дня (6 недель по 7 дней)
            for (int i = 0; i < 42; i++)
            {
                var currentDate = firstDisplayDay.PlusDays(i);
                var isCurrentMonth = currentDate.Month == month && currentDate.Year == year;
                
                var day = new CalendarDay
                {
                    Date = currentDate,
                    DayNumber = currentDate.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsSelected = (localStartDate.HasValue && currentDate == localStartDate.Value) ||
                                 (localEndDate.HasValue && currentDate == localEndDate.Value),
                    IsInRange = localStartDate.HasValue && localEndDate.HasValue && 
                               currentDate > localStartDate.Value && 
                               currentDate < localEndDate.Value
                };

                calendar.Days.Add(day);
            }

            return calendar;
        }

        public (int year, int month) ChangeMonth(int currentYear, int currentMonth, int offset)
        {
            var date = new LocalDate(currentYear, currentMonth, 1).PlusMonths(offset);
            return (date.Year, date.Month);
        }
    }
}