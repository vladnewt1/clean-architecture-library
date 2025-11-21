using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Demo controller to demonstrate AutoMapper with DTO pattern (ПР6)
/// Shows Entity to DTO mapping and DTO to Entity mapping
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AutoMapperDemoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AutoMapperDemoController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get AutoMapper pattern information
    /// </summary>
    [HttpGet("info")]
    public ActionResult<object> GetInfo()
    {
        return Ok(new
        {
            title = "AutoMapper + DTO Pattern (ПР6)",
            description = "Використання DTO для API з AutoMapper для маппінгу між Entity та DTO",
            patterns = new[]
            {
                "DTO (Data Transfer Object) - об'єкти для передачі даних через API",
                "AutoMapper - автоматичний маппінг між Entity та DTO"
            },
            profiles = new[]
            {
                "BookProfile - маппінг Book <-> BookDto",
                "MemberProfile - маппінг Member <-> MemberDto",
                "LoanProfile - маппінг Loan <-> LoanDto"
            },
            benefits = new[]
            {
                "Розділення domain моделей та API контрактів",
                "Автоматична конвертація між об'єктами",
                "Зменшення boilerplate коду",
                "Контроль експозиції даних через API",
                "Валідація та трансформація даних"
            },
            endpoints = new
            {
                info = "GET /api/automapperdemo/info",
                getBooksWithDto = "GET /api/automapperdemo/books",
                getBookByIdWithDto = "GET /api/automapperdemo/books/{id}",
                createBookWithDto = "POST /api/automapperdemo/books",
                getMembersWithDto = "GET /api/automapperdemo/members",
                createMemberWithDto = "POST /api/automapperdemo/members",
                getLoansWithDto = "GET /api/automapperdemo/loans"
            }
        });
    }

    /// <summary>
    /// Get all books mapped to BookDto using AutoMapper
    /// </summary>
    [HttpGet("books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        var books = await _unitOfWork.Books.GetAllAsync();
        
        // AutoMapper: Entity -> DTO
        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
        
        return Ok(new
        {
            mapping = "Book Entity -> BookDto",
            mapper = "AutoMapper",
            profile = "BookProfile",
            count = bookDtos.Count(),
            books = bookDtos
        });
    }

    /// <summary>
    /// Get book by ID mapped to BookDto
    /// </summary>
    [HttpGet("books/{id}")]
    public async Task<ActionResult<BookDto>> GetBookById(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        
        if (book == null)
        {
            return NotFound(new { message = $"Book with ID {id} not found" });
        }
        
        // AutoMapper: Entity -> DTO
        var bookDto = _mapper.Map<BookDto>(book);
        
        return Ok(new
        {
            mapping = "Book Entity -> BookDto",
            mapper = "AutoMapper",
            profile = "BookProfile",
            book = bookDto
        });
    }

    /// <summary>
    /// Create a new book using CreateBookDto and AutoMapper
    /// </summary>
    [HttpPost("books")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        // AutoMapper: DTO -> Entity
        // Note: Для коректного маппінгу використовуємо фабричний метод Book.Create
        var book = Book.Create(
            createBookDto.Title,
            createBookDto.Author,
            createBookDto.ISBN,
            createBookDto.PublishedYear,
            createBookDto.Publisher,
            createBookDto.Category,
            createBookDto.Description,
            createBookDto.PageCount,
            createBookDto.Language,
            createBookDto.TotalCopies,
            createBookDto.Price,
            createBookDto.CoverImageUrl
        );
        
        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();
        
        // AutoMapper: Entity -> DTO
        var bookDto = _mapper.Map<BookDto>(book);
        
        return CreatedAtAction(
            nameof(GetBookById),
            new { id = book.Id },
            new
            {
                message = "Book created successfully using DTO",
                mappings = new[]
                {
                    "CreateBookDto -> Book Entity (using Book.Create factory method)",
                    "Book Entity -> BookDto (using AutoMapper)"
                },
                book = bookDto
            });
    }

    /// <summary>
    /// Get all members mapped to MemberDto using AutoMapper
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        
        // AutoMapper: Entity -> DTO (включаючи Address -> AddressDto)
        var memberDtos = _mapper.Map<IEnumerable<MemberDto>>(members);
        
        return Ok(new
        {
            mapping = "Member Entity -> MemberDto (including Address -> AddressDto)",
            mapper = "AutoMapper",
            profile = "MemberProfile",
            count = memberDtos.Count(),
            members = memberDtos
        });
    }

    /// <summary>
    /// Create a new member using CreateMemberDto and AutoMapper
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto createMemberDto)
    {
        // AutoMapper: DTO -> Entity (використовує Member.Create фабричний метод)
        var member = _mapper.Map<Member>(createMemberDto);
        
        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();
        
        // AutoMapper: Entity -> DTO
        var memberDto = _mapper.Map<MemberDto>(member);
        
        return CreatedAtAction(
            "GetMemberById",
            "Members",
            new { id = member.Id },
            new
            {
                message = "Member created successfully using DTO + AutoMapper",
                mappings = new[]
                {
                    "CreateMemberDto -> Member Entity (using AutoMapper with Member.Create)",
                    "AddressDto -> Address ValueObject (using AutoMapper with Address.Create)",
                    "Member Entity -> MemberDto (using AutoMapper)"
                },
                member = memberDto
            });
    }

    /// <summary>
    /// Get all loans mapped to LoanDto using AutoMapper
    /// </summary>
    [HttpGet("loans")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
    {
        var loans = await _unitOfWork.Loans.GetAllAsync();
        
        // AutoMapper: Entity -> DTO (включаючи Book.Title та Member.FullName)
        var loanDtos = _mapper.Map<IEnumerable<LoanDto>>(loans);
        
        return Ok(new
        {
            mapping = "Loan Entity -> LoanDto (including navigation properties)",
            mapper = "AutoMapper",
            profile = "LoanProfile",
            navigationProperties = new[]
            {
                "Book.Title -> LoanDto.BookTitle",
                "Member.FullName -> LoanDto.MemberName"
            },
            count = loanDtos.Count(),
            loans = loanDtos
        });
    }

    /// <summary>
    /// Get member statistics using AutoMapper
    /// </summary>
    [HttpGet("members/statistics")]
    public async Task<ActionResult<IEnumerable<MemberStatisticsDto>>> GetMemberStatistics()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        
        // AutoMapper: Entity -> DTO з обчисленнями
        var statisticsDtos = _mapper.Map<IEnumerable<MemberStatisticsDto>>(members);
        
        return Ok(new
        {
            mapping = "Member Entity -> MemberStatisticsDto (with calculated properties)",
            mapper = "AutoMapper",
            profile = "MemberProfile",
            calculatedProperties = new[]
            {
                "ActiveLoansCount - calculated from Member.GetActiveLoansCount()",
                "TotalLoansCount - calculated from Member.Loans.Count"
            },
            count = statisticsDtos.Count(),
            statistics = statisticsDtos
        });
    }

    /// <summary>
    /// Demo: Complex mapping with transaction
    /// Creates member and loan using DTOs and AutoMapper
    /// </summary>
    [HttpPost("complex-demo")]
    public async Task<ActionResult<object>> ComplexDemo([FromBody] ComplexDemoRequest request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 1. CreateMemberDto -> Member Entity (using AutoMapper)
            var member = _mapper.Map<Member>(request.CreateMember);
            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            // 2. Get book
            var book = await _unitOfWork.Books.GetByIdAsync(request.BookId);
            if (book == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return NotFound(new { message = $"Book with ID {request.BookId} not found" });
            }

            // 3. Create loan
            var loan = Loan.Create(book.Id, member.Id, "Created via AutoMapper demo");
            await _unitOfWork.Loans.AddAsync(loan);
            
            book.BorrowCopy(member.Id);
            await _unitOfWork.Books.UpdateAsync(book);

            await _unitOfWork.CommitTransactionAsync();

            // Map all to DTOs
            var memberDto = _mapper.Map<MemberDto>(member);
            var bookDto = _mapper.Map<BookDto>(book);
            var loanDto = _mapper.Map<LoanDto>(loan);

            return Ok(new
            {
                message = "Complex operation completed using DTOs + AutoMapper",
                mappings = new[]
                {
                    "CreateMemberDto -> Member Entity (AutoMapper)",
                    "Member Entity -> MemberDto (AutoMapper)",
                    "Book Entity -> BookDto (AutoMapper)",
                    "Loan Entity -> LoanDto (AutoMapper)"
                },
                result = new
                {
                    member = memberDto,
                    book = bookDto,
                    loan = loanDto
                }
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return StatusCode(500, new { message = "Transaction failed", error = ex.Message });
        }
    }
}

// DTOs for complex demo
public class ComplexDemoRequest
{
    public CreateMemberDto CreateMember { get; set; } = new();
    public int BookId { get; set; }
}
