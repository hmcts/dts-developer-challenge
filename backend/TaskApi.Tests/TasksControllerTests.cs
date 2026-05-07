using Microsoft.EntityFrameworkCore;
using TaskApi.Controllers;
using TaskApi.Data;
using TaskApi.DTOs;
using TaskApi.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace TaskApi.Tests;

public class TasksControllerTests
{
    private TaskDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TaskDbContext(options);
    }

    private ILogger<TasksController> GetLogger()
    {
        return new Mock<ILogger<TasksController>>().Object;
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.GetAllTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsType<List<TaskResponse>>(okResult.Value);
        Assert.Empty(returnedTasks);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_ReturnsAllTasks_WhenTasksExist()
    {
        // Arrange
        using var context = GetDbContext();
        var task1 = new TaskApi.Models.Task { Title = "Task 1", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        var task2 = new TaskApi.Models.Task { Title = "Task 2", Status = "In Progress", DueDate = DateTime.UtcNow.AddDays(2) };
        context.Tasks.Add(task1);
        context.Tasks.Add(task2);
        await context.SaveChangesAsync();

        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.GetAllTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsType<List<TaskResponse>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ReturnsTask_WhenTaskExists()
    {
        // Arrange
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.GetTaskById(task.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTask = Assert.IsType<TaskResponse>(okResult.Value);
        Assert.Equal(task.Title, returnedTask.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.GetTaskById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_CreatesTask_WithValidRequest()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Test Description",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TasksController.GetTaskById), createdResult.ActionName);
        
        var returnedTask = Assert.IsType<TaskResponse>(createdResult.Value);
        Assert.Equal("New Task", returnedTask.Title);
        Assert.Equal("Test Description", returnedTask.Description);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());
        var request = new CreateTaskRequest
        {
            Title = "",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsBadRequest_WhenDueDateIsDefault()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            DueDate = default
        };

        // Act
        var result = await controller.CreateTask(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_UpdatesStatus_WhenTaskExists()
    {
        // Arrange
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = new TasksController(context, GetLogger());
        var request = new UpdateTaskStatusRequest { Status = "Completed" };

        // Act
        var result = await controller.UpdateTaskStatus(task.Id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTask = Assert.IsType<TaskResponse>(okResult.Value);
        Assert.Equal("Completed", returnedTask.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());
        var request = new UpdateTaskStatusRequest { Status = "Completed" };

        // Act
        var result = await controller.UpdateTaskStatus(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_ReturnsBadRequest_WhenStatusIsEmpty()
    {
        // Arrange
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = new TasksController(context, GetLogger());
        var request = new UpdateTaskStatusRequest { Status = "" };

        // Act
        var result = await controller.UpdateTaskStatus(task.Id, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_DeletesTask_WhenTaskExists()
    {
        // Arrange
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.DeleteTask(task.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        
        // Verify task is deleted
        var deletedTask = await context.Tasks.FindAsync(task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new TasksController(context, GetLogger());

        // Act
        var result = await controller.DeleteTask(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
