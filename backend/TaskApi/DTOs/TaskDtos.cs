namespace TaskApi.DTOs;

/// <summary>
/// Represents a request to create a new task.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the task. Default is "Pending".
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the due date of the task.
    /// </summary>
    public DateTime DueDate { get; set; }
}

/// <summary>
/// Represents a request to update the status of an existing task.
/// </summary>
public class UpdateTaskStatusRequest
{
    /// <summary>
    /// Gets or sets the new status of the task.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Represents a request to update an existing task.
/// </summary>
public class UpdateTaskRequest
{
    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the task. Default is "Pending".
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the due date of the task.
    /// </summary>
    public DateTime DueDate { get; set; }
}

/// <summary>
/// Represents the response for a task.
/// </summary>
public class TaskResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the task.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the due date of the task.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the task.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last updated date of the task, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
