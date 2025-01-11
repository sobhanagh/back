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

	public Address(
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
