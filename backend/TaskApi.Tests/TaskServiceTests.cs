using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskApi.Data;
using TaskApi.DTOs;
using TaskApi.Interfaces;
using TaskApi.Services;
using Xunit;

namespace TaskApi.Tests;

public class TaskServiceTests
{
    private TaskDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TaskDbContext(options);
    }

    private ITaskService CreateService(TaskDbContext context)
    {
        var logger = new Mock<ILogger<TaskService>>().Object;
        return new TaskService(context, logger);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_ReturnsEmptyList_WhenNoTasksExist()
    {
        using var context = GetDbContext();
        var service = CreateService(context);

        var result = await service.GetAllTasksAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_ReturnsAllTasks_WhenTasksExist()
    {
        using var context = GetDbContext();
        context.Tasks.Add(new TaskApi.Models.Task { Title = "Task 1", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) });
        context.Tasks.Add(new TaskApi.Models.Task { Title = "Task 2", Status = "In Progress", DueDate = DateTime.UtcNow.AddDays(2) });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetAllTasksAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ReturnsTask_WhenTaskExists()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetTaskByIdAsync(task.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(task.Title, result.Data!.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        using var context = GetDbContext();
        var service = CreateService(context);

        var result = await service.GetTaskByIdAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.NotFound, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_CreatesTask_WithValidRequest()
    {
        using var context = GetDbContext();
        var service = CreateService(context);
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Test Description",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await service.CreateTaskAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Task", result.Data!.Title);
        Assert.Equal("Test Description", result.Data.Description);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsValidationError_WhenTitleIsEmpty()
    {
        using var context = GetDbContext();
        var service = CreateService(context);
        var request = new CreateTaskRequest
        {
            Title = "",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await service.CreateTaskAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.Validation, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_ReturnsValidationError_WhenDueDateIsDefault()
    {
        using var context = GetDbContext();
        var service = CreateService(context);
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            DueDate = default
        };

        var result = await service.CreateTaskAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.Validation, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_UpdatesStatus_WhenTaskExists()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var request = new UpdateTaskStatusRequest { Status = "Completed" };

        var result = await service.UpdateTaskStatusAsync(task.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Completed", result.Data!.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        using var context = GetDbContext();
        var service = CreateService(context);
        var request = new UpdateTaskStatusRequest { Status = "Completed" };

        var result = await service.UpdateTaskStatusAsync(999, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.NotFound, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskStatus_ReturnsValidationError_WhenStatusIsEmpty()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var request = new UpdateTaskStatusRequest { Status = "" };

        var result = await service.UpdateTaskStatusAsync(task.Id, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.Validation, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_UpdatesTask_WhenRequestIsValid()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = "Pending",
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var result = await service.UpdateTaskAsync(task.Id, request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", result.Data!.Title);
        Assert.Equal("Updated Description", result.Data.Description);
        Assert.Equal("Completed", result.Data.Status);
        Assert.Equal(request.DueDate, result.Data.DueDate);
        Assert.NotNull(result.Data.UpdatedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_ReturnsValidationError_WhenTitleIsEmpty()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var request = new UpdateTaskRequest
        {
            Title = "",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var result = await service.UpdateTaskAsync(task.Id, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.Validation, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        using var context = GetDbContext();
        var service = CreateService(context);
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Status = "Completed",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var result = await service.UpdateTaskAsync(999, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.NotFound, result.Error!.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_DeletesTask_WhenTaskExists()
    {
        using var context = GetDbContext();
        var task = new TaskApi.Models.Task { Title = "Test Task", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.DeleteTaskAsync(task.Id);

        Assert.True(result.IsSuccess);
        var deletedTask = await context.Tasks.FindAsync(task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        using var context = GetDbContext();
        var service = CreateService(context);

        var result = await service.DeleteTaskAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorCodes.NotFound, result.Error!.Code);
    }
}
