namespace GamaEdtech.Back.Gateway.Rest.Countries;

public class FindCountriesDto
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public string SortBy { get; set; } = "Name";
	public string Order { get; set; } = "asc";
}

public class ContryInListDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
}

public class AddCountryDto
{
	public string Name { get; set; }
	public string Code { get; set; }
}

public class EditCountryInfoDto
{
	public string Name { get; set; }
	public string Code { get; set; }
}
