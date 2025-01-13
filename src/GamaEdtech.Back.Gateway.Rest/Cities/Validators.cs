using FluentValidation;
using GamaEdtech.Back.Gateway.Rest.States;

namespace GamaEdtech.Back.Gateway.Rest.Cities;

public class GetCitiesDtoValidator : AbstractValidator<GetCitiesDto>
{
	public GetCitiesDtoValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThan(0).WithMessage("page is not positive");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("pageSize is not positive")
			.LessThanOrEqualTo(50).WithMessage("pageSize is too large");

		RuleFor(x => x.SortBy)
			.NotNull().WithMessage("sortBy is required")
			.NotEmpty().WithMessage("sortBy is required")
			.Must(x => x == "Name").WithMessage("sortBy is invalid");

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


public class AddCityDtoValidator : AbstractValidator<AddCityDto>
{
	public AddCityDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotNull().WithMessage("name is required")
			.NotEmpty().WithMessage("name is required")
			.MaximumLength(100).WithMessage("name is too long");
	}
}

public class EditCityInfoDtoValidator : AbstractValidator<EditCityInfoDto>
{
	public EditCityInfoDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotNull().WithMessage("name is required")
			.NotEmpty().WithMessage("name is required")
			.MaximumLength(100).WithMessage("name is too long");
	}
}