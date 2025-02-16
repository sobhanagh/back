namespace GamaEdtech.Backend.Shared.Service
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Specification;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Data.Dto.Topic;
    using GamaEdtech.Backend.Data.Entity;

    [Injectable]
    public interface ITopicService
    {
        Task<ResultData<ListDataSource<TopicsDto>>> GetTopicsAsync(ListRequestDto<Topic>? requestDto = null);
        Task<ResultData<TopicDto>> GetTopicAsync([NotNull] ISpecification<Topic> specification);
        Task<ResultData<int>> ManageTopicAsync([NotNull] ManageTopicRequestDto requestDto);
        Task<ResultData<bool>> RemoveTopicAsync([NotNull] ISpecification<Topic> specification);
    }
}
