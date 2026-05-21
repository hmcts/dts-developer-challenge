using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApi.Controllers;
using TaskApi.DTOs;
using TaskApi.Interfaces;
using TaskApi.Services;
using Xunit;

namespace TaskApi.Tests;

public class TasksControllerTests
{
    private static TasksController CreateController(Mock<ITaskService> taskServiceMock)
    {
        return new TasksController(taskServiceMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_ReturnsOkWithTasks()
    {
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.GetAllTasksAsync())
            .ReturnsAsync(new List<TaskResponse>
            {
                new() { Id = 1, Title = "Task 1", Status = "Pending", DueDate = DateTime.UtcNow },
                new() { Id = 2, Title = "Task 2", Status = "Completed", DueDate = DateTime.UtcNow }
            });

        var controller = CreateController(taskServiceMock);

        var result = await controller.GetAllTasks();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsType<List<TaskResponse>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ReturnsNotFound_WhenServiceReturnsNotFound()
    {
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.GetTaskByIdAsync(999))
            .ReturnsAsync(ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.NotFound, "Task with ID 999 not found"));

        var controller = CreateController(taskServiceMock);

        var result = await controller.GetTaskById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsCreatedAtAction_WhenServiceSucceeds()
    {
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Test Description",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ServiceResult<TaskResponse>.Success(new TaskResponse
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                DueDate = request.DueDate
            }));

        var controller = CreateController(taskServiceMock);

        var result = await controller.CreateTask(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TasksController.GetTaskById), createdResult.ActionName);
        var returnedTask = Assert.IsType<TaskResponse>(createdResult.Value);
        Assert.Equal(request.Title, returnedTask.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsBadRequest_WhenServiceValidationFails()
    {
        var request = new CreateTaskRequest { Title = "", DueDate = DateTime.UtcNow.AddDays(1) };
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.Validation, "Title is required"));

        var controller = CreateController(taskServiceMock);

        var result = await controller.CreateTask(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_ReturnsOk_WhenServiceSucceeds()
    {
        var request = new UpdateTaskStatusRequest { Status = "Completed" };
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.UpdateTaskStatusAsync(1, request))
            .ReturnsAsync(ServiceResult<TaskResponse>.Success(new TaskResponse
            {
                Id = 1,
                Title = "Task 1",
                Status = "Completed",
                DueDate = DateTime.UtcNow
            }));

        var controller = CreateController(taskServiceMock);

        var result = await controller.UpdateTaskStatus(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTask = Assert.IsType<TaskResponse>(okResult.Value);
        Assert.Equal("Completed", returnedTask.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_ReturnsBadRequest_WhenServiceValidationFails()
    {
        var request = new UpdateTaskRequest
        {
            Title = "",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.UpdateTaskAsync(1, request))
            .ReturnsAsync(ServiceResult<TaskResponse>.Failure(ServiceErrorCodes.Validation, "Title is required"));

        var controller = CreateController(taskServiceMock);

        var result = await controller.UpdateTask(1, request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_ReturnsNoContent_WhenServiceSucceeds()
    {
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.DeleteTaskAsync(1))
            .ReturnsAsync(ServiceResult.Success());

        var controller = CreateController(taskServiceMock);

        var result = await controller.DeleteTask(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_ReturnsNotFound_WhenServiceReturnsNotFound()
    {
        var taskServiceMock = new Mock<ITaskService>();
        taskServiceMock
            .Setup(service => service.DeleteTaskAsync(999))
            .ReturnsAsync(ServiceResult.Failure(ServiceErrorCodes.NotFound, "Task with ID 999 not found"));

        var controller = CreateController(taskServiceMock);

        var result = await controller.DeleteTask(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
