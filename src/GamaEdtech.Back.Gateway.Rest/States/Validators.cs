using FluentValidation;
using GamaEdtech.Back.Gateway.Rest.Countries;

namespace GamaEdtech.Back.Gateway.Rest.States;

public class FindStatesDtoValidator : AbstractValidator<FindStatesDto>
{
	public FindStatesDtoValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThan(0).WithMessage("page is not positive");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("pageSize is not positive")
			.LessThanOrEqualTo(50).WithMessage("pageSize is too large");

		RuleFor(x => x.SortBy)
			.NotNull().WithMessage("sortBy is required")
			.NotEmpty().WithMessage("sortBy is required")
			.Must(x => x == "Name" || x == "Code").WithMessage("sortBy is invalid");

		RuleFor(x => x.Order)
			.NotNull().WithMessage("order is required")
			.NotEmpty().WithMessage("order is required")
			.Must(x =>
			{
				x = x.ToUpper();
				return x == "ASC" || x == "DESC";
			}).WithMessage("order is invalid");
	}
}

public class AddStateDtoValidator : AbstractValidator<AddStateDto>
{
	public AddStateDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotNull().WithMessage("name is required")
			.NotEmpty().WithMessage("name is required")
			.MaximumLength(50).WithMessage("name is too long");

		RuleFor(x => x.Code)
			.NotNull().WithMessage("code is required")
			.MaximumLength(5).WithMessage("code is too long");
	}
}

public class EditStateInfoDtoValidator : AbstractValidator<EditStateInfoDto>
{
	public EditStateInfoDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotNull().WithMessage("name is required")
			.NotEmpty().WithMessage("name is required")
			.MaximumLength(50).WithMessage("name is too long");

		RuleFor(x => x.Code)
			.NotNull().WithMessage("code is required")
			.MaximumLength(5).WithMessage("code is too long");
	}
}

