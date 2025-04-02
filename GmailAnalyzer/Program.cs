using Microsoft.Extensions.Configuration;
using GmailAnalyzer.Services;
using GmailAnalyzer.Models;
using System.Text;
using System.IO;

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

// Crear reporte en Markdown
var markdownReport = new StringBuilder();
markdownReport.AppendLine("# Análisis de Correos Electrónicos\n");

// Crear tabla de resumen de todos los correos
markdownReport.AppendLine("## Tabla de Resumen\n");
markdownReport.AppendLine("| Asunto | Importancia |");
markdownReport.AppendLine("|--------|------------|");

foreach (var result in analysisResults.OrderByDescending(r => r.ImportanceScore))
{
    markdownReport.AppendLine($"| {EscapeMarkdown(result.Subject)} | {result.ImportanceScore}/10 |");
}

markdownReport.AppendLine("\n## Correos Importantes\n");

// Filtrar y mostrar solo los correos importantes (score >= 7)
var importantEmails = analysisResults.Where(r => r.ImportanceScore >= 7).OrderByDescending(r => r.ImportanceScore);

if (!importantEmails.Any())
{
    markdownReport.AppendLine("*No se encontraron correos importantes que requieran atención inmediata.*");
}
else
{
    foreach (var result in importantEmails)
    {
        markdownReport.AppendLine($"### {EscapeMarkdown(result.Subject)}\n");
        markdownReport.AppendLine($"**Importancia:** {result.ImportanceScore}/10\n");
        markdownReport.AppendLine($"{EscapeMarkdown(result.Summary)}\n");
        markdownReport.AppendLine("---\n");
    }
}

// Escribir el reporte en un archivo Markdown
string fileName = $"email_analysis_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.md";
File.WriteAllText(fileName, markdownReport.ToString());

// Mostrar resultados en consola
Console.WriteLine(markdownReport.ToString());
Console.WriteLine($"\nReporte guardado como: {fileName}");

// Método auxiliar para escapar caracteres especiales de Markdown
static string EscapeMarkdown(string text)
{
    if (string.IsNullOrEmpty(text))
        return string.Empty;
        
    // Escapar caracteres especiales de Markdown: | * _ # > ~
    return text.Replace("|", "\\|")
               .Replace("*", "\\*")
               .Replace("_", "\\_")
               .Replace("#", "\\#")
               .Replace(">", "\\>")
               .Replace("~", "\\~");
}
