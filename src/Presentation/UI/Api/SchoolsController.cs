namespace GamaEdtech.Backend.UI.Web.Api
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using GamaEdtech.Backend.Common.Core;
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using GamaEdtech.Backend.Common.DataAccess.Specification.Impl;

    using GamaEdtech.Backend.Data.Entity;
    using GamaEdtech.Backend.Data.Specification.School;
    using GamaEdtech.Backend.Data.ViewModel.School;
    using GamaEdtech.Backend.Shared.Service;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SchoolsController(Lazy<ILogger<SchoolsController>> logger, Lazy<ISchoolService> schoolService)
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

                var result = await schoolService.Value.GetSchoolsAsync(new ListRequestDto<School>
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

        [HttpGet("{id:int}"), Produces<ApiResponse<SchoolResponseViewModel>>()]
        public async Task<IActionResult> GetSchool([FromRoute] int id)
        {
            try
            {
                var result = await schoolService.Value.GetSchoolAsync(new IdEqualsSpecification<School, int>(id));
                return Ok(new ApiResponse<SchoolResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = result.Data is null ? null : new()
                    {
                        Id = result.Data.Id,
                        Address = result.Data.Address,
                        LocalAddress = result.Data.LocalAddress,
                        Name = result.Data.Name,
                        LocalName = result.Data.LocalName,
                        SchoolType = result.Data.SchoolType,
                        StateId = result.Data.StateId,
                        StateTitle = result.Data.StateTitle,
                        ZipCode = result.Data.ZipCode,
                        Latitude = result.Data.Latitude,
                        Longitude = result.Data.Longitude,
                        Facilities = result.Data.Facilities,
                        WebSite = result.Data.WebSite,
                        Email = result.Data.Email,
                        CityId = result.Data.CityId,
                        CityTitle = result.Data.CityTitle,
                        CountryId = result.Data.CountryId,
                        CountryTitle = result.Data.CountryTitle,
                        FaxNumber = result.Data.FaxNumber,
                        PhoneNumber = result.Data.PhoneNumber,
                        Quarter = result.Data.Quarter,
                        OsmId = result.Data.OsmId,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<SchoolResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }
    }
}
