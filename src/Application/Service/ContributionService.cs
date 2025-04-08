namespace GamaEdtech.Application.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Core.Extensions.Linq;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.UnitOfWork;
    using GamaEdtech.Common.Service;
    using GamaEdtech.Data.Dto.Contribution;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using static GamaEdtech.Common.Core.Constants;

    public class ContributionService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<ContributionService>> localizer
        , Lazy<ILogger<ContributionService>> logger)
        : LocalizableServiceBase<ContributionService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), IContributionService
    {
        public async Task<ResultData<ListDataSource<ContributionsDto>>> GetContributionsAsync(ListRequestDto<Contribution>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<Contribution>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new ContributionsDto
                {
                    Id = t.Id,
                    Comment = t.Comment,
                    ContributionType = t.ContributionType,
                    IdentifierId = t.IdentifierId,
                    Status = t.Status,
                    CreationUser = t.CreationUser!.FirstName + " " + t.CreationUser.LastName,
                    CreationDate = t.CreationDate,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<ContributionDto>> GetContributionAsync([NotNull] ISpecification<Contribution> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var contribution = await uow.GetRepository<Contribution>().GetManyQueryable(specification).Select(t => new ContributionDto
                {
                    Id = t.Id,
                    Comment = t.Comment,
                    Data = t.Data,
                    IdentifierId = t.IdentifierId,
                    ContributionType = t.ContributionType,
                }).FirstOrDefaultAsync();

                return contribution is null
                    ? new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["ContributionNotFound"] },],
                    }
                    : new(OperationResult.Succeeded) { Data = contribution };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<long>> ManageContributionAsync([NotNull] ManageContributionRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Contribution>();
                Contribution? contribution = null;

                if (requestDto.Id.HasValue)
                {
                    contribution = await repository.GetAsync(requestDto.Id.Value);
                    if (contribution is null)
                    {
                        return new(OperationResult.NotFound)
                        {
                            Errors = [new() { Message = Localizer.Value["ContributionNotFound"] },],
                        };
                    }

                    contribution.Comment = requestDto.Comment;
                    contribution.ContributionType = requestDto.ContributionType;
                    contribution.Data = requestDto.Data;
                    contribution.IdentifierId = requestDto.IdentifierId;
                    contribution.Status = requestDto.Status;

                    _ = repository.Update(contribution);
                }
                else
                {
                    contribution = new Contribution
                    {
                        Comment = requestDto.Comment,
                        ContributionType = requestDto.ContributionType,
                        Data = requestDto.Data,
                        IdentifierId = requestDto.IdentifierId,
                        Status = requestDto.Status,
                    };
                    repository.Add(contribution);
                }

                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = contribution.Id };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> ExistContributionAsync([NotNull] ISpecification<Contribution> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Contribution>();

                return new(OperationResult.Succeeded) { Data = await repository.AnyAsync(specification) };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<ContributionDto>> ConfirmContributionAsync([NotNull] ConfirmContributionRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Contribution>();
                var contribution = await repository.GetAsync(requestDto.ContributionId);
                if (contribution is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["ContributionNotFound"] },],
                    };
                }

                contribution.Status = Status.Confirmed;
                _ = repository.Update(contribution);
                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded)
                {
                    Data = new ContributionDto
                    {
                        Id = contribution.Id,
                        Data = contribution.Data,
                        Comment = contribution.Comment,
                        CreationUserId = contribution.CreationUserId,
                        CreationDate = contribution.CreationDate,
                    }
                };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> RejectContributionAsync([NotNull] RejectContributionRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Contribution>();
                var userId = HttpContextAccessor.Value.HttpContext?.User.UserId();
                var affectedRows = await repository.GetManyQueryable(t => t.Id == requestDto.Id).ExecuteUpdateAsync(t => t
                    .SetProperty(p => p.Status, Status.Rejected)
                    .SetProperty(p => p.Comment, requestDto.Comment)
                    .SetProperty(p => p.LastModifyUserId, userId)
                    .SetProperty(p => p.LastModifyDate, DateTimeOffset.UtcNow));

                return new(OperationResult.Succeeded) { Data = affectedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }
    }
}
