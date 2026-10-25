using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CourseTasksList.Models;

namespace CourseTasksList.Services;

/// <summary>Хранит задачи в JSON-файле рядом с исполняемым файлом программы.</summary>
public sealed class JsonTaskStorage : ITaskStorage
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;

    public JsonTaskStorage(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "tasks.json");
    }

    public string FilePath => _filePath;

    public List<TaskBase> Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<TaskBase>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskBase>>(json, SerializerOptions)
                   ?? new List<TaskBase>();
        }
        catch (JsonException)
        {
            return new List<TaskBase>();
        }
        catch (IOException)
        {
            return new List<TaskBase>();
        }
    }

    public void Save(IEnumerable<TaskBase> tasks)
    {
        var json = JsonSerializer.Serialize(tasks.ToList(), SerializerOptions);
        File.WriteAllText(_filePath, json);
    }
}
