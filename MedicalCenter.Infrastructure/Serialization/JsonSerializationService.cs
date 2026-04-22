using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MedicalCenter.Application.Interfaces;

namespace MedicalCenter.Infrastructure.Serialization;

/// <summary>
/// Реализация сериализации на основе System.Text.Json.
/// </summary>
public sealed class JsonSerializationService : ISerializationService
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public string Serialize<T>(T obj)
    {
        if (obj is null) throw new System.ArgumentNullException(nameof(obj));
        return JsonSerializer.Serialize(obj, DefaultOptions);
    }

    public T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new System.ArgumentException("JSON-строка не может быть пустой.", nameof(json));

        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    public async Task SerializeToFileAsync<T>(
        T obj,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (obj is null) throw new System.ArgumentNullException(nameof(obj));
        if (string.IsNullOrWhiteSpace(filePath)) throw new System.ArgumentException("filePath не может быть пустым.", nameof(filePath));

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, obj, DefaultOptions, cancellationToken);
    }

    public async Task<T?> DeserializeFromFileAsync<T>(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new System.ArgumentException("filePath не может быть пустым.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}", filePath);

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(stream, DefaultOptions, cancellationToken);
    }
}
