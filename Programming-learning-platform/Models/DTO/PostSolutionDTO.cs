namespace lab2.Models.DTO
{
    public class PostSolutionDTO
    {
        public string sourceCode { get; set; }
        public string programmingLanguage { get; set; }
        public PostSolutionDTO(string sourceCode, string programmingLanguage)
        {
            this.sourceCode = sourceCode;
            this.programmingLanguage = programmingLanguage;
        }
    }
}
