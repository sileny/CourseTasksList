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

            try
            {
                return JsonSerializer.Deserialize<List<TaskBase>>(json, SerializerOptions)
                       ?? new List<TaskBase>();
            }
            catch (JsonException)
            {
                return ReadLegacy(json);
            }
            catch (NotSupportedException)
            {
                // Файл сохранён до появления видов задач — в нём нет поля "$type".
                return ReadLegacy(json);
            }
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

    /// <summary>Читает файл старого формата: вид задачи определяется по наличию срока.</summary>
    private static List<TaskBase> ReadLegacy(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var tasks = new List<TaskBase>();

            foreach (var element in document.RootElement.EnumerateArray())
                tasks.Add(ReadLegacyTask(element));

            return tasks;
        }
        catch (JsonException)
        {
            return new List<TaskBase>();
        }
    }

    private static TaskBase ReadLegacyTask(JsonElement element)
    {
        var hasDeadline = element.TryGetProperty("Deadline", out var deadlineElement)
                          && deadlineElement.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined);

        TaskBase task = hasDeadline ? new DeadlineTask() : new SimpleTask();

        if (element.TryGetProperty("Id", out var id) && id.TryGetGuid(out var guid))
            task.Id = guid;

        if (element.TryGetProperty("CreatedAt", out var createdAt) && createdAt.TryGetDateTime(out var created))
            task.CreatedAt = created;

        if (element.TryGetProperty("Title", out var title))
            task.Title = title.GetString() ?? string.Empty;

        if (hasDeadline && deadlineElement.TryGetDateTime(out var deadline))
            task.Deadline = deadline;

        if (element.TryGetProperty("Status", out var status)
            && Enum.TryParse<TaskState>(status.GetString(), ignoreCase: true, out var state))
            task.Status = state;

        return task;
    }
}
