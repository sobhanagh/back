namespace GamaEdtech.Back.Gateway.Rest.Cities;

public class AddCityDto
{
	public string Name { get; set; }
	public int? StateId { get; set; }
	public int CountryId { get; set; }
}

public class EditCityInfoDto
{
	public string Name { get; set; }
}