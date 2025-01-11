namespace GamaEdtech.Back.Gateway.Rest.Controllers;

public class RegisterSchoolDto
{
	public SchoolNameDto Name { get; set; }
	public int Type { get; set; }
	public AddressDto Address { get; set; }
}

public class SchoolInListDto
{
	public Guid Id { get; set; }
	public SchoolNameDto Name { get; set; }
	public int Type { get; set; }
}

public class SearchByLocationDto
{
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public double RadiusInKm { get; set; }
}


public class SchoolNameDto
{
	public string InEnglish { get; set; }
	public string InLocalLanguage { get; set; }
}

public class AddressDto
{
	public string Description { get; set; }
	public LocationDto Location { get; set; }
	public string Country { get; set; }
	public string State { get; set; }
	public string City { get; set; }
	public string ZipCode { get; set; }

}

public class LocationDto
{
	public double Latitude { get; set; }
	public double Longitude { get; set; }
}



