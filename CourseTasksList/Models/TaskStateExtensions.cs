namespace CourseTasksList.Models;

/// <summary>Подписи состояний для интерфейса.</summary>
public static class TaskStateExtensions
{
    public static string ToRussian(this TaskState state) => state switch
    {
        TaskState.Planned => "Запланировано",
        TaskState.InProgress => "В процессе",
        TaskState.Completed => "Выполнено",
        TaskState.Postponed => "Отложено",
        _ => state.ToString()
    };
}

/// <summary>Пара «значение — подпись» для выпадающего списка состояний.</summary>
public sealed class TaskStateOption
{
    public TaskStateOption(TaskState value, string title)
    {
        Value = value;
        Title = title;
    }

    public TaskState Value { get; }

    public string Title { get; }
}
