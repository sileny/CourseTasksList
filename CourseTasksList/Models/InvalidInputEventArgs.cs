namespace CourseTasksList.Models;

/// <summary>Сообщение о неверно введённом значении.</summary>
public sealed class InvalidInputEventArgs : EventArgs
{
    public InvalidInputEventArgs(string message) => Message = message;

    /// <summary>Текст, который нужно показать пользователю.</summary>
    public string Message { get; }
}
