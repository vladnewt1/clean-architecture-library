using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
    {
        var loans = await _loanService.GetAllAsync();
        return Ok(loans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetById(int id)
    {
        var loan = await _loanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        return Ok(loan);
    }

    [HttpGet("member/{memberId}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByMemberId(int memberId)
    {
        var loans = await _loanService.GetActiveLoansByMemberIdAsync(memberId);
        return Ok(loans);
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByBookId(int bookId)
    {
        var loans = await _loanService.GetLoansByBookIdAsync(bookId);
        return Ok(loans);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdue()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanDto loanDto)
    {
        try
        {
            var loan = await _loanService.CreateLoanAsync(loanDto);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<LoanDto>> ReturnLoan(int id, [FromBody] ReturnLoanDto returnDto)
    {
        try
        {
            returnDto.LoanId = id;
            var loan = await _loanService.ReturnLoanAsync(id, returnDto);
            return Ok(loan);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
