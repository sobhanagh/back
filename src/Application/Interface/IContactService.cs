namespace GamaEdtech.Application.Interface
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Data.Dto.Contact;

    [Injectable]
    public interface IContactService
    {
        Task<ResultData<long>> CreateContactAsync([NotNull] CreateContactRequestDto requestDto);
    }
}
