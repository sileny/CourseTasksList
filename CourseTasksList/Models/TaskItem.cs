using System.Text.Json.Serialization;
using CourseTasksList.Infrastructure;

namespace CourseTasksList.Models;

/// <summary>Одна задача планировщика.</summary>
public sealed class TaskItem : ObservableObject
{
    private string _title = string.Empty;
    private DateTime? _deadline;
    private TaskState _state = TaskState.Planned;
    private bool _isEditing;
    private bool _isNew;

    /// <summary>Уникальный идентификатор задачи.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Дата добавления задачи в планировщик.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Today;

    /// <summary>Название задачи.</summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>Срок сдачи (может быть не указан).</summary>
    public DateTime? Deadline
    {
        get => _deadline;
        set => SetProperty(ref _deadline, value);
    }

    /// <summary>Состояние задачи.</summary>
    public TaskState Status
    {
        get => _state;
        set
        {
            if (SetProperty(ref _state, value))
                OnPropertyChanged(nameof(StatusTitle));
        }
    }

    /// <summary>Подпись состояния для отображения в таблице. Вычисляется на лету.</summary>
    [JsonIgnore]
    public string StatusTitle => _state.ToRussian();

    /// <summary>Строка сейчас редактируется. В файл не сохраняется.</summary>
    [JsonIgnore]
    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (SetProperty(ref _isEditing, value))
                OnPropertyChanged(nameof(IsNotEditing));
        }
    }

    /// <summary>Задача ещё не добавлена в планировщик (строка в режиме создания).</summary>
    [JsonIgnore]
    public bool IsNew
    {
        get => _isNew;
        set
        {
            if (SetProperty(ref _isNew, value))
                OnPropertyChanged(nameof(IsExisting));
        }
    }

    /// <summary>Задача уже добавлена в планировщик.</summary>
    [JsonIgnore]
    public bool IsExisting => !_isNew;

    /// <summary>Строка не редактируется.</summary>
    [JsonIgnore]
    public bool IsNotEditing => !_isEditing;
}
