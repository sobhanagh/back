namespace GamaEdtech.Backend.Shared.Service
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Data.Dto.File;

    [Injectable]
    public interface IFileService
    {
        Task<ResultData<CreateFileResponseDto>> CreateFileAsync([NotNull] CreateFileRequestDto requestDto);
    }
}
