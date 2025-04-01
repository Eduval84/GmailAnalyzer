using OpenAI_API;
using OpenAI_API.Chat;
using GmailAnalyzer.Models;
using System.Text.RegularExpressions;

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
                        "Provide a summary and importance score (1-10) in the following format:\n" +
                        "Subject: [Email subject]\nImportanceScore: [1-10]\nSummary: [Brief summary of the email content]";

            var chatRequest = new ChatRequest()
            {
                Model = "gpt-3.5-turbo",
                Temperature = 0.7,
                MaxTokens = 500,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "You are an AI assistant that helps analyze emails."),
                    new ChatMessage(ChatMessageRole.User, prompt)
                }
            };

            var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
            var responseText = result.Choices[0].Message.Content;

            // Parse the response to extract Subject, ImportanceScore, and Summary
            var emailAnalysis = new EmailAnalysisResult();
            
            // Extract subject
            var subjectMatch = Regex.Match(responseText, @"Subject:\s*(.+)");
            if (subjectMatch.Success)
                emailAnalysis.Subject = subjectMatch.Groups[1].Value.Trim();
            
            // Extract importance score
            var importanceMatch = Regex.Match(responseText, @"ImportanceScore:\s*(\d+)");
            if (importanceMatch.Success && int.TryParse(importanceMatch.Groups[1].Value, out int score))
                emailAnalysis.ImportanceScore = score;
            
            // Extract summary
            var summaryMatch = Regex.Match(responseText, @"Summary:\s*(.+)", RegexOptions.Singleline);
            if (summaryMatch.Success)
                emailAnalysis.Summary = summaryMatch.Groups[1].Value.Trim();

            return emailAnalysis;
        }
    }
}
