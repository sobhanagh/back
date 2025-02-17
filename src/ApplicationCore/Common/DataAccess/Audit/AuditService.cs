namespace GamaEdtech.Backend.Common.DataAccess.Audit
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using GamaEdtech.Backend.Common.Core;
    using GamaEdtech.Backend.Common.Core.Extensions.Linq;
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using GamaEdtech.Backend.Common.DataAccess.UnitOfWork;
    using GamaEdtech.Backend.Common.Service;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using NUlid;

    using static GamaEdtech.Backend.Common.Core.Constants;

    public class AuditService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<ILogger<AuditService>> logger) : ServiceBase<AuditService>(unitOfWorkProvider, httpContextAccessor, logger), IAuditService
    {
        public async Task<ResultData<ListDataSource<AuditListDto>>> GetAuditsAsync(ListRequestDto<Audit>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();

                requestDto ??= new();
                requestDto.PagingDto ??= new();
                requestDto.PagingDto.SortFilter = [new SortFilter { Column = nameof(Audit.Date), SortType = SortType.Desc }];

                var result = await uow.GetRepository<Audit, Ulid>().GetManyQueryable(requestDto.Specification).FilterListAsync(requestDto.PagingDto);
                var audits = await result.List.Select(t => new AuditListDto
                {
                    Id = t.Id,
                    User = t.UserName + " (" + t.UserId + ")",
                    Date = t.Date,
                    IpAddress = t.IpAddress,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new ListDataSource<AuditListDto> { List = audits, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new Error { Message = exc.Message }] };
            }
        }

        public async Task<ResultData<AuditDto>> GetAsync(ISpecification<Audit> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var audit = await uow.GetRepository<Audit, Ulid>().GetManyQueryable(specification).Select(t => new AuditDto
                {
                    Id = t.Id,
                    User = t.UserName + " (" + t.UserId + ")",
                    Date = t.Date,
                    IpAddress = t.IpAddress,
                    UserAgent = t.UserAgent,
                    AuditEntries = t.AuditEntries!.Select(u => new AuditEntryDto
                    {
                        Id = u.Id,
                        AuditId = u.AuditId,
                        AuditType = u.AuditType,
                        EntityType = u.EntityType,
                        IdentifierId = u.IdentifierId,
                        AuditEntryProperties = u.AuditEntryProperties!.Select(v => new AuditEntryPropertyDto
                        {
                            Id = v.Id,
                            AuditEntryId = v.AuditEntryId,
                            PropertyName = v.PropertyName,
                            NewValue = v.NewValue,
                            OldValue = v.OldValue,
                        }).ToList(),
                    }).ToList(),
                }).FirstOrDefaultAsync();
                return audit is null
                    ? new(OperationResult.NotFound) { Errors = new[] { new Error { Message = Resources.GlobalResource.Validation_NotFound } } }
                    : new(OperationResult.Succeeded) { Data = audit };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new Error { Message = exc.Message }] };
            }
        }
    }
}
