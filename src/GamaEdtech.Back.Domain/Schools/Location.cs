using CSharpFunctionalExtensions;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace GamaEdtech.Back.Domain.Schools;

public class Location : ValueObject
{
	public Point Geography { get; }

	public double Latitude => Geography.Y; // Latitude is Y in NetTopologySuite
	public double Longitude => Geography.X; // Longitude is X in NetTopologySuite

	private Location() { }

	private Location(double latitude, double longitude)
	{
		var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
		Geography = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
	}

	public static Result<Location> Create(double latitude, double longitude)
	{
		if (latitude > 90 || latitude < -90)
			return Result.Failure<Location>("Latitude is out of range");

		if (longitude > 180 || longitude < -180)
			return Result.Failure<Location>("Longitude is out of range");

		return new Location(latitude, longitude);
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Geography;
	}
}