using System.ComponentModel.DataAnnotations;

namespace lab2.Models
{
    public class Solution
    {
        [Key]
        public int id { get; set; }
        public string sourceCode { get; set; }
        public string programmingLanguage { get; set; }
        public string? verdict { get; set; }
        public int authorId { get; set; }
        public int taskId { get; set; }
        public Solution(string sourceCode, string programmingLanguage, int authorId, int taskId, string verdict = "Pending")
        {
            this.sourceCode = sourceCode;
            this.programmingLanguage = programmingLanguage;
            this.authorId = authorId;
            this.taskId = taskId;
            this.verdict = verdict;
        }
    }
}
