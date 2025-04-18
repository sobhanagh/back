namespace GamaEdtech.Presentation.Api.Controllers
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.Specification.Impl;
    using GamaEdtech.Common.Identity;
    using GamaEdtech.Data.Dto.Blog;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Specification.Post;
    using GamaEdtech.Presentation.ViewModel.Blog;
    using GamaEdtech.Presentation.ViewModel.Tag;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BlogsController(Lazy<ILogger<BlogsController>> logger, Lazy<IBlogService> blogService)
        : ApiControllerBase<BlogsController>(logger)
    {
        [HttpGet("posts"), Produces<ApiResponse<ListDataSource<PostsResponseViewModel>>>()]
        public async Task<IActionResult<ListDataSource<PostsResponseViewModel>>> GetPosts([NotNull, FromQuery] PostsRequestViewModel request)
        {
            try
            {
                ISpecification<Post>? specification = request.TagId.HasValue
                    ? new TagIncludedSpecification(request.TagId.Value)
                    : null;
                var result = await blogService.Value.GetPostsAsync(new ListRequestDto<Post>
                {
                    PagingDto = request.PagingDto,
                    Specification = specification,
                });
                return Ok<ListDataSource<PostsResponseViewModel>>(new(result.Errors)
                {
                    Data = result.Data.List is null ? new() : new()
                    {
                        List = result.Data.List.Select(t => new PostsResponseViewModel
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Summary = t.Summary,
                            LikeCount = t.LikeCount,
                            DislikeCount = t.DislikeCount,
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

        [HttpGet("posts/{postId:long}"), Produces<ApiResponse<PostResponseViewModel>>()]
        public async Task<IActionResult<PostResponseViewModel>> GetPost([FromRoute] long postId)
        {
            try
            {
                var result = await blogService.Value.GetPostAsync(new IdEqualsSpecification<Post, long>(postId));

                return Ok<PostResponseViewModel>(new(result.Errors)
                {
                    Data = result.Data is null ? null : new()
                    {
                        Title = result.Data.Title,
                        Summary = result.Data.Summary,
                        Body = result.Data.Body,
                        ImageUri = result.Data.ImageUri,
                        LikeCount = result.Data.LikeCount,
                        DislikeCount = result.Data.DislikeCount,
                        CreationUser = result.Data.CreationUser,
                        Tags = result.Data.Tags?.Select(t => new TagResponseViewModel
                        {
                            Id = t.Id,
                            Icon = t.Icon,
                            Name = t.Name,
                            TagType = t.TagType,
                        }),
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<PostResponseViewModel>(new(new Error { Message = exc.Message }));
            }
        }

        [HttpPost("posts"), Produces<ApiResponse<ManagePostResponseViewModel>>()]
        [Permission(policy: null)]
        public async Task<IActionResult> CreatePost([NotNull] ManagePostRequestViewModel request)
        {
            try
            {
                var result = await blogService.Value.ManagePostAsync(MapTo(request, null));

                return Ok(new ApiResponse<ManagePostResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new() { Id = result.Data, },
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ManagePostResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPut("posts/{postId:long}"), Produces<ApiResponse<ManagePostResponseViewModel>>()]
        [Permission(policy: null)]
        public async Task<IActionResult> UpdatePost([FromRoute] long postId, [NotNull, FromBody] ManagePostRequestViewModel request)
        {
            try
            {
                var dto = MapTo(request, postId);
                var result = await blogService.Value.ManagePostAsync(dto);

                return Ok(new ApiResponse<ManagePostResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new() { Id = result.Data, },
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ManagePostResponseViewModel> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpDelete("posts/{postId:long}"), Produces<ApiResponse<bool>>()]
        [Permission(policy: null)]
        public async Task<IActionResult> RemovePost([FromRoute] long postId)
        {
            try
            {
                var result = await blogService.Value.RemovePostAsync(new IdEqualsSpecification<Post, long>(postId));
                return Ok(new ApiResponse<bool>
                {
                    Errors = result.Errors,
                    Data = result.Data
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = [new() { Message = exc.Message }] });
            }
        }

        [HttpPatch("posts/{postId:long}/like"), Produces<ApiResponse<bool>>()]
        [Permission(policy: null)]
        public async Task<IActionResult<bool>> LikePost([FromRoute] long postId)
        {
            try
            {
                var result = await blogService.Value.LikePostAsync(new()
                {
                    PostId = postId,
                });
                return Ok<bool>(new(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<bool>(new(new Error { Message = exc.Message }));
            }
        }

        [HttpPatch("posts/{postId:long}/dislike"), Produces<ApiResponse<bool>>()]
        [Permission(policy: null)]
        public async Task<IActionResult<bool>> DislikePost([FromRoute] long postId)
        {
            try
            {
                var result = await blogService.Value.DislikePostAsync(new()
                {
                    PostId = postId,
                });
                return Ok<bool>(new(result.Errors)
                {
                    Data = result.Data,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<bool>(new(new Error { Message = exc.Message }));
            }
        }

        private static ManagePostRequestDto MapTo(ManagePostRequestViewModel request, long? id) => new()
        {
            Id = id,
            Title = request.Title,
            Summary = request.Summary,
            Body = request.Body,
            Image = request.Image,
            Tags = request.Tags,
        };
    }
}
