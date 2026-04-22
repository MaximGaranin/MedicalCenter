using System.Threading;
using System.Threading.Tasks;

namespace MedicalCenter.Application.Interfaces;

/// <summary>
/// Сервис сериализации/десериализации объектов.
/// </summary>
public interface ISerializationService
{
    /// <summary>Сериализует объект в строку JSON.</summary>
    string Serialize<T>(T obj);

    /// <summary>Десериализует строку JSON в объект типа T.</summary>
    T? Deserialize<T>(string json);

    /// <summary>Сериализует объект в файл.</summary>
    Task SerializeToFileAsync<T>(T obj, string filePath, CancellationToken cancellationToken = default);

    /// <summary>Десериализует объект из файла.</summary>
    Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default);
}
