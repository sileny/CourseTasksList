using System.Globalization;
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

    /// <summary>Возникает, когда пользователь ввёл недопустимое значение в поле задачи.</summary>
    public event EventHandler<InvalidInputEventArgs>? InvalidInputEntered;

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
        set
        {
            if (SetProperty(ref _deadline, value))
                OnPropertyChanged(nameof(DeadlineText));
        }
    }

    /// <summary>
    /// Срок сдачи в виде текста дд.мм.гггг — для поля ввода.
    /// При неверном формате поле возвращается к прежнему значению и сообщается об ошибке.
    /// </summary>
    [JsonIgnore]
    public string DeadlineText
    {
        get => _deadline?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? string.Empty;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Deadline = null;
                return;
            }

            string[] formats = ["dd.MM.yyyy", "d.M.yyyy", "dd.MM.yy", "d.M.yy"];
            if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
            {
                Deadline = date;
                return;
            }

            InvalidInputEntered?.Invoke(this, new InvalidInputEventArgs(
                "Введи дату в формате ДД.ММ.ГГГГ исключительно цифрами"));

            // Возвращаем в поле последнее корректное значение.
            OnPropertyChanged(nameof(DeadlineText));
        }
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
