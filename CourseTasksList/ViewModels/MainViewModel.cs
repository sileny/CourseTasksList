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
        Tasks = new ObservableCollection<TaskItem>(_storage.Load());

        StatusOptions = Enum.GetValues<TaskState>()
            .Select(state => new TaskStateOption(state, state.ToRussian()))
            .ToList();

        AddTaskCommand = new RelayCommand(AddTask);
        CommitTaskCommand = new RelayCommand<TaskItem>(CommitTask);
        EditTaskCommand = new RelayCommand<TaskItem>(EditTask);
        DeleteTaskCommand = new RelayCommand<TaskItem>(DeleteTask);
        SaveAndExitCommand = new RelayCommand(SaveAndExit);
    }

    /// <summary>Задачи, отображаемые в таблице.</summary>
    public ObservableCollection<TaskItem> Tasks { get; }

    /// <summary>Доступные состояния для выпадающего списка.</summary>
    public IReadOnlyList<TaskStateOption> StatusOptions { get; }

    public RelayCommand AddTaskCommand { get; }

    public RelayCommand<TaskItem> CommitTaskCommand { get; }

    public RelayCommand<TaskItem> EditTaskCommand { get; }

    public RelayCommand<TaskItem> DeleteTaskCommand { get; }

    public RelayCommand SaveAndExitCommand { get; }

    /// <summary>Событие: данные сохранены, окно можно закрывать.</summary>
    public event EventHandler? SaveAndExitRequested;

    private void AddTask()
    {
        var task = new TaskItem
        {
            CreatedAt = DateTime.Today,
            Status = TaskState.Planned,
            IsNew = true,
            IsEditing = true
        };

        Tasks.Insert(0, task);
    }

    private void CommitTask(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            return;

        task.Title = task.Title.Trim();
        task.IsNew = false;
        task.IsEditing = false;
    }

    private void EditTask(TaskItem task) => task.IsEditing = true;

    private void DeleteTask(TaskItem task) => Tasks.Remove(task);

    private void SaveAndExit()
    {
        // Строки, которые так и не подтвердили кнопкой «Д», в файл не попадают.
        var tasksToSave = Tasks.Where(task => !task.IsNew).ToList();
        _storage.Save(tasksToSave);

        SaveAndExitRequested?.Invoke(this, EventArgs.Empty);
    }
}
