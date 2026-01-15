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
                };
            }



            // Получаем обратную строку
            string reversed = ReverseString(input);

            // Проверяем на палиндром
            string isText = input + "hi" + reversed;



            return new PalindromeResult
            {
                Original = input,
                Reversed = reversed,
                IsText = isText

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


    }

    /// <summary>
    /// Класс для хранения результата проверки
    /// </summary>
    public class PalindromeResult
    {
        public string Original { get; set; } = "";

        public string Reversed { get; set; } = "";
        public string IsText { get; set; } = "";

    }
}