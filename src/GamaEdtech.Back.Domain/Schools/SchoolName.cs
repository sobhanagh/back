using CSharpFunctionalExtensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GamaEdtech.Back.Domain.Schools;

public class SchoolName : ValueObject
{
	public string InEnglish { get; }
	public string InLocalLanguage { get; }

	private SchoolName(string inEnglish, string inLocalLanguage)
	{
		InEnglish = inEnglish;
		InLocalLanguage = inLocalLanguage;
	}

	public static Result<SchoolName> Create(
		string nameInEnglish, 
		string nameInLocalLanguage)
	{
		var nameInEnglishOrError = ValidateNameInEnglish(nameInEnglish);
		var nameInLocalLanguageOrError = ValidateNameInLocalLanguage(nameInLocalLanguage);

		var result = Result.Combine(
			nameInEnglishOrError, 
			nameInLocalLanguageOrError);

		if(result.IsFailure)
			return Result.Failure<SchoolName>(result.Error);

		return new SchoolName(
			nameInEnglishOrError.Value, 
			nameInLocalLanguageOrError.Value);
	}

	private static Result<string> ValidateNameInEnglish(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<string>("Name in English is required");

		value = value.Trim();

		if (value.Length > 100)
			return Result.Failure<string>("Name in English is too long");

		return value;
	}

	private static Result<string> ValidateNameInLocalLanguage(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Success("");

		value = value.Trim();

		if (value.Length > 100)
			return Result.Failure<string>("Name in local language is too long");

		return value;
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return InEnglish;
		yield return InLocalLanguage;
	}
}
