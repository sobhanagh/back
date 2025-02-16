namespace GamaEdtech.Backend.Shared.Service
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Data.Dto.Grade;
    using GamaEdtech.Backend.Data.Entity;

    [Injectable]
    public interface IGradeService
    {
        Task<ResultData<ListDataSource<GradesDto>>> GetGradesAsync(ListRequestDto<Grade>? requestDto = null);
        Task<ResultData<GradeDto>> GetGradeAsync([NotNull] ISpecification<Grade> specification);
        Task<ResultData<int>> ManageGradeAsync([NotNull] ManageGradeRequestDto requestDto);
        Task<ResultData<bool>> RemoveGradeAsync([NotNull] ISpecification<Grade> specification);
    }
}
