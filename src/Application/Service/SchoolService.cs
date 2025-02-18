namespace GamaEdtech.Application.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using EntityFramework.Exceptions.Common;

    using GamaEdtech.Common.Core.Extensions.Linq;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.UnitOfWork;
    using GamaEdtech.Common.Service;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Data.Dto.School;
    using GamaEdtech.Domain.Entity;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using static GamaEdtech.Common.Core.Constants;
    using GamaEdtech.Application.Interface;

    public class SchoolService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<FileService>> localizer
        , Lazy<ILogger<FileService>> logger)
        : LocalizableServiceBase<FileService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), ISchoolService
    {
        public async Task<ResultData<ListDataSource<SchoolsDto>>> GetSchoolsAsync(ListRequestDto<School>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<School, int>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
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

        public async Task<ResultData<SchoolDto>> GetSchoolAsync([NotNull] ISpecification<School> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var school = await uow.GetRepository<School, int>().GetManyQueryable(specification).Select(t => new SchoolDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    LocalName = t.LocalName,
                    Address = t.Address,
                    LocalAddress = t.LocalAddress,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    SchoolType = t.SchoolType!,
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

        public async Task<ResultData<int>> ManageSchoolAsync([NotNull] ManageSchoolRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<School, int>();
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

                    school.Name = requestDto.Name;
                    school.LocalName = requestDto.LocalName;
                    school.Address = requestDto.Address;
                    school.Latitude = requestDto.Latitude;
                    school.Longitude = requestDto.Longitude;
                    school.SchoolType = requestDto.SchoolType;
                    school.StateId = requestDto.StateId;
                    school.ZipCode = requestDto.ZipCode;
                    school.WebSite = requestDto.WebSite;
                    school.Quarter = requestDto.Quarter;
                    school.PhoneNumber = requestDto.PhoneNumber;
                    school.LocalAddress = requestDto.LocalAddress;
                    school.FaxNumber = requestDto.FaxNumber;
                    school.Facilities = requestDto.Facilities;
                    school.Email = requestDto.Email;
                    school.CityId = requestDto.CityId;
                    school.CountryId = requestDto.CountryId;
                    school.OsmId = requestDto.OsmId;

                    _ = repository.Update(school);
                }
                else
                {
                    school = new School
                    {
                        Name = requestDto.Name,
                        LocalName = requestDto.LocalName,
                        Address = requestDto.Address,
                        Latitude = requestDto.Latitude,
                        Longitude = requestDto.Longitude,
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
                var school = await uow.GetRepository<School, int>().GetAsync(specification);
                if (school is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Data = false,
                        Errors = [new() { Message = Localizer.Value["SchoolNotFound"] },],
                    };
                }

                uow.GetRepository<School, int>().Remove(school);
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
    }
}
