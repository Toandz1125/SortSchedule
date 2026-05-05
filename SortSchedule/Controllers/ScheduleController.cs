using Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;
using SortSchedule.Application.Abstractions;
using SortSchedule.Application.DTOs.Schedules;
using SortSchedule.Controllers.Common;
using SortSchedule.Domain.Entities;
using SortSchedule.Domain.Enums;

namespace SortSchedule.Controllers;

[ApiController]
[Route("api/schedule")]
public sealed class ScheduleController(
    IScheduleOrchestrator orchestrator,
    IScheduleScenarioRepository scenarioRepository) : BaseController
{
    private readonly IScheduleOrchestrator _orchestrator = orchestrator;
    private readonly IScheduleScenarioRepository _scenarioRepository = scenarioRepository;
    [HasPermission("Schedule", PermissionAction.Manage)]
    [HttpPost("scenarios/{scenarioId}")]
    public async Task<IActionResult> SaveScenario(string scenarioId, [FromBody] ScheduleDto schedule, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_orchestrator.SaveScenarioAsync(scenarioId, schedule.ToDomain(), cancellationToken));
    }

    [HasPermission("Schedule", PermissionAction.Manage)]
    [HttpPost("solve")]
    public async Task<IActionResult> Solve([FromBody] SolveScheduleRequest request, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_orchestrator.SolveAsync(request, cancellationToken));
    }

    [HasPermission("Schedule", PermissionAction.Read)]
    [HttpGet("result/{scenarioId}")]
    public async Task<IActionResult> GetResult(string scenarioId, CancellationToken cancellationToken)
    {
        return await HandleActionAsync(_orchestrator.GetResultAsync(scenarioId, cancellationToken));
    }
}
