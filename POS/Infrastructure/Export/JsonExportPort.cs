using System.Text.Json;

using POS.Application.Dtos;
using POS.Application.Ports;

namespace POS.Infrastructure.Export;

/// <summary>
/// JSON adapter for IExportPort.
/// </summary>
internal sealed class JsonExportPort : IExportPort
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string Format => "JSON";

    public void Export(IReadOnlyList<FinalScoreDto> rows, string filePath)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var payload = JsonSerializer.Serialize(rows, Options);
        File.WriteAllText(filePath, payload, System.Text.Encoding.UTF8);
    }
}