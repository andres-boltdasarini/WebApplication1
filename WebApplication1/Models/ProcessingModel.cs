using System.Text.RegularExpressions;

namespace PalindromeChecker.Models
{
public class InputProcessor
    {
        public ProcessingResult ProcessInput(string userInput)
        {
            var result = new ProcessingResult
            {
                OriginalInput = userInput ?? "",
                IsSuccess = false
            };
            
            try
            {
                // Проверка на null или пустую строку
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    result.ErrorMessage = "Ошибка: Входная строка пуста или содержит только пробелы";
                    return result;
                }
                
                // Преобразование строки в число и умножение на 10
                decimal number = decimal.Parse(userInput, 
                    System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture);
                
                decimal calculationResult = number * 10;
                
                result.Result = calculationResult;
                result.DisplayValue = calculationResult.ToString("G29"); // Форматированный вывод
                result.IsSuccess = true;
                
                return result;
            }
            catch (FormatException)
            {
                result.ErrorMessage = $"Ошибка: Строка '{userInput}' имеет неверный формат числа";
                return result;
            }
            catch (OverflowException)
            {
                result.ErrorMessage = $"Ошибка: Число '{userInput}' слишком большое или слишком маленькое";
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Непредвиденная ошибка: {ex.Message}";
                return result;
            }
        }
    }


public class ProcessingResult
    {
        /// <summary>
        /// Исходная строка
        /// </summary>
        public string OriginalInput { get; set; } = "";
        
        /// <summary>
        /// Результат вычислений (если успешно)
        /// </summary>
        public decimal? Result { get; set; }
        
        /// <summary>
        /// Форматированный результат для отображения
        /// </summary>
        public string DisplayValue { get; set; } = "";
        
        /// <summary>
        /// Сообщение об ошибке (если есть)
        /// </summary>
        public string ErrorMessage { get; set; } = "";
        
        /// <summary>
        /// Флаг успешного выполнения
        /// </summary>
        public bool IsSuccess { get; set; }
    }

}