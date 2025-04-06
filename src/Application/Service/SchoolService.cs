namespace GamaEdtech.Application.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.Json;

    using EntityFramework.Exceptions.Common;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Core.Extensions.Linq;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.Specification.Impl;
    using GamaEdtech.Common.DataAccess.UnitOfWork;
    using GamaEdtech.Common.Service;
    using GamaEdtech.Data.Dto.Contribution;
    using GamaEdtech.Data.Dto.School;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Domain.Specification.Contribution;
    using GamaEdtech.Domain.Specification;

    using MetadataExtractor;
    using MetadataExtractor.Formats.Exif;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using NetTopologySuite;
    using NetTopologySuite.Geometries;

    using static GamaEdtech.Common.Core.Constants;

    public class SchoolService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<FileService>> localizer
        , Lazy<ILogger<FileService>> logger, Lazy<IFileService> fileService, Lazy<IContributionService> contributionService, Lazy<IIdentityService> identityService)
        : LocalizableServiceBase<FileService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), ISchoolService
    {
        #region Schools

        public async Task<ResultData<ListDataSource<SchoolsDto>>> GetSchoolsAsync(ListRequestDto<School>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<School, long>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new SchoolsDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    LocalName = t.LocalName,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<ListDataSource<SchoolInfoDto>>> GetSchoolsListAsync(ListRequestDto<School>? requestDto = null, Point? point = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var lst = uow.GetRepository<School, long>().GetManyQueryable(requestDto?.Specification);
                int? total = requestDto?.PagingDto?.PageFilter?.ReturnTotalRecordsCount == true ? await lst.CountAsync() : null;
                var query = lst.Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.LastModifyDate,
                    t.CreationDate,
                    t.WebSite,
                    t.Email,
                    t.PhoneNumber,
                    t.Coordinates,
                    Score = t.Comments != null ? t.Comments.Average(c => c.AverageRate) : (double?)null,
                    CityTitle = t.City == null ? "" : t.City.Title,
                    CountryTitle = t.Country == null ? "" : t.Country.Title,
                    StateTitle = t.State == null ? "" : t.State.Title,
                    Distance = point != null && t.Coordinates != null ? t.Coordinates!.Distance(point) : (double?)null,
                });
                query = point is null ? query.OrderBy(t => t.Distance) : query.OrderBy(t => t.Id);
                if (requestDto?.PagingDto?.PageFilter is not null)
                {
                    query = query.Skip(requestDto.PagingDto.PageFilter.Skip)
                        .Take(requestDto.PagingDto.PageFilter.Size);
                }
                var data = await query.ToListAsync();

                var result = data.Select(t => new SchoolInfoDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    LastModifyDate = t.LastModifyDate ?? t.CreationDate,
                    HasWebSite = !string.IsNullOrEmpty(t.WebSite),
                    HasEmail = !string.IsNullOrEmpty(t.Email),
                    HasPhoneNumber = !string.IsNullOrEmpty(t.PhoneNumber),
                    Coordinates = t.Coordinates,
                    Score = t.Score,
                    CityTitle = t.CityTitle,
                    CountryTitle = t.CountryTitle,
                    StateTitle = t.StateTitle,
                    Distance = t.Distance,
                });
                return new(OperationResult.Succeeded) { Data = new() { List = result, TotalRecordsCount = total } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<SchoolDto>> GetSchoolAsync([NotNull] ISpecification<School> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var school = await uow.GetRepository<School, long>().GetManyQueryable(specification).Select(t => new SchoolDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    LocalName = t.LocalName,
                    Address = t.Address,
                    LocalAddress = t.LocalAddress,
                    Coordinates = t.Coordinates,
                    SchoolType = t.SchoolType,
                    ZipCode = t.ZipCode,
                    CityId = t.CityId,
                    CityTitle = t.City != null ? t.City.Title : "",
                    CountryId = t.CountryId,
                    CountryTitle = t.Country != null ? t.Country.Title : "",
                    StateId = t.StateId,
                    StateTitle = t.State != null ? t.State.Title : "",
                    Facilities = t.Facilities,
                    WebSite = t.WebSite,
                    FaxNumber = t.FaxNumber,
                    PhoneNumber = t.PhoneNumber,
                    Email = t.Email,
                    Quarter = t.Quarter,
                    OsmId = t.OsmId,
                }).FirstOrDefaultAsync();

                return school is null
                    ? new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["SchoolNotFound"] },],
                    }
                    : new(OperationResult.Succeeded) { Data = school };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<long>> ManageSchoolAsync([NotNull] ManageSchoolRequestDto requestDto, bool ignoreNullValues)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<School, long>();
                School? school = null;

                if (requestDto.Id.HasValue)
                {
                    school = await repository.GetAsync(requestDto.Id.Value);
                    if (school is null)
                    {
                        return new(OperationResult.NotFound)
                        {
                            Errors = [new() { Message = Localizer.Value["SchoolNotFound"] },],
                        };
                    }

                    school.Name = Get(requestDto.Name, school.Name);
                    school.LocalName = Get(requestDto.LocalName, school.LocalName);
                    school.Address = Get(requestDto.Address, school.Address);
                    school.Coordinates = Get(requestDto.Coordinates, school.Coordinates);
                    school.SchoolType = Get(requestDto.SchoolType, school.SchoolType);
                    school.StateId = Get(requestDto.StateId, school.StateId);
                    school.ZipCode = Get(requestDto.ZipCode, school.ZipCode);
                    school.WebSite = Get(requestDto.WebSite, school.WebSite);
                    school.Quarter = Get(requestDto.Quarter, school.Quarter);
                    school.PhoneNumber = Get(requestDto.PhoneNumber, school.PhoneNumber);
                    school.LocalAddress = Get(requestDto.LocalAddress, school.LocalAddress);
                    school.FaxNumber = Get(requestDto.FaxNumber, school.FaxNumber);
                    school.Facilities = Get(requestDto.Facilities, school.Facilities);
                    school.Email = Get(requestDto.Email, school.Email);
                    school.CityId = Get(requestDto.CityId, school.CityId);
                    school.CountryId = Get(requestDto.CountryId, school.CountryId);
                    school.OsmId = Get(requestDto.OsmId, school.OsmId);

                    _ = repository.Update(school);
                }
                else
                {
                    school = new School
                    {
                        Name = requestDto.Name,
                        LocalName = requestDto.LocalName,
                        Address = requestDto.Address,
                        Coordinates = requestDto.Coordinates,
                        SchoolType = requestDto.SchoolType,
                        StateId = requestDto.StateId,
                        ZipCode = requestDto.ZipCode,
                        WebSite = requestDto.WebSite,
                        Quarter = requestDto.Quarter,
                        PhoneNumber = requestDto.PhoneNumber,
                        LocalAddress = requestDto.LocalAddress,
                        FaxNumber = requestDto.FaxNumber,
                        Facilities = requestDto.Facilities,
                        Email = requestDto.Email,
                        CityId = requestDto.CityId,
                        CountryId = requestDto.CountryId,
                        OsmId = requestDto.OsmId,
                    };
                    repository.Add(school);
                }

                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = school.Id };

                T Get<T>(T newValue, T oldValue) => !ignoreNullValues && !string.IsNullOrEmpty(newValue?.ToString())
                    ? newValue
                    : oldValue;
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["InvalidStateId"], }] };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> RemoveSchoolAsync([NotNull] ISpecification<School> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var school = await uow.GetRepository<School, long>().GetAsync(specification);
                if (school is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Data = false,
                        Errors = [new() { Message = Localizer.Value["SchoolNotFound"] },],
                    };
                }

                uow.GetRepository<School, long>().Remove(school);
                _ = await uow.SaveChangesAsync();
                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["SchoolCantBeRemoved"], },] };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> ExistSchoolAsync([NotNull] ISpecification<School> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var exist = await uow.GetRepository<School, long>().AnyAsync(specification);
                return new(OperationResult.Succeeded) { Data = exist };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        #endregion

        #region Rate and Comments

        public async Task<ResultData<SchoolRateDto>> GetSchoolRateAsync([NotNull] ISpecification<SchoolComment> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<SchoolComment>().GetManyQueryable(specification)
                    .GroupBy(t => t.SchoolId)
                    .Select(t => new SchoolRateDto
                    {
                        AverageRate = t.Average(c => c.AverageRate),
                        TuitionRatioRate = t.Average(c => c.TuitionRatioRate),
                        SafetyAndHappinessRate = t.Average(c => c.SafetyAndHappinessRate),
                        ITTrainingRate = t.Average(c => c.ITTrainingRate),
                        FacilitiesRate = t.Average(c => c.FacilitiesRate),
                        EducationRate = t.Average(c => c.EducationRate),
                        ClassesQualityRate = t.Average(c => c.ClassesQualityRate),
                        BehaviorRate = t.Average(c => c.BehaviorRate),
                        ArtisticActivitiesRate = t.Average(c => c.ArtisticActivitiesRate),
                        TotalCount = t.Count(),
                    }).FirstOrDefaultAsync();

                return new(OperationResult.Succeeded) { Data = result };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<ListDataSource<SchoolCommentDto>>> GetSchoolCommentsAsync(ListRequestDto<SchoolComment>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<SchoolComment>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new SchoolCommentDto
                {
                    Id = t.Id,
                    Comment = t.Comment,
                    CreationUser = t.CreationUser!.FirstName + " " + t.CreationUser.LastName,
                    CreationUserAvatar = t.CreationUser!.Avatar,
                    CreationDate = t.CreationDate,
                    LikeCount = t.LikeCount,
                    DislikeCount = t.DislikeCount,
                    AverageRate = t.AverageRate,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<long>> ManageSchoolCommentAsync([NotNull] ManageSchoolCommentRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<SchoolComment>();
                var schoolRepository = uow.GetRepository<School, long>();
                SchoolComment? schoolComment = null;

                if (requestDto.Id.HasValue)
                {
                    schoolComment = await repository.GetAsync(requestDto.Id.Value);
                    if (schoolComment is null)
                    {
                        return new(OperationResult.NotFound)
                        {
                            Errors = [new() { Message = Localizer.Value["SchoolCommentNotFound"] },],
                        };
                    }

                    if (schoolComment.CreationUserId != requestDto.CreationUserId)
                    {
                        return new(OperationResult.NotValid)
                        {
                            Errors = [new() { Message = Localizer.Value["InvalidRequest"] },],
                        };
                    }

                    if (schoolComment.Comment != requestDto.Comment)
                    {
                        schoolComment.Status = Status.Draft;
                    }
                    schoolComment.Comment = requestDto.Comment;
                    schoolComment.CreationDate = requestDto.CreationDate;
                    schoolComment.CreationUserId = requestDto.CreationUserId;

                    schoolComment.ArtisticActivitiesRate = requestDto.ArtisticActivitiesRate;
                    schoolComment.BehaviorRate = requestDto.BehaviorRate;
                    schoolComment.ClassesQualityRate = requestDto.ClassesQualityRate;
                    schoolComment.EducationRate = requestDto.EducationRate;
                    schoolComment.FacilitiesRate = requestDto.FacilitiesRate;
                    schoolComment.ITTrainingRate = requestDto.ITTrainingRate;
                    schoolComment.SafetyAndHappinessRate = requestDto.SafetyAndHappinessRate;
                    schoolComment.TuitionRatioRate = requestDto.TuitionRatioRate;
                    schoolComment.AverageRate = Calculate(schoolComment);

                    _ = repository.Update(schoolComment);
                }
                else
                {
                    schoolComment = new SchoolComment
                    {
                        SchoolId = requestDto.SchoolId,
                        ArtisticActivitiesRate = requestDto.ArtisticActivitiesRate,
                        BehaviorRate = requestDto.BehaviorRate,
                        ClassesQualityRate = requestDto.ClassesQualityRate,
                        Comment = requestDto.Comment,
                        CreationDate = requestDto.CreationDate,
                        CreationUserId = requestDto.CreationUserId,
                        EducationRate = requestDto.EducationRate,
                        FacilitiesRate = requestDto.FacilitiesRate,
                        ITTrainingRate = requestDto.ITTrainingRate,
                        SafetyAndHappinessRate = requestDto.SafetyAndHappinessRate,
                        TuitionRatioRate = requestDto.TuitionRatioRate,
                        //Status = Status.Draft,
                        Status = Status.Confirmed,
                    };
                    schoolComment.AverageRate = Calculate(schoolComment);
                    repository.Add(schoolComment);
                }

                _ = await uow.SaveChangesAsync();
                _ = await schoolRepository.GetManyQueryable(t => t.Id == requestDto.SchoolId)
                    .ExecuteUpdateAsync(t => t.SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow));

                return new(OperationResult.Succeeded) { Data = schoolComment.Id };

                static double Calculate(SchoolComment comment) => (
                        comment.ArtisticActivitiesRate +
                        comment.BehaviorRate +
                        comment.ClassesQualityRate +
                        comment.EducationRate +
                        comment.FacilitiesRate +
                        comment.ITTrainingRate +
                        comment.SafetyAndHappinessRate +
                        comment.TuitionRatioRate
                        ) / 8;
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["InvalidData"], }] };
            }
            catch (UniqueConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["DuplicateData"], }] };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> LikeSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolComment>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t.SetProperty(p => p.LikeCount, p => p.LikeCount + 1));

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> DislikeSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolComment>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t.SetProperty(p => p.DislikeCount, p => p.DislikeCount + 1));

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> ConfirmSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolComment>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t
                        .SetProperty(p => p.Status, Status.Confirmed)
                        .SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow)
                        .SetProperty(p => p.LastModifyUserId, HttpContextAccessor.Value.HttpContext.UserId())
                    );

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> RejectSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolComment>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t
                        .SetProperty(p => p.Status, Status.Rejected)
                        .SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow)
                        .SetProperty(p => p.LastModifyUserId, HttpContextAccessor.Value.HttpContext.UserId())
                    );

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        #endregion

        #region Images

        public async Task<ResultData<ListDataSource<SchoolImageDto>>> GetSchoolImagesAsync(ListRequestDto<SchoolImage>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<SchoolImage>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new SchoolImageDto
                {
                    Id = t.Id,
                    CreationUser = t.CreationUser!.FirstName + " " + t.CreationUser.LastName,
                    CreationUserAvatar = t.CreationUser!.Avatar,
                    CreationDate = t.CreationDate,
                    FileId = t.FileId,
                    FileType = t.FileType,
                    SchoolId = t.SchoolId,
                    SchoolName = t.School!.Name,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<IEnumerable<string?>>> GetSchoolImagesPathAsync([NotNull] ISpecification<SchoolImage> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<SchoolImage>().GetManyQueryable(specification)
                    .Select(t => t.FileId).ToListAsync();
                return new(OperationResult.Succeeded) { Data = result.Select(t => fileService.Value.GetFileUri(t, ContainerType.School).Data?.ToString()) };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<bool>> ConfirmSchoolImageAsync([NotNull] ISpecification<SchoolImage> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolImage>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t
                        .SetProperty(p => p.Status, Status.Confirmed)
                        .SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow)
                        .SetProperty(p => p.LastModifyUserId, HttpContextAccessor.Value.HttpContext.UserId())
                    );

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> RejectSchoolImageAsync([NotNull] ISpecification<SchoolImage> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<SchoolImage>().GetManyQueryable(specification)
                    .ExecuteUpdateAsync(t => t
                        .SetProperty(p => p.Status, Status.Rejected)
                        .SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow)
                        .SetProperty(p => p.LastModifyUserId, HttpContextAccessor.Value.HttpContext.UserId())
                    );

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<long>> CreateSchoolImageAsync([NotNull] CreateSchoolImageRequestDto requestDto)
        {
            try
            {
                using MemoryStream stream = new();
                await requestDto.File.CopyToAsync(stream);

                var fileId = await fileService.Value.UploadFileAsync(new()
                {
                    File = stream.ToArray(),
                    ContainerType = ContainerType.School,
                    FileExtension = Path.GetExtension(requestDto.File.FileName),
                });
                if (fileId.OperationResult is not OperationResult.Succeeded)
                {
                    return new(fileId.OperationResult) { Errors = fileId.Errors, };
                }

                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var imageRepository = uow.GetRepository<SchoolImage>();
                var schoolImage = new SchoolImage
                {
                    FileId = fileId.Data,
                    SchoolId = requestDto.SchoolId,
                    FileType = requestDto.FileType,
                    CreationDate = requestDto.CreationDate,
                    CreationUserId = requestDto.CreationUserId,
                    Status = Status.Draft,
                };

                await TryExtractGpsDataAsync();
                imageRepository.Add(schoolImage);
                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = schoolImage.Id };

                async Task TryExtractGpsDataAsync()
                {
                    try
                    {
                        var gps = ImageMetadataReader.ReadMetadata(stream)
                             .OfType<GpsDirectory>()
                             .FirstOrDefault()?.GetGeoLocation();
                        if (gps is not null)
                        {
                            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                            var point = geometryFactory.CreatePoint(new Coordinate(gps.Longitude, gps.Latitude));

                            var schoolRepository = uow.GetRepository<School, long>();
                            var schoolCoordinates = await schoolRepository.GetManyQueryable(t => t.Id == requestDto.SchoolId).Select(t => t.Coordinates).FirstOrDefaultAsync();
                            if (schoolCoordinates is not null && schoolCoordinates.Distance(point) < 2000)
                            {
                                schoolImage.Status = Status.Confirmed;
                            }
                        }
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        #endregion

        #region Contribution

        public async Task<ResultData<long>> ManageSchoolContributionAsync([NotNull] ManageSchoolContributionRequestDto requestDto)
        {
            var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
            var exist = await uow.GetRepository<School, long>().AnyAsync(t => t.Id == requestDto.SchoolId);
            if (!exist)
            {
                return new(OperationResult.Failed) { Errors = [new() { Message = "School not found", },] };
            }

            if (requestDto.Id.HasValue)
            {
                var specification = new IdEqualsSpecification<Contribution, long>(requestDto.Id.Value)
                    .And(new IdentifierIdEqualsSpecification(requestDto.SchoolId))
                    .And(new CreationUserIdEqualsSpecification<Contribution, int>(requestDto.UserId))
                    .And(new ContributionTypeEqualsSpecification(ContributionType.School))
                    .And(new StatusEqualsSpecification<Contribution>(Status.Draft).Or(new StatusEqualsSpecification<Contribution>(Status.Rejected)));
                var data = await contributionService.Value.ExistContributionAsync(specification);
                if (!data.Data)
                {
                    return new(data.OperationResult) { Errors = data.Errors };
                }
            }

            var contributionResult = await contributionService.Value.ManageContributionAsync(new ManageContributionRequestDto
            {
                ContributionType = ContributionType.School,
                IdentifierId = requestDto.SchoolId,
                Status = Status.Draft,
                Data = JsonSerializer.Serialize(requestDto.Data),
                Id = requestDto.Id,
            });
            if (contributionResult.OperationResult is not OperationResult.Succeeded)
            {
                return new(contributionResult.OperationResult) { Errors = contributionResult.Errors };
            }

            var hasAutoConfirmSchoolContribution = await identityService.Value.HasClaimAsync(requestDto.UserId, SystemClaim.AutoConfirmSchoolContribution);
            if (hasAutoConfirmSchoolContribution.Data)
            {
                _ = await ConfirmSchoolContributionAsync(new()
                {
                    ContributionId = contributionResult.Data,
                    SchoolId = requestDto.SchoolId,
                    Data = requestDto.Data,
                });
            }

            return new(OperationResult.Succeeded) { Data = contributionResult.Data };
        }

        public async Task<ResultData<bool>> ConfirmSchoolContributionAsync([NotNull] ConfirmSchoolContributionRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var school = await uow.GetRepository<School, long>().GetAsync(requestDto.SchoolId);
                if (school is null)
                {
                    return new(OperationResult.Failed) { Errors = [new() { Message = "School not found", },] };
                }

                var result = await contributionService.Value.ConfirmContributionAsync(new()
                {
                    ContributionId = requestDto.ContributionId,
                    ContributionType = ContributionType.School,
                    IdentifierId = requestDto.SchoolId,
                });
                if (result.OperationResult is not OperationResult.Succeeded)
                {
                    return new(OperationResult.Failed) { Errors = result.Errors };
                }

                ManageSchoolRequestDto manageSchoolRequestDto = new()
                {
                    Address = requestDto.Data.Address,
                    CityId = requestDto.Data.CityId,
                    CountryId = requestDto.Data.CountryId,
                    Email = requestDto.Data.Email,
                    Facilities = requestDto.Data.Facilities,
                    FaxNumber = requestDto.Data.FaxNumber,
                    LocalAddress = requestDto.Data.LocalAddress,
                    LocalName = requestDto.Data.LocalName,
                    Name = requestDto.Data.Name,
                    PhoneNumber = requestDto.Data.PhoneNumber,
                    Quarter = requestDto.Data.Quarter,
                    SchoolType = requestDto.Data.SchoolType,
                    StateId = requestDto.Data.StateId,
                    WebSite = requestDto.Data.WebSite,
                    ZipCode = requestDto.Data.ZipCode,
                    Id = requestDto.SchoolId,
                };
                if (requestDto.Data.Latitude.HasValue && requestDto.Data.Longitude.HasValue)
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
                    manageSchoolRequestDto.Coordinates = geometryFactory.CreatePoint(new Coordinate(requestDto.Data.Longitude.Value, requestDto.Data.Latitude.Value));
                }
                var manageSchoolResult = await ManageSchoolAsync(manageSchoolRequestDto, true);

                return manageSchoolResult.OperationResult is OperationResult.Succeeded
                    ? new(OperationResult.Failed) { Errors = result.Errors }
                    : new(OperationResult.Succeeded) { Data = true };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        #endregion
    }
}
