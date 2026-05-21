using Microsoft.AspNetCore.Mvc;
using TaskApi.DTOs;
using TaskApi.Interfaces;
using TaskApi.Services;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Retrieve all tasks
    /// </summary>
    /// <returns>List of all tasks</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
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
        var result = await _taskService.GetTaskByIdAsync(id);
        return ToActionResult(result);
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
        var result = await _taskService.CreateTaskAsync(request);
        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetTaskById), new { id = result.Data!.Id }, result.Data);
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
        var result = await _taskService.UpdateTaskStatusAsync(id, request);
        return ToActionResult(result);
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
        var result = await _taskService.UpdateTaskAsync(id, request);
        return ToActionResult(result);
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
        var result = await _taskService.DeleteTaskAsync(id);
        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        return NoContent();
    }

    private ActionResult<TaskResponse> ToActionResult(ServiceResult<TaskResponse> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return result.Error?.Code switch
        {
            ServiceErrorCodes.Validation => BadRequest(new { message = result.Error.Message }),
            ServiceErrorCodes.NotFound => NotFound(new { message = result.Error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    private ActionResult ToActionResult(ServiceResult result)
    {
        return result.Error?.Code switch
        {
            ServiceErrorCodes.Validation => BadRequest(new { message = result.Error!.Message }),
            ServiceErrorCodes.NotFound => NotFound(new { message = result.Error!.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
