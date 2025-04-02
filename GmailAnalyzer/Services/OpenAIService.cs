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

        // Método sobrecargado que mantiene la compatibilidad con el código existente
        public async Task<EmailAnalysisResult> AnalyzeEmail(string emailContent)
        {
            // Intentar extraer el asunto del contenido del correo
            string extractedSubject = ExtractSubjectFromEmailContent(emailContent);
            string extractedSender = ExtractSenderFromEmailContent(emailContent);
            string[] extractedCcRecipients = ExtractCcRecipientsFromEmailContent(emailContent);
            
            // Llamar al método principal con los datos extraídos
            return await AnalyzeEmail(emailContent, extractedSubject, extractedSender, extractedCcRecipients);
        }

        private async Task<EmailAnalysisResult> AnalyzeEmail(string emailContent, string subject, string sender, string[] ccRecipients)
        {
            // Primer paso: Evaluar importancia basada en datos básicos
            var initialScore = await EvaluateEmailImportance(subject, sender, ccRecipients);
            
            // Si el correo no es suficientemente importante (puntuación < 7), devolver análisis básico
            if (initialScore < 7)
            {
                return new EmailAnalysisResult
                {
                    Subject = subject,
                    ImportanceScore = initialScore,
                    Summary = "Este correo ha sido clasificado como de baja prioridad basado en el asunto, remitente y destinatarios."
                };
            }
            
            // Si es importante, realizar análisis completo
            return await PerformFullEmailAnalysis(emailContent, subject, initialScore);
        }
        
        // Métodos auxiliares para extraer información del correo
        private string ExtractSubjectFromEmailContent(string emailContent)
        {
            // Intenta extraer el asunto del contenido del correo
            var subjectMatch = Regex.Match(emailContent, @"Subject:[ \t]*([^\r\n]+)", RegexOptions.IgnoreCase);
            return subjectMatch.Success ? subjectMatch.Groups[1].Value.Trim() : "Sin asunto";
        }
        
        private string ExtractSenderFromEmailContent(string emailContent)
        {
            // Intenta extraer el remitente del contenido del correo
            var fromMatch = Regex.Match(emailContent, @"From:[ \t]*([^\r\n]+)", RegexOptions.IgnoreCase);
            return fromMatch.Success ? fromMatch.Groups[1].Value.Trim() : "Remitente desconocido";
        }
        
        private string[] ExtractCcRecipientsFromEmailContent(string emailContent)
        {
            // Intenta extraer los destinatarios en copia del contenido del correo
            var ccMatch = Regex.Match(emailContent, @"Cc:[ \t]*([^\r\n]+)", RegexOptions.IgnoreCase);
            if (ccMatch.Success)
            {
                return ccMatch.Groups[1].Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(cc => cc.Trim()).ToArray();
            }
            return Array.Empty<string>();
        }
        
        private async Task<int> EvaluateEmailImportance(string subject, string sender, string[] ccRecipients)
        {
            var ccList = string.Join(", ", ccRecipients);
            var prompt = $"{_contextPrompt}\n\nEvalúa la importancia de este correo basado solo en estos datos:\n" +
                         $"Asunto: {subject}\nRemitente: {sender}\nCopias: {ccList}\n\n" +
                         "Proporciona solo una puntuación de importancia del 1 al 10 en el formato:\nImportanceScore: [1-10]";
            
            var chatRequest = new ChatRequest()
            {
                Model = "gpt-3.5-turbo",
                Temperature = 0.5,
                MaxTokens = 50, // Menos tokens para una respuesta corta
                Messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Eres un asistente que evalúa la importancia de correos electrónicos basándose en datos básicos."),
                    new ChatMessage(ChatMessageRole.User, prompt)
                }
            };
            
            var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
            var responseText = result.Choices[0].Message.Content;
            
            // Extraer puntuación de importancia
            var importanceMatch = Regex.Match(responseText, @"ImportanceScore:\s*(\d+)");
            if (importanceMatch.Success && int.TryParse(importanceMatch.Groups[1].Value, out int score))
                return score;
            
            return 5; // Valor predeterminado si no se puede determinar
        }
        
        private async Task<EmailAnalysisResult> PerformFullEmailAnalysis(string emailContent, string subject, int initialScore)
        {
            var prompt = $"{_contextPrompt}\n\nAnaliza este correo en detalle:\n{emailContent}\n\n" +
                        "Proporciona un resumen y confirma o ajusta la puntuación de importancia (1-10) en el siguiente formato:\n" +
                        $"Subject: {subject}\nImportanceScore: [1-10]\nSummary: [Resumen breve del contenido del correo]";

            var chatRequest = new ChatRequest()
            {
                Model = "gpt-3.5-turbo",
                Temperature = 0.7,
                MaxTokens = 500,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Eres un asistente que analiza correos electrónicos en detalle."),
                    new ChatMessage(ChatMessageRole.User, prompt)
                }
            };

            var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
            var responseText = result.Choices[0].Message.Content;

            // Analizar la respuesta para extraer Subject, ImportanceScore y Summary
            var emailAnalysis = new EmailAnalysisResult();
            
            // Extraer asunto
            var subjectMatch = Regex.Match(responseText, @"Subject:\s*(.+)");
            if (subjectMatch.Success)
                emailAnalysis.Subject = subjectMatch.Groups[1].Value.Trim();
            else
                emailAnalysis.Subject = subject;
            
            // Extraer puntuación de importancia
            var importanceMatch = Regex.Match(responseText, @"ImportanceScore:\s*(\d+)");
            if (importanceMatch.Success && int.TryParse(importanceMatch.Groups[1].Value, out int score))
                emailAnalysis.ImportanceScore = score;
            else
                emailAnalysis.ImportanceScore = initialScore;
            
            // Extraer resumen
            var summaryMatch = Regex.Match(responseText, @"Summary:\s*(.+)", RegexOptions.Singleline);
            if (summaryMatch.Success)
                emailAnalysis.Summary = summaryMatch.Groups[1].Value.Trim();

            return emailAnalysis;
        }
    }
}
