using CSharpFunctionalExtensions;

namespace GamaEdtech.Back.Domain.Schools;

public class Address : ValueObject
{
	public string Description { get; }
	public Location Location { get; }
	public string Country { get; }
	public string State { get; }
	public string City { get; set; }
	public string ZipCode { get; set; }

	protected Address() { }

	private Address(
		string description,
		Location location, 
		string country, 
		string state, 
		string city, 
		string zipCode)
	{
		Description = description;
		Location = location;
		Country = country;
		State = state;
		City = city;
		ZipCode = zipCode;
	}

	public static Result<Address> Create(
		string description,
		Location location,
		string country,
		string state,
		string city,
		string zipCode)
	{
		var descriptionOrError = ValidateDescription(description);
		var stateOrError = ValidateState(state);
		var cityOrError = ValidateCity(city);
		var zipCodeOrError = ValidateZipCode(zipCode);

		var result = Result.Combine(
			descriptionOrError,
			stateOrError,
			cityOrError,
			zipCodeOrError);

		if (result.IsFailure)
			return Result.Failure<Address>(result.Error);

		return new Address(
			descriptionOrError.Value, 
			location, 
			country,
			stateOrError.Value,
			cityOrError.Value,
			zipCodeOrError.Value);
	}

	private static Result<string> ValidateDescription(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<string>("Addess's description is required");

		value = value.Trim();

		if (value.Length > 500)
			return Result.Failure<string>("Addess's description is too long");

		return value;
	}

	private static Result<string> ValidateState(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<string>("Addess's state is required");

		value = value.Trim();

		if (value.Length > 50)
			return Result.Failure<string>("Addess's state is too long");

		return value;
	}

	private static Result<string> ValidateCity(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<string>("Addess's city is required");

		value = value.Trim();

		if (value.Length > 50)
			return Result.Failure<string>("Addess's city is too long");

		return value;
	}

	private static Result<string> ValidateZipCode(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<string>("Addess's zipcode is required");

		value = value.Trim();

		if (value.Length > 10)
			return Result.Failure<string>("Addess's zipcode is too long");

		return value;
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Description;
		yield return Location;
		yield return Country;
		yield return State;
		yield return City; 
		yield return ZipCode;
	}
}
