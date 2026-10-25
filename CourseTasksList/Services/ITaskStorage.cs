using CourseTasksList.Models;

namespace CourseTasksList.Services;

/// <summary>
/// Хранилище задач. Интерфейс позволяет менять формат хранения
/// (JSON, XML, база данных), не переписывая остальную программу.
/// </summary>
public interface ITaskStorage
{
    /// <summary>Читает все задачи.</summary>
    List<TaskItem> Load();

    /// <summary>Записывает задачи.</summary>
    void Save(IEnumerable<TaskItem> tasks);
}
