namespace GamaEdtech.Backend.UI.Web.Api
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;
    using GamaEdtech.Backend.Data.Specification.School;
    using GamaEdtech.Backend.Data.ViewModel.School;
    using GamaEdtech.Backend.Shared.Service;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SchoolsController(Lazy<ILogger<SchoolsController>> logger, Lazy<ISchoolService> boardService)
        : ApiControllerBase<SchoolsController>(logger)
    {
        [HttpGet, Produces<ApiResponse<ListDataSource<SchoolsResponseViewModel>>>()]
        public async Task<IActionResult> GetSchools([NotNull, FromQuery] SchoolsRequestViewModel request)
        {
            try
            {
                ISpecification<School>? specification = null;
                if (request.CountryIds?.Any() == true)
                {
                    specification = new CountryIdsContainsSpecification(request.CountryIds);
                }
                if (request.StateIds?.Any() == true)
                {
                    var stateSpecification = new StateIdsContainsSpecification(request.StateIds);
                    specification = specification is null ? stateSpecification : specification.And(stateSpecification);
                }
                if (request.CityIds?.Any() == true)
                {
                    var citySpecification = new CityIdsContainsSpecification(request.CityIds);
                    specification = specification is null ? citySpecification : specification.And(citySpecification);
                }

                var result = await boardService.Value.GetSchoolsAsync(new ListRequestDto<School>
                {
                    PagingDto = request.PagingDto,
                    Specification = specification,
                });
                return Ok(new ApiResponse<ListDataSource<SchoolsResponseViewModel>>
                {
                    Errors = result.Errors,
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new SchoolsResponseViewModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            LocalName = t.LocalName,
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<SchoolsResponseViewModel>> { Errors = [new() { Message = exc.Message }] });
            }
        }
    }
}
