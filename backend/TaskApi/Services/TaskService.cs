using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.DTOs;
using TaskApi.Interfaces;

namespace TaskApi.Services;

public class TaskService : ITaskService
{
    private readonly TaskDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(TaskDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync()
    {
        _logger.LogInformation("Retrieving all tasks");

        return await _context.Tasks
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
    }

    public async Task<ServiceResult<TaskResponse>> GetTaskByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.NotFound, $"Task with ID {id} not found");
        }

        return ServiceResult<TaskResponse>.Success(MapToTaskResponse(task));
    }

    public async Task<ServiceResult<TaskResponse>> CreateTaskAsync(CreateTaskRequest request)
    {
        _logger.LogInformation("Creating new task with title: {Title}", request.Title);

        var validationResult = ValidateCreateOrUpdateRequest(request.Title, request.DueDate, "creation");
        if (!validationResult.IsSuccess)
        {
            return ServiceResult<TaskResponse>.Failure(validationResult.Error!.Code, validationResult.Error.Message);
        }

        var task = new Models.Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = NormalizeStatus(request.Status),
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);
        return ServiceResult<TaskResponse>.Success(MapToTaskResponse(task));
    }

    public async Task<ServiceResult<TaskResponse>> UpdateTaskStatusAsync(int id, UpdateTaskStatusRequest request)
    {
        _logger.LogInformation("Updating status for task ID: {TaskId}", id);

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            _logger.LogWarning("Status update failed: Status is required");
            return ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.Validation, "Status is required");
        }

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.NotFound, $"Task with ID {id} not found");
        }

        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} status updated to {Status}", id, request.Status);
        return ServiceResult<TaskResponse>.Success(MapToTaskResponse(task));
    }

    public async Task<ServiceResult<TaskResponse>> UpdateTaskAsync(int id, UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);

        var validationResult = ValidateCreateOrUpdateRequest(request.Title, request.DueDate, "update");
        if (!validationResult.IsSuccess)
        {
            return ServiceResult<TaskResponse>.Failure(validationResult.Error!.Code, validationResult.Error.Message);
        }

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.NotFound, $"Task with ID {id} not found");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = NormalizeStatus(request.Status);
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} updated successfully", id);
        return ServiceResult<TaskResponse>.Success(MapToTaskResponse(task));
    }

    public async Task<ServiceResult> DeleteTaskAsync(int id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);

        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return ServiceResult.Failure(ServiceErrorCodes.NotFound, $"Task with ID {id} not found");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} deleted successfully", id);
        return ServiceResult.Success();
    }

    private ServiceResult ValidateCreateOrUpdateRequest(string title, DateTime dueDate, string operationName)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Task {Operation} failed: Title is required", operationName);
            return ServiceResult.Failure(ServiceErrorCodes.Validation, "Title is required");
        }

        if (dueDate == default)
        {
            _logger.LogWarning("Task {Operation} failed: DueDate is required", operationName);
            return ServiceResult.Failure(ServiceErrorCodes.Validation, "DueDate is required");
        }

        return ServiceResult.Success();
    }

    private static string NormalizeStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? "Pending" : status;

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
