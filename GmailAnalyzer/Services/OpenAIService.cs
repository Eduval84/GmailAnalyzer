using OpenAI_API;
using GmailAnalyzer.Models;

namespace GmailAnalyzer.Services
{
    public class OpenAIService
    {
        private readonly OpenAIAPI _api;
        private readonly string _contextPrompt;

        public OpenAIService(string apiKey, string contextPrompt)
        {
            _api = new OpenAIAPI(apiKey);
            _contextPrompt = contextPrompt;
        }

        public async Task<EmailAnalysisResult> AnalyzeEmail(string emailContent)
        {
            var prompt = $"{_contextPrompt}\n\nAnalyze this email:\n{emailContent}\n\n" +
                        "Provide a summary and importance score (1-10) in markdown format.";

            var result = await _api.Completions.CreateCompletion(new OpenAI_API.Completions.CompletionRequest
            {
                Model = "gpt-3.5-turbo",
                Prompt = prompt,
                MaxTokens = 500
            });

            // Parse the JSON response and create EmailAnalysisResult
            // Implementation details...
            return new EmailAnalysisResult();
        }
    }
}
