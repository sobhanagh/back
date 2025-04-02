namespace GamaEdtech.Application.Interface
{
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.DataAnnotation;

    using GamaEdtech.Data.Dto.Contribution;
    using GamaEdtech.Domain.Entity;

    [Injectable]
    public interface IContributionService
    {
        Task<ResultData<ListDataSource<ContributionsDto>>> GetContributionsAsync(ListRequestDto<Contribution>? requestDto = null);
        Task<ResultData<ContributionDto>> GetContributionAsync([NotNull] ISpecification<Contribution> specification);
        Task<ResultData<long>> ManageContributionAsync([NotNull] ManageContributionRequestDto requestDto);
    }
}
