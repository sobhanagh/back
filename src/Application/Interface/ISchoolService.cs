namespace GamaEdtech.Application.Interface
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Data.Dto.School;
    using GamaEdtech.Domain.Entity;

    using NetTopologySuite.Geometries;

    [Injectable]
    public interface ISchoolService
    {
        Task<ResultData<ListDataSource<SchoolsDto>>> GetSchoolsAsync(ListRequestDto<School>? requestDto = null);
        Task<ResultData<ListDataSource<SchoolInfoDto>>> GetSchoolsListAsync(ListRequestDto<School>? requestDto = null, Point? point = null);
        Task<ResultData<SchoolDto>> GetSchoolAsync([NotNull] ISpecification<School> specification);
        Task<ResultData<long>> ManageSchoolAsync([NotNull] ManageSchoolRequestDto requestDto, bool ignoreNullValues);
        Task<ResultData<bool>> RemoveSchoolAsync([NotNull] ISpecification<School> specification);
        Task<ResultData<bool>> ExistSchoolAsync([NotNull] ISpecification<School> specification);

        Task<ResultData<SchoolRateDto>> GetSchoolRateAsync([NotNull] ISpecification<SchoolComment> specification);
        Task<ResultData<ListDataSource<SchoolCommentDto>>> GetSchoolCommentsAsync(ListRequestDto<SchoolComment>? requestDto = null);
        Task<ResultData<bool>> LikeSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification);
        Task<ResultData<bool>> DislikeSchoolCommentAsync([NotNull] ISpecification<SchoolComment> specification);
        Task<ResultData<long>> ManageSchoolCommentContributionAsync([NotNull] ManageSchoolCommentContributionRequestDto requestDto);
        Task<ResultData<bool>> ConfirmSchoolCommentContributionAsync([NotNull] ConfirmSchoolCommentContributionRequestDto requestDto);

        Task<ResultData<ListDataSource<SchoolImageDto>>> GetSchoolImagesAsync(ListRequestDto<SchoolImage>? requestDto = null);
        Task<ResultData<IEnumerable<string?>>> GetSchoolImagesPathAsync([NotNull] ISpecification<SchoolImage> specification);
        Task<ResultData<long>> ManageSchoolImageContributionAsync([NotNull] ManageSchoolImageContributionRequestDto requestDto);
        Task<ResultData<bool>> ConfirmSchoolImageContributionAsync([NotNull] ConfirmSchoolImageContributionRequestDto requestDto);

        Task<ResultData<long>> ManageSchoolContributionAsync([NotNull] ManageSchoolContributionRequestDto requestDto);
        Task<ResultData<bool>> ConfirmSchoolContributionAsync([NotNull] ConfirmSchoolContributionRequestDto requestDto);

        Task<ResultData<bool>> UpdateAllSchoolScoreAsync(long? schoolId = null);
    }
}
