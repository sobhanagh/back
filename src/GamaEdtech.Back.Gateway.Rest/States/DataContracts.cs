namespace GamaEdtech.Back.Gateway.Rest.States;

public class FindStatesDto
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public int? CountryId { get; set; }
	public string SortBy { get; set; } = "Name";
	public string Order { get; set; } = "asc";
}

public class StateInListDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public int CountryId { get; set; }
}

public class AddStateDto
{
	public int CountryId { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
}

public class EditStateInfoDto
{
	public string Name { get; set; }
	public string Code { get; set; }
}

public class MoveStateToAnotherDto
{
	public int CountryId { get; set; }
}