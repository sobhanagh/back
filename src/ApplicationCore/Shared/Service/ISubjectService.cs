namespace GamaEdtech.Backend.Shared.Service
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Data.Dto.Subject;
    using GamaEdtech.Backend.Data.Entity;

    [Injectable]
    public interface ISubjectService
    {
        Task<ResultData<ListDataSource<SubjectsDto>>> GetSubjectsAsync(ListRequestDto<Subject>? requestDto = null);
        Task<ResultData<SubjectDto>> GetSubjectAsync([NotNull] ISpecification<Subject> specification);
        Task<ResultData<int>> ManageSubjectAsync([NotNull] ManageSubjectRequestDto requestDto);
        Task<ResultData<bool>> RemoveSubjectAsync([NotNull] ISpecification<Subject> specification);
    }
}
