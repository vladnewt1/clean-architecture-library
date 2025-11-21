using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Demo controller to demonstrate usage of generic IRepository&lt;T&gt; pattern
/// Uses IRepository&lt;Member&gt; to perform CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GenericRepositoryController : ControllerBase
{
    private readonly IRepository<Member> _genericRepository;

    public GenericRepositoryController(IRepository<Member> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    /// <summary>
    /// Get all members using generic repository
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllMembers()
    {
        var members = await _genericRepository.GetAllAsync();
        
        var result = members.Select(m => new
        {
            id = m.Id,
            firstName = m.FirstName,
            lastName = m.LastName,
            email = m.Email,
            phoneNumber = m.PhoneNumber,
            membershipType = m.MembershipType.ToString(),
            membershipDate = m.MembershipDate,
            isActive = m.IsActive,
            note = "Retrieved using IRepository<Member>.GetAllAsync()"
        });

        return Ok(result);
    }

    /// <summary>
    /// Get member by ID using generic repository
    /// </summary>
    [HttpGet("members/{id}")]
    public async Task<ActionResult<object>> GetMemberById(int id)
    {
        var member = await _genericRepository.GetByIdAsync(id);
        
        if (member == null)
        {
            return NotFound(new { message = $"Member with ID {id} not found" });
        }

        return Ok(new
        {
            id = member.Id,
            firstName = member.FirstName,
            lastName = member.LastName,
            email = member.Email,
            phoneNumber = member.PhoneNumber,
            address = new
            {
                street = member.Address.Street,
                city = member.Address.City,
                zipCode = member.Address.ZipCode,
                country = member.Address.Country
            },
            membershipType = member.MembershipType.ToString(),
            membershipDate = member.MembershipDate,
            isActive = member.IsActive,
            note = $"Retrieved using IRepository<Member>.GetByIdAsync({id})"
        });
    }

    /// <summary>
    /// Add new member using generic repository
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<object>> AddMember([FromBody] CreateMemberRequest request)
    {
        var address = Address.Create(
            request.Address.Street,
            request.Address.City,
            request.Address.State ?? "",
            request.Address.ZipCode,
            request.Address.Country
        );

        var member = Member.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.DateOfBirth,
            address,
            request.MembershipType
        );

        var addedMember = await _genericRepository.AddAsync(member);

        return CreatedAtAction(
            nameof(GetMemberById),
            new { id = addedMember.Id },
            new
            {
                id = addedMember.Id,
                firstName = addedMember.FirstName,
                lastName = addedMember.LastName,
                email = addedMember.Email,
                note = "Created using IRepository<Member>.AddAsync()"
            });
    }

    /// <summary>
    /// Update member using generic repository
    /// </summary>
    [HttpPut("members/{id}")]
    public async Task<ActionResult> UpdateMember(int id, [FromBody] UpdateMemberRequest request)
    {
        var member = await _genericRepository.GetByIdAsync(id);
        
        if (member == null)
        {
            return NotFound(new { message = $"Member with ID {id} not found" });
        }

        // Update member using UpdatePersonalInfo method
        member.UpdatePersonalInfo(
            request.FirstName ?? member.FirstName,
            request.LastName ?? member.LastName,
            request.Email ?? member.Email,
            request.PhoneNumber ?? member.PhoneNumber,
            member.DateOfBirth,
            member.Address
        );

        await _genericRepository.UpdateAsync(member);

        return Ok(new
        {
            message = "Member updated successfully",
            id = member.Id,
            note = "Updated using IRepository<Member>.UpdateAsync()"
        });
    }

    /// <summary>
    /// Delete member using generic repository
    /// </summary>
    [HttpDelete("members/{id}")]
    public async Task<ActionResult> DeleteMember(int id)
    {
        var member = await _genericRepository.GetByIdAsync(id);
        
        if (member == null)
        {
            return NotFound(new { message = $"Member with ID {id} not found" });
        }

        await _genericRepository.DeleteAsync(id);

        return Ok(new
        {
            message = $"Member {id} deleted successfully",
            note = "Deleted using IRepository<Member>.DeleteAsync()"
        });
    }

    /// <summary>
    /// Get repository info
    /// </summary>
    [HttpGet("info")]
    public ActionResult<object> GetRepositoryInfo()
    {
        return Ok(new
        {
            title = "Generic Repository Pattern (ПР5)",
            description = "Демонстрація використання IRepository<T> для моделі Member",
            repositoryType = "IRepository<Member>",
            implementationType = "Repository<Member>",
            availableMethods = new[]
            {
                "GetByIdAsync(int id) - Get entity by ID",
                "GetAllAsync() - Get all entities",
                "AddAsync(T entity) - Add new entity",
                "UpdateAsync(T entity) - Update existing entity",
                "DeleteAsync(int id) - Delete entity by ID"
            },
            benefits = new[]
            {
                "Reusable CRUD operations for any entity",
                "Consistent interface across repositories",
                "Easy to test and maintain",
                "Follows Repository Pattern principles"
            },
            endpoints = new
            {
                getAll = "GET /api/genericrepository/members",
                getById = "GET /api/genericrepository/members/{id}",
                add = "POST /api/genericrepository/members",
                update = "PUT /api/genericrepository/members/{id}",
                delete = "DELETE /api/genericrepository/members/{id}",
                info = "GET /api/genericrepository/info"
            }
        });
    }
}

// DTOs for requests
public class CreateMemberRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public MemberAddressRequest Address { get; set; } = new();
    public MembershipType MembershipType { get; set; }
}

public class UpdateMemberRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class MemberAddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
