namespace lab2.Models.DTO
{
    public class PostmoderationDTO
    {
        public string verdict { get; set; }
        public PostmoderationDTO(string verdict)
        {
            this.verdict = verdict;
        }
    }
}
