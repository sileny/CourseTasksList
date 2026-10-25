using System.Windows.Input;

namespace CourseTasksList.Infrastructure;

/// <summary>
/// Общая часть всех команд: событие о смене доступности и его вызов.
/// Наследники решают, что именно выполнять и когда команда доступна.
/// </summary>
public abstract class RelayCommandBase : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);

    /// <summary>Сообщает интерфейсу, что доступность команды могла измениться.</summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>Команда без параметра — для обычных кнопок.</summary>
public sealed class RelayCommand : RelayCommandBase
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public override void Execute(object? parameter) => _execute();
}

/// <summary>Команда с параметром — например, действие над конкретной строкой задачи.</summary>
public sealed class RelayCommand<T> : RelayCommandBase
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
        => parameter is T value && (_canExecute?.Invoke(value) ?? true);

    public override void Execute(object? parameter)
    {
        if (parameter is T value)
            _execute(value);
    }
}
