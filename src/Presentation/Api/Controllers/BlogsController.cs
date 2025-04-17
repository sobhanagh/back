namespace GamaEdtech.Presentation.Api.Controllers
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Presentation.ViewModel.Blog;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BlogsController(Lazy<ILogger<BlogsController>> logger, Lazy<IBlogService> blogService)
        : ApiControllerBase<BlogsController>(logger)
    {
        [HttpGet, Produces<ApiResponse<ListDataSource<PostsResponseViewModel>>>()]
        public async Task<IActionResult<ListDataSource<PostsResponseViewModel>>> GetPosts([NotNull, FromQuery] PostsRequestViewModel request)
        {
            try
            {
                var result = await blogService.Value.GetPostsAsync(new ListRequestDto<Post>
                {
                    PagingDto = request.PagingDto,
                });
                return Ok<ListDataSource<PostsResponseViewModel>>(new(result.Errors)
                {
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new PostsResponseViewModel
                        {
                            Title = t.Title,
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<ListDataSource<PostsResponseViewModel>>(new(new Error { Message = exc.Message }));
            }
        }
    }
}
