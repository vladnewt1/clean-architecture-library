using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<MemberDto>> GetByEmail(string email)
    {
        var member = await _memberService.GetByEmailAsync(email);
        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetActive()
    {
        var members = await _memberService.GetActiveMembersAsync();
        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto memberDto)
    {
        try
        {
            var member = await _memberService.CreateAsync(memberDto);
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMemberDto memberDto)
    {
        try
        {
            await _memberService.UpdateAsync(id, memberDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _memberService.DeleteAsync(id);
        return NoContent();
    }
}
