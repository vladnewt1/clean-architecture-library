using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validators;

public class CreateLoanDtoValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanDtoValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0).WithMessage("ID книги має бути більше 0");

        RuleFor(x => x.MemberId)
            .GreaterThan(0).WithMessage("ID члена має бути більше 0");
    }
}
