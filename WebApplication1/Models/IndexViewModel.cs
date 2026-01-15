namespace PalindromeChecker.Models
{
    public class IndexViewModel
    {
        public string InputWord { get; set; } = "";
        public PalindromeResult? Result { get; set; }
        public List<string> Examples { get; set; } = new List<string>();
        public string SelectedExample { get; set; } = "";
    }
}