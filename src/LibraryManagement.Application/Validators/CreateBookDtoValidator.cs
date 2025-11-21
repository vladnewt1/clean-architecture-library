using FluentValidation;
using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Назва книги обов'язкова")
            .MaximumLength(200).WithMessage("Назва не може перевищувати 200 символів");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Автор обов'язковий")
            .MaximumLength(100).WithMessage("Ім'я автора не може перевищувати 100 символів");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN обов'язковий")
            .Matches(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$")
            .WithMessage("Невірний формат ISBN");

        RuleFor(x => x.PublishedYear)
            .InclusiveBetween(1000, DateTime.Now.Year + 1)
            .WithMessage($"Рік публікації має бути між 1000 та {DateTime.Now.Year + 1}");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Кількість сторінок має бути більше 0");

        RuleFor(x => x.TotalCopies)
            .GreaterThan(0).WithMessage("Кількість копій має бути більше 0");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Ціна не може бути від'ємною");
    }
}
