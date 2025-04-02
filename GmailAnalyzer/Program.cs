using Microsoft.Extensions.Configuration;
using GmailAnalyzer.Services;
using GmailAnalyzer.Models;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var openAiKey = configuration["OpenAI:ApiKey"];
var gmailSettings = configuration.GetSection("Gmail").Get<GmailSettings>();
var contextPrompt = configuration["Context:Prompt"];

var gmailService = new GmailService(gmailSettings.Email, gmailSettings.Password);
var openAiService = new OpenAIService(openAiKey ?? string.Empty, contextPrompt ?? string.Empty);

var unreadEmails = await gmailService.GetUnreadEmails();
var analysisResults = new List<EmailAnalysisResult>();

foreach (var email in unreadEmails)
{
    var analysis = await openAiService.AnalyzeEmail(email.ToString());
    analysisResults.Add(analysis);
}

// Display results
foreach (var result in analysisResults.OrderByDescending(r => r.ImportanceScore))
{
    Console.WriteLine($"Subject: {result.Subject}");
    Console.WriteLine($"Importance: {result.ImportanceScore}/10");
    Console.WriteLine($"Summary: {result.Summary}");
    Console.WriteLine("------------------------");
}
