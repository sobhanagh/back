using CSharpFunctionalExtensions;

namespace GamaEdtech.Back.Domain.Schools;

public class Location : ValueObject
{
	public double Latitude { get; }
	public double Longitude { get; }

	public Location(double latitude, double longitude)
	{
		Latitude = latitude;
		Longitude = longitude;
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Latitude;
		yield return Longitude;
	}
}
