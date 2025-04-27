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
    using GamaEdtech.Data.Dto.Contact;
    using GamaEdtech.Domain.Entity;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using static GamaEdtech.Common.Core.Constants;
    using GamaEdtech.Application.Interface;

    public class ContactService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<ContactService>> localizer
        , Lazy<ILogger<ContactService>> logger)
        : LocalizableServiceBase<ContactService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), IContactService
    {
        public async Task<ResultData<ListDataSource<ContactsDto>>> GetContactsAsync(ListRequestDto<Contact>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<Contact, int>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new ContactsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Icon = t.Icon,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<ContactDto>> GetContactAsync([NotNull] ISpecification<Contact> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var contact = await uow.GetRepository<Contact, int>().GetManyQueryable(specification).Select(t => new ContactDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Icon = t.Icon,
                }).FirstOrDefaultAsync();

                return contact is null
                    ? new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["ContactNotFound"] },],
                    }
                    : new(OperationResult.Succeeded) { Data = contact };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<int>> ManageContactAsync([NotNull] ManageContactRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Contact, int>();
                Contact? contact = null;

                if (requestDto.Id.HasValue)
                {
                    contact = await repository.GetAsync(requestDto.Id.Value);
                    if (contact is null)
                    {
                        return new(OperationResult.NotFound)
                        {
                            Errors = [new() { Message = Localizer.Value["ContactNotFound"] },],
                        };
                    }

                    contact.Title = requestDto.Title;
                    contact.Description = requestDto.Description;
                    contact.Icon = requestDto.Icon;

                    _ = repository.Update(contact);
                }
                else
                {
                    contact = new Contact
                    {
                        Title = requestDto.Title,
                        Description = requestDto.Description,
                        Icon = requestDto.Icon,
                    };
                    repository.Add(contact);
                }

                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = contact.Id };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> RemoveContactAsync([NotNull] ISpecification<Contact> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var contact = await uow.GetRepository<Contact, int>().GetAsync(specification);
                if (contact is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Data = false,
                        Errors = [new() { Message = Localizer.Value["ContactNotFound"] },],
                    };
                }

                uow.GetRepository<Contact, int>().Remove(contact);
                _ = await uow.SaveChangesAsync();
                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["ContactCantBeRemoved"], },] };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }
    }
}
