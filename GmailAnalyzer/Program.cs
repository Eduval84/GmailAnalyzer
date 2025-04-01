using Microsoft.Extensions.Configuration;
using GmailAnalyzer.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var openAiKey = configuration["OpenAI:ApiKey"];
var gmailCredentialsPath = configuration["Gmail:CredentialsPath"];

// Example context prompt
var contextPrompt = @"I am a Tech Lead in the Platform team. 
My direct manager is Emilio Medina.
My Cto is Modesto San Juan.
My Coo is Emilio Macias.
My Director is Fernando Hernandez.
My Head of product is Borja Navarro.
I am responsible for the development of the platform and now de most important project is the migration de DAG to the cloud, the new implementation in Panamá and for last mile in the new implementation in Rumania.
I am not interested in the emails of the team members, only the emails that are important for the project.
I am not interested in emails for invitations to SIMA Desktop deployments.
I work with team members Enrique Rosales, Victor Santos, Ronny, Luis León y Alejandro Esteban.";

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
