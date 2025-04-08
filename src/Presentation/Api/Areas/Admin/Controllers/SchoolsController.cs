namespace GamaEdtech.Presentation.Api.Areas.Admin.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;

    using Asp.Versioning;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.Specification.Impl;
    using GamaEdtech.Common.Identity;
    using GamaEdtech.Data.Dto.Contribution;
    using GamaEdtech.Data.Dto.School;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Domain.Specification;
    using GamaEdtech.Domain.Specification.Contribution;
    using GamaEdtech.Presentation.ViewModel.School;

    using Microsoft.AspNetCore.Mvc;

    using NetTopologySuite;
    using NetTopologySuite.Geometries;

    [Common.DataAnnotation.Area(nameof(Admin), "Admin")]
    [Route("api/v{version:apiVersion}/[area]/[controller]")]
    [ApiVersion("1.0")]
    [Permission(Roles = [nameof(Role.Admin)])]
    public class SchoolsController(Lazy<ILogger<SchoolsController>> logger, Lazy<ISchoolService> schoolService, Lazy<IContributionService> contributionService)
        : ApiControllerBase<SchoolsController>(logger)
    {
        #region Schools

        [HttpGet, Produces<ApiResponse<ListDataSource<SchoolsResponseViewModel>>>()]
        public async Task<IActionResult> GetSchools([NotNull, FromQuery] SchoolsRequestViewModel request)
        {
            try
            {
                var result = await schoolService.Value.GetSchoolsAsync(new ListRequestDto<School>
                {
                    PagingDto = request.PagingDto,
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
                var result = await schoolService.Value.GetSchoolAsync(new IdEqualsSpecification<School, long>(id));
                return Ok(new ApiResponse<SchoolResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = result.Data is null ? null : MapFrom(result.Data)
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<SchoolResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPost, Produces<ApiResponse<ManageSchoolResponseViewModel>>()]
        public async Task<IActionResult> CreateSchool([NotNull] ManageSchoolRequestViewModel request)
        {
            try
            {
                Point? coordinates = null;
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                    coordinates = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
                }

                var result = await schoolService.Value.ManageSchoolAsync(new ManageSchoolRequestDto
                {
                    Address = request.Address,
                    Name = request.Name,
                    LocalName = request.LocalName,
                    SchoolType = request.SchoolType!,
                    StateId = request.StateId,
                    ZipCode = request.ZipCode,
                    Coordinates = coordinates,
                    Quarter = request.Quarter,
                    PhoneNumber = request.PhoneNumber,
                    FaxNumber = request.FaxNumber,
                    Email = request.Email,
                    CountryId = request.CountryId,
                    CityId = request.CityId,
                    Facilities = request.Facilities,
                    LocalAddress = request.LocalAddress,
                    WebSite = request.WebSite,
                    OsmId = request.OsmId,
                }, false);
                return Ok(new ApiResponse<ManageSchoolResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new() { Id = result.Data, },
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ManageSchoolResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPut("{id:int}"), Produces<ApiResponse<ManageSchoolResponseViewModel>>()]
        public async Task<IActionResult> UpdateSchool([FromRoute] int id, [NotNull, FromBody] ManageSchoolRequestViewModel request)
        {
            try
            {
                Point? coordinates = null;
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                    coordinates = geometryFactory.CreatePoint(new Coordinate(request.Longitude.Value, request.Latitude.Value));
                }
                var result = await schoolService.Value.ManageSchoolAsync(new ManageSchoolRequestDto
                {
                    Id = id,
                    Address = request.Address,
                    Name = request.Name,
                    LocalName = request.LocalName,
                    SchoolType = request.SchoolType!,
                    StateId = request.StateId,
                    ZipCode = request.ZipCode,
                    Coordinates = coordinates,
                    WebSite = request.WebSite,
                    LocalAddress = request.LocalAddress,
                    Facilities = request.Facilities,
                    CityId = request.CityId,
                    CountryId = request.CountryId,
                    Email = request.Email,
                    FaxNumber = request.FaxNumber,
                    PhoneNumber = request.PhoneNumber,
                    Quarter = request.Quarter,
                    OsmId = request.OsmId,
                }, false);
                return Ok(new ApiResponse<ManageSchoolResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new() { Id = result.Data, },
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ManageSchoolResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpDelete("{id:long}"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult> RemoveSchool([FromRoute] long id)
        {
            try
            {
                var result = await schoolService.Value.RemoveSchoolAsync(new IdEqualsSpecification<School, long>(id));
                return Ok(new ApiResponse<bool>
                {
                    Errors = result.Errors,
                    Data = result.Data
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        #endregion

        #region Comments

        [HttpGet("comments/pending"), Produces<ApiResponse<ListDataSource<SchoolCommentContributionListResponseViewModel>>>()]
        public async Task<IActionResult<ListDataSource<SchoolCommentContributionListResponseViewModel>>> GetPendingSchoolCommentContributionList([NotNull, FromQuery] SchoolCommentContributionListRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.GetContributionsAsync(new ListRequestDto<Contribution>
                {
                    PagingDto = request.PagingDto,
                    Specification = new StatusEqualsSpecification<Contribution>(Status.Draft)
                        .And(new ContributionTypeEqualsSpecification(ContributionType.SchoolComment)),
                });
                return Ok(new ApiResponse<ListDataSource<SchoolCommentContributionListResponseViewModel>>(result.Errors)
                {
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new SchoolCommentContributionListResponseViewModel
                        {
                            Id = t.Id,
                            CreationUser = t.CreationUser,
                            CreationDate = t.CreationDate,
                            SchoolId = t.IdentifierId.GetValueOrDefault(),
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<SchoolCommentContributionListResponseViewModel>>(new Error { Message = exc.Message }));
            }
        }

        [HttpGet("comments/{contributionId:long}"), Produces<ApiResponse<SchoolCommentContributionReviewViewModel>>()]
        public async Task<IActionResult<SchoolCommentContributionReviewViewModel>> GetSchoolCommentContribution([FromRoute] long contributionId)
        {
            try
            {
                var specification = new IdEqualsSpecification<Contribution, long>(contributionId)
                    .And(new ContributionTypeEqualsSpecification(ContributionType.SchoolComment));
                var contributionResult = await contributionService.Value.GetContributionAsync(specification);
                if (contributionResult.Data?.Data is null)
                {
                    return Ok(new ApiResponse<SchoolCommentContributionReviewViewModel>(contributionResult.Errors));
                }

                var schoolResult = await schoolService.Value.GetSchoolAsync(new IdEqualsSpecification<School, long>(contributionResult.Data.IdentifierId.GetValueOrDefault()));
                if (schoolResult.OperationResult is not Constants.OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<SchoolCommentContributionReviewViewModel>(schoolResult.Errors));
                }

                var dto = JsonSerializer.Deserialize<SchoolCommentContributionDto>(contributionResult.Data.Data)!;

                SchoolCommentContributionReviewViewModel result = new()
                {
                    Id = contributionResult.Data.Id,
                    SchoolId = dto.SchoolId,
                    SchoolName = schoolResult.Data?.Name,
                    ArtisticActivitiesRate = dto.ArtisticActivitiesRate,
                    AverageRate = dto.AverageRate,
                    BehaviorRate = dto.BehaviorRate,
                    ClassesQualityRate = dto.ClassesQualityRate,
                    Comment = dto.Comment,
                    EducationRate = dto.EducationRate,
                    FacilitiesRate = dto.FacilitiesRate,
                    ITTrainingRate = dto.ITTrainingRate,
                    SafetyAndHappinessRate = dto.SafetyAndHappinessRate,
                    TuitionRatioRate = dto.TuitionRatioRate,
                };

                return Ok(new ApiResponse<SchoolCommentContributionReviewViewModel>
                {
                    Data = result,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<SchoolCommentContributionReviewViewModel>(new Error { Message = exc.Message }));
            }
        }


        [HttpPatch("comments/{contributionId:long}/confirm"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult> ConfirmSchoolCommentContribution([FromRoute] long contributionId)
        {
            try
            {
                var result = await schoolService.Value.ConfirmSchoolCommentContributionAsync(new ConfirmSchoolCommentContributionRequestDto
                {
                    ContributionId = contributionId,
                });

                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPatch("comments/{contributionId:long}/reject"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult> RejectSchoolCommentContribution([FromRoute] long contributionId, [NotNull, FromBody] RejectContributionRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.RejectContributionAsync(new RejectContributionRequestDto
                {
                    Id = contributionId,
                    Comment = request.Comment,
                });
                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        #endregion

        #region Images

        [HttpGet("images/pending"), Produces<ApiResponse<ListDataSource<SchoolImageContributionListResponseViewModel>>>()]
        public async Task<IActionResult<ListDataSource<SchoolImageContributionListResponseViewModel>>> GetPendingSchoolImageContributionList([NotNull, FromQuery] SchoolImageContributionListRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.GetContributionsAsync(new ListRequestDto<Contribution>
                {
                    PagingDto = request.PagingDto,
                    Specification = new StatusEqualsSpecification<Contribution>(Status.Draft)
                        .And(new ContributionTypeEqualsSpecification(ContributionType.SchoolImage)),
                });
                return Ok(new ApiResponse<ListDataSource<SchoolImageContributionListResponseViewModel>>(result.Errors)
                {
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new SchoolImageContributionListResponseViewModel
                        {
                            Id = t.Id,
                            CreationUser = t.CreationUser,
                            CreationDate = t.CreationDate,
                            SchoolId = t.IdentifierId.GetValueOrDefault(),
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<SchoolImageContributionListResponseViewModel>>(new Error { Message = exc.Message }));
            }
        }

        [HttpGet("images/{contributionId:long}"), Produces<ApiResponse<SchoolImageContributionReviewViewModel>>()]
        public async Task<IActionResult<SchoolImageContributionReviewViewModel>> GetSchoolImageContribution([FromRoute] long contributionId)
        {
            try
            {
                var specification = new IdEqualsSpecification<Contribution, long>(contributionId)
                    .And(new ContributionTypeEqualsSpecification(ContributionType.SchoolImage));
                var contributionResult = await contributionService.Value.GetContributionAsync(specification);
                if (contributionResult.Data?.Data is null)
                {
                    return Ok(new ApiResponse<SchoolImageContributionReviewViewModel>(contributionResult.Errors));
                }

                var schoolResult = await schoolService.Value.GetSchoolAsync(new IdEqualsSpecification<School, long>(contributionResult.Data.IdentifierId.GetValueOrDefault()));
                if (schoolResult.OperationResult is not Constants.OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<SchoolImageContributionReviewViewModel>(schoolResult.Errors));
                }

                var dto = JsonSerializer.Deserialize<SchoolImageContributionDto>(contributionResult.Data.Data)!;

                SchoolImageContributionReviewViewModel result = new()
                {
                    Id = contributionResult.Data.Id,
                    FileId = dto.FileId,
                    FileType = dto.FileType,
                    SchoolId = dto.SchoolId,
                    SchoolName = schoolResult.Data?.Name,
                };

                return Ok(new ApiResponse<SchoolImageContributionReviewViewModel>
                {
                    Data = result,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<SchoolImageContributionReviewViewModel>(new Error { Message = exc.Message }));
            }
        }

        [HttpPatch("images/{contributionId:long}/confirm"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult> ConfirmSchoolImageContribution([FromRoute] long contributionId)
        {
            try
            {
                var result = await schoolService.Value.ConfirmSchoolImageContributionAsync(new ConfirmSchoolImageContributionRequestDto
                {
                    ContributionId = contributionId,
                });

                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPatch("images/{contributionId:long}/reject"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult> RejectSchoolImageContribution([FromRoute] long contributionId, [NotNull, FromBody] RejectContributionRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.RejectContributionAsync(new RejectContributionRequestDto
                {
                    Id = contributionId,
                    Comment = request.Comment,
                });
                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        #endregion

        #region Contributions

        [HttpGet("contributions/pending"), Produces<ApiResponse<ListDataSource<SchoolContributionListResponseViewModel>>>()]
        public async Task<IActionResult<ListDataSource<SchoolContributionListResponseViewModel>>> GetSchoolContributionList([NotNull, FromQuery] SchoolContributionListRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.GetContributionsAsync(new ListRequestDto<Contribution>
                {
                    PagingDto = request.PagingDto,
                    Specification = new StatusEqualsSpecification<Contribution>(Status.Draft)
                        .And(new ContributionTypeEqualsSpecification(ContributionType.School)),
                });
                return Ok(new ApiResponse<ListDataSource<SchoolContributionListResponseViewModel>>
                {
                    Errors = result.Errors,
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new SchoolContributionListResponseViewModel
                        {
                            Id = t.Id,
                            Comment = t.Comment,
                            CreationUser = t.CreationUser,
                            CreationDate = t.CreationDate,
                            IdentifierId = t.IdentifierId,
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<SchoolContributionListResponseViewModel>>(new Error { Message = exc.Message }));
            }
        }

        [HttpGet("contributions/{contributionId:long}"), Produces<ApiResponse<SchoolContributionReviewViewModel>>()]
        public async Task<IActionResult<SchoolContributionReviewViewModel>> GetSchoolContribution([FromRoute] long contributionId)
        {
            try
            {
                var specification = new IdEqualsSpecification<Contribution, long>(contributionId)
                    .And(new ContributionTypeEqualsSpecification(ContributionType.School));
                var contributionResult = await contributionService.Value.GetContributionAsync(specification);
                if (contributionResult.Data?.Data is null)
                {
                    return Ok(new ApiResponse<SchoolContributionReviewViewModel>(contributionResult.Errors));
                }

                var schoolResult = await schoolService.Value.GetSchoolAsync(new IdEqualsSpecification<School, long>(contributionResult.Data.IdentifierId.GetValueOrDefault()));
                if (schoolResult.OperationResult is not Constants.OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<SchoolContributionReviewViewModel>(schoolResult.Errors));
                }

                SchoolContributionReviewViewModel result = new()
                {
                    NewValues = Api.Controllers.SchoolsController.MapFrom(JsonSerializer.Deserialize<SchoolContributionDto>(contributionResult.Data.Data)!),
                    OldValues = MapFrom(schoolResult.Data!),
                };

                return Ok(new ApiResponse<SchoolContributionReviewViewModel>
                {
                    Data = result,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<SchoolContributionReviewViewModel>(new Error { Message = exc.Message }));
            }
        }

        [HttpPatch("contributions/{contributionId:long}/confirm"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult<bool>> ConfirmSchoolContribution([FromRoute] long contributionId, [NotNull, FromBody] ConfirmSchoolContributionRequestViewModel request)
        {
            try
            {
                var contribution = await contributionService.Value.GetContributionAsync(new IdEqualsSpecification<Contribution, long>(contributionId));
                if (contribution.Data is null)
                {
                    return Ok(new ApiResponse<bool>(contribution.Errors));
                }

                var result = await schoolService.Value.ConfirmSchoolContributionAsync(new ConfirmSchoolContributionRequestDto
                {
                    ContributionId = contributionId,
                    SchoolId = request.SchoolId.GetValueOrDefault(),
                    Data = JsonSerializer.Deserialize<SchoolContributionDto>(contribution.Data.Data!)!,
                });

                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPatch("contributions/{contributionId:long}/reject"), Produces<ApiResponse<bool>>()]
        public async Task<IActionResult<bool>> RejectSchoolContribution([FromRoute] long contributionId, [NotNull, FromBody] RejectContributionRequestViewModel request)
        {
            try
            {
                var result = await contributionService.Value.RejectContributionAsync(new RejectContributionRequestDto
                {
                    Id = contributionId,
                    Comment = request.Comment,
                });
                return Ok(new ApiResponse<bool>(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool>(new Error { Message = exc.Message }));
            }
        }

        #endregion

        private static SchoolResponseViewModel MapFrom(SchoolDto dto) => new()
        {
            Id = dto.Id,
            Address = dto.Address,
            LocalAddress = dto.LocalAddress,
            Name = dto.Name,
            LocalName = dto.LocalName,
            SchoolType = dto.SchoolType,
            StateId = dto.StateId,
            StateTitle = dto.StateTitle,
            ZipCode = dto.ZipCode,
            Latitude = dto.Coordinates?.Y,
            Longitude = dto.Coordinates?.X,
            Facilities = dto.Facilities,
            WebSite = dto.WebSite,
            Email = dto.Email,
            CityId = dto.CityId,
            CityTitle = dto.CityTitle,
            CountryId = dto.CountryId,
            CountryTitle = dto.CountryTitle,
            FaxNumber = dto.FaxNumber,
            PhoneNumber = dto.PhoneNumber,
            Quarter = dto.Quarter,
            OsmId = dto.OsmId,
        };
    }
}
