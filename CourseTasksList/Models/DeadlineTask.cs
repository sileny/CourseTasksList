using System.Text.Json.Serialization;

namespace CourseTasksList.Models;

/// <summary>Задача со сроком сдачи: умеет определять, что срок прошёл.</summary>
public sealed class DeadlineTask : TaskBase
{
    [JsonIgnore]
    public override bool IsOverdue =>
        Deadline is { } deadline && deadline.Date < DateTime.Today && Status != TaskState.Completed;
}
