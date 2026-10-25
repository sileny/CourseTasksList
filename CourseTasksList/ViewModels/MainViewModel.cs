using System.Collections.ObjectModel;
using CourseTasksList.Infrastructure;
using CourseTasksList.Models;
using CourseTasksList.Services;

namespace CourseTasksList.ViewModels;

/// <summary>Логика главного окна: список задач, добавление, редактирование, сохранение.</summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly ITaskStorage _storage;

    public MainViewModel(ITaskStorage storage)
    {
        _storage = storage;
        Tasks = new ObservableCollection<TaskBase>(_storage.Load());

        StatusOptions = Enum.GetValues<TaskState>()
            .Select(state => new TaskStateOption(state, state.ToRussian()))
            .ToList();

        AddTaskCommand = new RelayCommand(AddTask);
        CommitTaskCommand = new RelayCommand<TaskBase>(CommitTask);
        EditTaskCommand = new RelayCommand<TaskBase>(EditTask);
        DeleteTaskCommand = new RelayCommand<TaskBase>(DeleteTask);
        SaveAndExitCommand = new RelayCommand(SaveAndExit);

        foreach (var task in Tasks)
            task.InvalidInputEntered += OnTaskInvalidInput;
    }

    /// <summary>Задачи, отображаемые в таблице.</summary>
    public ObservableCollection<TaskBase> Tasks { get; }

    /// <summary>Доступные состояния для выпадающего списка.</summary>
    public IReadOnlyList<TaskStateOption> StatusOptions { get; }

    public RelayCommand AddTaskCommand { get; }

    public RelayCommand<TaskBase> CommitTaskCommand { get; }

    public RelayCommand<TaskBase> EditTaskCommand { get; }

    public RelayCommand<TaskBase> DeleteTaskCommand { get; }

    public RelayCommand SaveAndExitCommand { get; }

    /// <summary>Событие: данные сохранены, окно можно закрывать.</summary>
    public event EventHandler? SaveAndExitRequested;

    /// <summary>Событие: нужно показать пользователю сообщение.</summary>
    public event EventHandler<InvalidInputEventArgs>? NotificationRequested;

    private void AddTask()
    {
        // Новая строка — простая задача. Если пользователь укажет срок,
        // при подтверждении она превратится в задачу со сроком.
        var task = new SimpleTask
        {
            CreatedAt = DateTime.Today,
            Status = TaskState.Planned,
            IsNew = true,
            IsEditing = true
        };

        task.InvalidInputEntered += OnTaskInvalidInput;
        Tasks.Insert(0, task);
    }

    private void CommitTask(TaskBase task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            return;

        // Вид задачи зависит от того, указан ли срок, поэтому после
        // редактирования строка может сменить тип (простая ↔ со сроком).
        var committed = WithCorrectType(task);

        if (!ReferenceEquals(committed, task))
        {
            task.InvalidInputEntered -= OnTaskInvalidInput;
            committed.InvalidInputEntered += OnTaskInvalidInput;

            var index = Tasks.IndexOf(task);
            if (index >= 0)
                Tasks[index] = committed;
        }

        committed.Title = committed.Title.Trim();
        committed.IsNew = false;
        committed.IsEditing = false;
    }

    private void EditTask(TaskBase task) => task.IsEditing = true;

    private void DeleteTask(TaskBase task)
    {
        task.InvalidInputEntered -= OnTaskInvalidInput;
        Tasks.Remove(task);
    }

    private void SaveAndExit()
    {
        // Строки, которые так и не подтвердили кнопкой «Д», в файл не попадают.
        var tasksToSave = Tasks.Where(task => !task.IsNew).ToList();
        _storage.Save(tasksToSave);

        SaveAndExitRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnTaskInvalidInput(object? sender, InvalidInputEventArgs e)
        => NotificationRequested?.Invoke(this, e);

    /// <summary>
    /// Возвращает задачу нужного вида: со сроком — <see cref="DeadlineTask"/>,
    /// без срока — <see cref="SimpleTask"/>. Если вид уже верный, возвращает тот же объект.
    /// </summary>
    private static TaskBase WithCorrectType(TaskBase task)
    {
        var needsDeadlineType = task.Deadline is not null;
        if (needsDeadlineType == task is DeadlineTask)
            return task;

        TaskBase copy = needsDeadlineType ? new DeadlineTask() : new SimpleTask();
        copy.Id = task.Id;
        copy.CreatedAt = task.CreatedAt;
        copy.Title = task.Title;
        copy.Deadline = task.Deadline;
        copy.Status = task.Status;

        return copy;
    }
}
