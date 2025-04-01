using Microsoft.Extensions.Configuration;
using GmailAnalyzer.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var openAiKey = configuration["OpenAI:ApiKey"];
var gmailCredentialsPath = configuration["Gmail:CredentialsPath"];

// Example context prompt
var contextPrompt = @"I am a software developer in the XYZ team. 
My direct manager is John Smith. 
I work with team members Alice, Bob, and Charlie. 
We are currently working on Project Alpha.";

var gmailService = new GmailService(gmailCredentialsPath);
var openAiService = new OpenAIService(openAiKey, contextPrompt);

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
