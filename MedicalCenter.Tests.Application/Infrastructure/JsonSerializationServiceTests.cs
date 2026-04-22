using MedicalCenter.Infrastructure.Serialization;
using NUnit.Framework;

namespace MedicalCenter.Tests.Infrastructure;

[TestFixture]
public class JsonSerializationServiceTests
{
    private JsonSerializationService _service = null!;

    [SetUp]
    public void SetUp() => _service = new JsonSerializationService();

    // ─── Serialize ───────────────────────────────────────────────

    [Test]
    public void Serialize_ValidObject_ReturnsJsonString()
    {
        var dto = new SampleDto { Name = "Иван", Age = 30 };

        var json = _service.Serialize(dto);

        Assert.That(json, Does.Contain("\"name\""));
        Assert.That(json, Does.Contain("\"иван\"").Or.Contain("\"Иван\""));
    }

    [Test]
    public void Serialize_NullObject_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.Serialize<SampleDto>(null!));
    }

    // ─── Deserialize ─────────────────────────────────────────────

    [Test]
    public void Deserialize_ValidJson_ReturnsObject()
    {
        const string json = """{"name":"Иван","age":30}""";

        var result = _service.Deserialize<SampleDto>(json);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Иван"));
        Assert.That(result.Age,  Is.EqualTo(30));
    }

    [Test]
    [TestCase("")]
    [TestCase("   ")]
    public void Deserialize_EmptyJson_ThrowsArgumentException(string emptyJson)
    {
        Assert.Throws<ArgumentException>(() => _service.Deserialize<SampleDto>(emptyJson));
    }

    // ─── Round-trip ───────────────────────────────────────────────

    [Test]
    public void SerializeDeserialize_RoundTrip_ObjectsAreEqual()
    {
        var original = new SampleDto { Name = "Мария", Age = 25 };

        var json   = _service.Serialize(original);
        var result = _service.Deserialize<SampleDto>(json);

        Assert.That(result,       Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(original.Name));
        Assert.That(result.Age,   Is.EqualTo(original.Age));
    }

    // ─── File async ───────────────────────────────────────────────

    [Test]
    public async Task SerializeToFileAsync_ValidObject_CreatesFile()
    {
        var dto      = new SampleDto { Name = "Тест", Age = 1 };
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            await _service.SerializeToFileAsync(dto, filePath);
            Assert.That(File.Exists(filePath), Is.True);
        }
        finally { if (File.Exists(filePath)) File.Delete(filePath); }
    }

    [Test]
    public async Task DeserializeFromFileAsync_ExistingFile_ReturnsObject()
    {
        var original = new SampleDto { Name = "Файл", Age = 99 };
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            await _service.SerializeToFileAsync(original, filePath);
            var result = await _service.DeserializeFromFileAsync<SampleDto>(filePath);

            Assert.That(result,       Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo(original.Name));
            Assert.That(result.Age,   Is.EqualTo(original.Age));
        }
        finally { if (File.Exists(filePath)) File.Delete(filePath); }
    }

    [Test]
    public void DeserializeFromFileAsync_MissingFile_ThrowsFileNotFoundException()
    {
        Assert.ThrowsAsync<FileNotFoundException>(() =>
            _service.DeserializeFromFileAsync<SampleDto>("/nonexistent/path.json"));
    }

    // ─── Вспомогательный класс ────────────────────────────────────
    private sealed class SampleDto
    {
        public string Name { get; init; } = string.Empty;
        public int    Age  { get; init; }
    }
}
