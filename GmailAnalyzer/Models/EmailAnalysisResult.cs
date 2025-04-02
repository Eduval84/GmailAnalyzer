namespace GmailAnalyzer.Models
{
    public class EmailAnalysisResult
    {
        public string? Subject { get; set; }
        public string? From { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? Summary { get; set; }
        public int ImportanceScore { get; set; }
    }
}
