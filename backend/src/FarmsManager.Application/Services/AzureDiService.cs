using System.Reflection;
using Azure;
using Azure.AI.DocumentIntelligence;
using FarmsManager.Application.Interfaces;
using FarmsManager.Shared.Attributes;
using FarmsManager.Shared.Extensions;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.Application.Services;

public class AzureDiService : IAzureDiService
{
    private readonly DocumentIntelligenceClient _client;
    private const string ModelId = "prebuilt-invoice";

    public AzureDiService(IConfiguration config)
    {
        var endpoint = config.GetValue<string>("AzureDi:Endpoint");
        var apiKey = config.GetValue<string>("AzureDi:ApiKey");
        var credential = new AzureKeyCredential(apiKey);
        _client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
    }

    public async Task<T> AnalyzeInvoiceAsync<T>(string preSignedUrl) where T : class, new()
    {
        var options = new AnalyzeDocumentOptions(ModelId, new Uri(preSignedUrl));
        options.Features.Add("queryFields");

        var customQueryFields = GetCustomQueryFields<T>();
        foreach (var field in customQueryFields)
        {
            options.QueryFields.Add(field);
        }

        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, options);
        var result = operation.Value;

        if (!result.Documents.Any())
            throw new Exception("No documents detected in analysis.");

        var documentFields = result.Documents[0].Fields;
        if (documentFields == null)
            throw new Exception("No documents detected in analysis.");

        return documentFields.MapFromAzureDiFields<T>();
    }


    private static List<string> GetCustomQueryFields<T>()
    {
        return typeof(T)
            .GetProperties()
            .Select(p => p.GetCustomAttribute<AzureDiFieldAttribute>())
            .Where(attr => attr is { CustomField: true })
            .Select(attr => attr.FieldName)
            .Distinct()
            .ToList();
    }
}