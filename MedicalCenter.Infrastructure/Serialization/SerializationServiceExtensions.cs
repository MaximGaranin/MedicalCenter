using Microsoft.Extensions.DependencyInjection;
using MedicalCenter.Application.Interfaces;

namespace MedicalCenter.Infrastructure.Serialization;

/// <summary>
/// Расширения для регистрации сериализации в DI-контейнере.
/// </summary>
public static class SerializationServiceExtensions
{
    /// <summary>
    /// Регистрирует <see cref="ISerializationService"/> (JSON-реализация) в DI.
    /// </summary>
    public static IServiceCollection AddJsonSerialization(this IServiceCollection services)
    {
        services.AddSingleton<ISerializationService, JsonSerializationService>();
        return services;
    }
}
