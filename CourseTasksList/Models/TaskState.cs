namespace CourseTasksList.Models;

/// <summary>Состояние задачи в планировщике.</summary>
public enum TaskState
{
    /// <summary>Запланировано — задача добавлена, но ещё не начата.</summary>
    Planned,

    /// <summary>В процессе — работа идёт.</summary>
    InProgress,

    /// <summary>Выполнено — задача закрыта.</summary>
    Completed,

    /// <summary>Отложено — работа приостановлена.</summary>
    Postponed
}
