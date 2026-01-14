using System.Text.RegularExpressions;

namespace PalindromeChecker.Models
{
    public class PalindromeModel
    {
        /// <summary>
        /// Проверяет, является ли строка палиндромом
        /// </summary>
        public PalindromeResult CheckPalindrome(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new PalindromeResult
                {
                    Original = input ?? "",
                    Cleaned = "",
                    Reversed = "",
                    IsPalindrome = false,
                    Length = 0,
                    Message = "Введите текст для проверки"
                };
            }

            // Очищаем строку: удаляем пробелы, знаки препинания, приводим к нижнему регистру
            string cleaned = input;

            if (string.IsNullOrEmpty(cleaned))
            {
                return new PalindromeResult
                {
                    Original = input,
                    Cleaned = "",
                    Reversed = "",
                    IsPalindrome = false,
                    Length = 0,
                    Message = "После очистки от знаков препинания текст пуст"
                };
            }

            // Получаем обратную строку
            string reversed = ReverseString(cleaned);

            // Проверяем на палиндром
            bool isPalindrome = cleaned == reversed;

            string message = isPalindrome
                ? "✅ Это палиндром! Текст читается одинаково в обоих направлениях."
                : "❌ Это не палиндром. Текст читается по-разному.";

            return new PalindromeResult
            {
                Original = input,
                Cleaned = cleaned,
                Reversed = reversed,
                IsPalindrome = isPalindrome,
                Length = cleaned.Length,
                Message = message
            };
        }

        /// <summary>

        /// Переворачивает строку
        /// </summary>
        private string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Возвращает примеры палиндромов
        /// </summary>
        public List<string> GetPalindromeExamples()
        {
            return new List<string>
            {
                "топот",
                "казак",
                "шалаш",
                "радар",
                "level",
                "madam",
                "А роза упала на лапу Азора",
                "Аргентина манит негра",
                "Я иду с мечем судия",
                "Лёша на полке клопа нашёл",
                "Madam, I'm Adam",
                "Race car",
                "Never odd or even",
                "12321",
                "1234321"
            };
        }
    }

    /// <summary>
    /// Класс для хранения результата проверки
    /// </summary>
    public class PalindromeResult
    {
        public string Original { get; set; } = "";
        public string Cleaned { get; set; } = "";
        public string Reversed { get; set; } = "";
        public bool IsPalindrome { get; set; }
        public int Length { get; set; }
        public string Message { get; set; } = "";
    }
}