using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemberManagementController : ControllerBase
{
    private readonly IMemberManagementService _memberManagementService;

    public MemberManagementController(IMemberManagementService memberManagementService)
    {
        _memberManagementService = memberManagementService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<MemberDto>> RegisterMember([FromBody] CreateMemberDto memberDto)
    {
        try
        {
            var member = await _memberManagementService.RegisterMemberAsync(memberDto);
            return CreatedAtAction(nameof(RegisterMember), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{memberId}/upgrade")]
    public async Task<IActionResult> UpgradeMembership(
        int memberId, 
        [FromBody] UpgradeMembershipDto request)
    {
        try
        {
            await _memberManagementService.UpgradeMembershipAsync(memberId, request.NewMembershipType);
            return Ok(new { message = $"Membership upgraded to {request.NewMembershipType}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{memberId}/renew")]
    public async Task<IActionResult> RenewMembership(int memberId)
    {
        try
        {
            await _memberManagementService.RenewMembershipAsync(memberId);
            return Ok(new { message = "Membership renewed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{memberId}/deactivate")]
    public async Task<IActionResult> DeactivateMember(int memberId)
    {
        try
        {
            await _memberManagementService.DeactivateMemberAsync(memberId);
            return Ok(new { message = "Member deactivated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{memberId}/activate")]
    public async Task<IActionResult> ActivateMember(int memberId)
    {
        try
        {
            await _memberManagementService.ActivateMemberAsync(memberId);
            return Ok(new { message = "Member activated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{memberId}/statistics")]
    public async Task<ActionResult<MemberStatisticsDto>> GetMemberStatistics(int memberId)
    {
        try
        {
            var statistics = await _memberManagementService.GetMemberStatisticsAsync(memberId);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UpgradeMembershipDto
{
    public MembershipType NewMembershipType { get; set; }
}
