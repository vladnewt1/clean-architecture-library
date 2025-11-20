using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("report")]
    public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
    {
        try
        {
            var report = await _inventoryService.GetInventoryReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetLowStockBooks([FromQuery] int threshold = 2)
    {
        try
        {
            var books = await _inventoryService.GetLowStockBooksAsync(threshold);
            return Ok(books);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("out-of-stock")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetOutOfStockBooks()
    {
        try
        {
            var books = await _inventoryService.GetOutOfStockBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{bookId}/add-copies")]
    public async Task<IActionResult> AddBookCopies(
        int bookId, 
        [FromBody] AddCopiesDto request)
    {
        try
        {
            await _inventoryService.AddBookCopiesAsync(bookId, request.Quantity, request.Reason);
            return Ok(new { message = $"Successfully added {request.Quantity} copies" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{bookId}/remove-copies")]
    public async Task<IActionResult> RemoveBookCopies(
        int bookId, 
        [FromBody] RemoveCopiesDto request)
    {
        try
        {
            await _inventoryService.RemoveBookCopiesAsync(bookId, request.Quantity, request.Reason);
            return Ok(new { message = $"Successfully removed {request.Quantity} copies" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class AddCopiesDto
{
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RemoveCopiesDto
{
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
