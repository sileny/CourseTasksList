using System.Text.Json.Serialization;

namespace CourseTasksList.Models;

/// <summary>Задача без срока сдачи: просрочить её невозможно.</summary>
public sealed class SimpleTask : TaskBase
{
    [JsonIgnore]
    public override bool IsOverdue => false;
}
