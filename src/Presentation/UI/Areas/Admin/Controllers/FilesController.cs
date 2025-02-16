namespace GamaEdtech.Backend.UI.Web.Areas.Admin.Controllers
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using GamaEdtech.Backend.Common.Core;
    using GamaEdtech.Backend.Common.Data;

    using GamaEdtech.Backend.Data.Dto.File;
    using GamaEdtech.Backend.Data.ViewModel.File;
    using GamaEdtech.Backend.Shared.Service;

    using Microsoft.AspNetCore.Mvc;

    [Common.DataAnnotation.Area(nameof(Admin), "Admin")]
    [Route("api/v{version:apiVersion}/[area]/[controller]")]
    [ApiVersion("1.0")]
    //[Permission(Roles = [nameof(Role.Admin)])]
    public class FilesController(Lazy<ILogger<FilesController>> logger, Lazy<IFileService> fileService) : ApiControllerBase<FilesController>(logger)
    {
        [HttpPost, Produces(typeof(ApiResponse<CreateFileResponseViewModel>))]
        public async Task<IActionResult> CreateFile([NotNull, FromForm] CreateFileRequestViewModel request)
        {
            try
            {
                var result = await fileService.Value.CreateFileAsync(new CreateFileRequestDto
                {
                    File = request.File,
                });
                return Ok(new ApiResponse<CreateFileResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new()
                    {
                        FileId = result.Data?.FileId,
                        PreviewId1 = result.Data?.PreviewId1,
                        PreviewId2 = result.Data?.PreviewId2,
                        PreviewId3 = result.Data?.PreviewId3,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<CreateFileResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }
    }
}
