namespace GamaEdtech.Backend.Shared.Service
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Data.Dto.School;
    using GamaEdtech.Backend.Data.Entity;

    [Injectable]
    public interface ISchoolService
    {
        Task<ResultData<ListDataSource<SchoolsDto>>> GetSchoolsAsync(ListRequestDto<School>? requestDto = null);
        Task<ResultData<SchoolDto>> GetSchoolAsync([NotNull] ISpecification<School> specification);
        Task<ResultData<int>> ManageSchoolAsync([NotNull] ManageSchoolRequestDto requestDto);
        Task<ResultData<bool>> RemoveSchoolAsync([NotNull] ISpecification<School> specification);
    }
}
