using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validators;

public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
{
    public CreateMemberDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ім'я обов'язкове")
            .MaximumLength(50).WithMessage("Ім'я не може перевищувати 50 символів");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Прізвище обов'язкове")
            .MaximumLength(50).WithMessage("Прізвище не може перевищувати 50 символів");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий")
            .EmailAddress().WithMessage("Невірний формат email");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Телефон обов'язковий")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Невірний формат телефону");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Дата народження обов'язкова")
            .LessThan(DateTime.Now).WithMessage("Дата народження має бути в минулому")
            .Must(BeAtLeast16YearsOld).WithMessage("Вік має бути не менше 16 років");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Адреса обов'язкова")
            .SetValidator(new AddressDtoValidator());
    }

    private bool BeAtLeast16YearsOld(DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;
        if (DateTime.Now < dateOfBirth.AddYears(age)) age--;
        return age >= 16;
    }
}

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Вулиця обов'язкова")
            .MaximumLength(100).WithMessage("Вулиця не може перевищувати 100 символів");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Місто обов'язкове")
            .MaximumLength(50).WithMessage("Місто не може перевищувати 50 символів");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Країна обов'язкова")
            .MaximumLength(50).WithMessage("Країна не може перевищувати 50 символів");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Поштовий індекс обов'язковий")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Невірний формат поштового індексу");
    }
}
