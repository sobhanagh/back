namespace GamaEdtech.Back.Gateway.Rest.Cities;

public class GetCitiesDto
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public int? CountryId { get; set; }
	public int? StateId { get; set; }
	public string SortBy { get; set; } = "Name";
	public string Order { get; set; } = "asc";
}

public class CityInListDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string? State { get; set; }
	public string Country { get; set; }
}

public class CityDetailDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public CountryInCityDto Country { get; set; }
	public StateInCityDto? State { get; set; }
}

public class CountryInCityDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
}

public class StateInCityDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
}

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