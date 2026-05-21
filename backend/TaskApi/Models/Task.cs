namespace TaskApi.Models;

/// <summary>
/// Represents a task in the task management system.
/// </summary>
public class Task
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// This property is optional.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of the task.
    /// Default value is "Pending".
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the due date for the task.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// Default value is the current UTC date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the task was last updated.
    /// This property is optional.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
