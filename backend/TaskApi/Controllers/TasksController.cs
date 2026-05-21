using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.DTOs;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskDbContext _context;
    private readonly ILogger<TasksController> _logger;

    public TasksController(TaskDbContext context, ILogger<TasksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve all tasks
    /// </summary>
    /// <returns>List of all tasks</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAllTasks()
    {
        _logger.LogInformation("Retrieving all tasks");
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Select(task => new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Retrieve a task by ID
    /// </summary>
    /// <param name="id">The task ID</param>
    /// <returns>The task with the specified ID</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetTaskById(int id)
    {
        _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return Ok(MapToTaskResponse(task));
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="request">The task creation request</param>
    /// <returns>The created task</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> CreateTask(CreateTaskRequest request)
    {
        _logger.LogInformation("Creating new task with title: {Title}", request.Title);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Task creation failed: Title is required");
            return BadRequest(new { message = "Title is required" });
        }

        if (request.DueDate == default)
        {
            _logger.LogWarning("Task creation failed: DueDate is required");
            return BadRequest(new { message = "DueDate is required" });
        }

        var task = new Models.Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, MapToTaskResponse(task));
    }

    /// <summary>
    /// Update the status of a task
    /// </summary>
    /// <param name="id">The task ID</param>
    /// <param name="request">The status update request</param>
    /// <returns>The updated task</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> UpdateTaskStatus(int id, UpdateTaskStatusRequest request)
    {
        _logger.LogInformation("Updating status for task ID: {TaskId}", id);

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            _logger.LogWarning("Status update failed: Status is required");
            return BadRequest(new { message = "Status is required" });
        }

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} status updated to {Status}", id, request.Status);
        return Ok(MapToTaskResponse(task));
    }

    /// <summary>
    /// Update a task
    /// </summary>
    /// <param name="id">The task ID</param>
    /// <param name="request">The task update request</param>
    /// <returns>The updated task</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> UpdateTask(int id, UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Task update failed: Title is required");
            return BadRequest(new { message = "Title is required" });
        }

        if (request.DueDate == default)
        {
            _logger.LogWarning("Task update failed: DueDate is required");
            return BadRequest(new { message = "DueDate is required" });
        }

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} updated successfully", id);
        return Ok(MapToTaskResponse(task));
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    /// <param name="id">The task ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTask(int id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} deleted successfully", id);
        return NoContent();
    }

    private static TaskResponse MapToTaskResponse(Models.Task task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
