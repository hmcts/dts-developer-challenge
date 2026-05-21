using TaskApi.DTOs;
using TaskApi.Services;

namespace TaskApi.Interfaces;

/// <summary>
/// Interface for task management services.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Retrieves all tasks asynchronously.
    /// </summary>
    /// <returns>A read-only list of task responses.</returns>
    Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync();

    /// <summary>
    /// Retrieves a specific task by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the task to retrieve.</param>
    /// <returns>A service result containing the task response.</returns>
    Task<ServiceResult<TaskResponse>> GetTaskByIdAsync(int id);

    /// <summary>
    /// Creates a new task asynchronously.
    /// </summary>
    /// <param name="request">The request containing the details of the task to create.</param>
    /// <returns>A service result containing the created task response.</returns>
    Task<ServiceResult<TaskResponse>> CreateTaskAsync(CreateTaskRequest request);

    /// <summary>
    /// Updates the status of an existing task asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the task to update.</param>
    /// <param name="request">The request containing the new status for the task.</param>
    /// <returns>A service result containing the updated task response.</returns>
    Task<ServiceResult<TaskResponse>> UpdateTaskStatusAsync(int id, UpdateTaskStatusRequest request);

    /// <summary>
    /// Updates an existing task asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the task to update.</param>
    /// <param name="request">The request containing the updated details of the task.</param>
    /// <returns>A service result containing the updated task response.</returns>
    Task<ServiceResult<TaskResponse>> UpdateTaskAsync(int id, UpdateTaskRequest request);

    /// <summary>
    /// Deletes a task asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the task to delete.</param>
    /// <returns>A service result indicating the success or failure of the deletion.</returns>
    Task<ServiceResult> DeleteTaskAsync(int id);
}
