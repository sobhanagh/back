namespace GamaEdtech.Application.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using EntityFramework.Exceptions.Common;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Core.Extensions.Linq;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Common.DataAccess.Specification.Impl;
    using GamaEdtech.Common.DataAccess.UnitOfWork;
    using GamaEdtech.Common.Service;
    using GamaEdtech.Data.Dto.Blog;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Domain.Specification;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using static GamaEdtech.Common.Core.Constants;

    public class BlogService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<BlogService>> localizer
        , Lazy<ILogger<BlogService>> logger, Lazy<IReactionService> reactionService, Lazy<IFileService> fileService)
        : LocalizableServiceBase<BlogService>(unitOfWorkProvider, httpContextAccessor, localizer, logger), IBlogService
    {
        public async Task<ResultData<ListDataSource<PostsDto>>> GetPostsAsync(ListRequestDto<Post>? requestDto = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var result = await uow.GetRepository<Post>().GetManyQueryable(requestDto?.Specification).FilterListAsync(requestDto?.PagingDto);
                var users = await result.List.Select(t => new PostsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Summary = t.Summary,
                    LikeCount = t.LikeCount,
                    DislikeCount = t.DislikeCount,
                }).ToListAsync();
                return new(OperationResult.Succeeded) { Data = new() { List = users, TotalRecordsCount = result.TotalRecordsCount } };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<PostDto>> GetPostAsync([NotNull] ISpecification<Post> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var post = await uow.GetRepository<Post>().GetManyQueryable(specification).Select(t => new
                {
                    t.Title,
                    t.Body,
                    t.Summary,
                    t.DislikeCount,
                    t.LikeCount,
                    t.ImageId,
                    CreationUser = t.CreationUser.FirstName + " " + t.CreationUser.LastName,
                }).FirstOrDefaultAsync();
                if (post is null)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["PostNotFound"] },],
                    };
                }

                PostDto result = new()
                {
                    Body = post.Body,
                    DislikeCount = post.DislikeCount,
                    LikeCount = post.LikeCount,
                    Summary = post.Summary,
                    Title = post.Title,
                    ImageUri = fileService.Value.GetFileUri(post.ImageId!, ContainerType.Post).Data,
                };

                return new(OperationResult.Succeeded) { Data = result };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message },] };
            }
        }

        public async Task<ResultData<long>> ManagePostAsync([NotNull] ManagePostRequestDto requestDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<Post>();
                Post? post = null;

                string? imageId = null;
                if (requestDto.Image is not null)
                {
                    using MemoryStream stream = new();
                    await requestDto.Image.CopyToAsync(stream);

                    var fileId = await fileService.Value.UploadFileAsync(new()
                    {
                        File = stream.ToArray(),
                        ContainerType = ContainerType.Post,
                        FileExtension = Path.GetExtension(requestDto.Image.FileName),
                    });
                    if (fileId.OperationResult is not OperationResult.Succeeded)
                    {
                        return new(fileId.OperationResult) { Errors = fileId.Errors, };
                    }

                    imageId = fileId.Data;
                }

                if (requestDto.Id.HasValue)
                {
                    post = await repository.GetAsync(requestDto.Id.Value);
                    if (post is null)
                    {
                        return new(OperationResult.NotFound)
                        {
                            Errors = [new() { Message = Localizer.Value["PostNotFound"] },],
                        };
                    }

                    post.Title = requestDto.Title;
                    post.Summary = requestDto.Summary;
                    post.Body = requestDto.Body;
                    post.ImageId = imageId;

                    _ = repository.Update(post);
                }
                else
                {
                    post = new()
                    {
                        Title = requestDto.Title,
                        Summary = requestDto.Summary,
                        Body = requestDto.Body,
                        ImageId = imageId,
                        Status = Status.Confirmed,
                    };
                    repository.Add(post);
                }

                _ = await uow.SaveChangesAsync();

                return new(OperationResult.Succeeded) { Data = post.Id };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, }] };
            }
        }

        public async Task<ResultData<bool>> LikePostAsync([NotNull] PostReactionRequestDto requestDto)
        {
            try
            {
                var specification = new IdEqualsSpecification<Post, long>(requestDto.PostId);
                var exist = await PostExistAsync(specification);
                if (!exist.Data)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["InvalidRequest"] },],
                    };
                }

                var reactionResult = await reactionService.Value.ManageReactionAsync(new()
                {
                    CategoryType = CategoryType.Post,
                    CreationDate = DateTimeOffset.UtcNow,
                    CreationUserId = HttpContextAccessor.Value.HttpContext.UserId(),
                    IdentifierId = requestDto.PostId,
                    IsLike = true,
                });
                if (reactionResult.OperationResult is not OperationResult.Succeeded)
                {
                    return new(OperationResult.Failed) { Errors = reactionResult.Errors };
                }

                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var updatedRows = await uow.GetRepository<Post>().GetManyQueryable(t => t.Id == requestDto.PostId)
                    .ExecuteUpdateAsync(t => t.SetProperty(p => p.LikeCount, p => p.LikeCount + 1));

                return new(OperationResult.Succeeded) { Data = updatedRows > 0 };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> DislikePostAsync([NotNull] PostReactionRequestDto requestDto)
        {
            try
            {
                var specification = new IdEqualsSpecification<Post, long>(requestDto.PostId);
                var exist = await PostExistAsync(specification);
                if (!exist.Data)
                {
                    return new(OperationResult.NotFound)
                    {
                        Errors = [new() { Message = Localizer.Value["InvalidRequest"] },],
                    };
                }

                var reactionResult = await reactionService.Value.ManageReactionAsync(new()
                {
                    CategoryType = CategoryType.Post,
                    CreationDate = DateTimeOffset.UtcNow,
                    CreationUserId = HttpContextAccessor.Value.HttpContext.UserId(),
                    IdentifierId = requestDto.PostId,
                    IsLike = false,
                });
                if (reactionResult.OperationResult is not OperationResult.Succeeded)
                {
                    return new(OperationResult.Failed) { Errors = reactionResult.Errors };
                }

                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                _ = await uow.GetRepository<SchoolComment>().GetManyQueryable(t => t.Id == requestDto.PostId)
                    .ExecuteUpdateAsync(t => t.SetProperty(p => p.DislikeCount, p => p.DislikeCount + 1));

                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> RemovePostAsync([NotNull] long postId)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var postRepository = uow.GetRepository<Post>();
                var imageId = await postRepository.GetManyQueryable(t => t.Id == postId).Select(t => t.ImageId).FirstOrDefaultAsync();

                //remove post
                _ = await postRepository.GetManyQueryable(t => t.Id == postId).ExecuteDeleteAsync();

                //remove reactions
                var reactionSpecification = new IdentifierIdEqualsSpecification<Reaction>(postId)
                    .And(new CategoryTypeEqualsSpecification<Reaction>(CategoryType.Post));
                _ = reactionService.Value.RemoveReactionAsync(reactionSpecification);

                if (imageId != null)
                {
                    //remove image
                    _ = await fileService.Value.RemoveFileAsync(new()
                    {
                        ContainerType = ContainerType.Post,
                        FileId = imageId,
                    });
                }

                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (ReferenceConstraintException)
            {
                return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["PostCantBeRemoved"], },] };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> PostExistAsync([NotNull] ISpecification<Post> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var exist = await uow.GetRepository<Post>().AnyAsync(specification);

                return new(OperationResult.Succeeded) { Data = exist };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }
    }
}
