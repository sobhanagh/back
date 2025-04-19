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
    using GamaEdtech.Data.Dto.Tag;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Domain.Specification;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    using static GamaEdtech.Common.Core.Constants;

    public class BlogService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<IStringLocalizer<BlogService>> localizer
        , Lazy<ILogger<BlogService>> logger, Lazy<IReactionService> reactionService, Lazy<IFileService> fileService, Lazy<IIdentityService> identityService)
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
                    t.Summary,
                    t.Body,
                    t.ImageId,
                    t.LikeCount,
                    t.DislikeCount,
                    CreationUser = t.CreationUser.FirstName + " " + t.CreationUser.LastName,
                    Tags = t.PostTags == null ? null : t.PostTags.Select(s => new TagDto
                    {
                        Icon = s.Tag.Icon,
                        Id = s.TagId,
                        Name = s.Tag.Name,
                        TagType = s.Tag.TagType,
                    }),
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
                    Title = post.Title,
                    Summary = post.Summary,
                    Body = post.Body,
                    ImageUri = fileService.Value.GetFileUri(post.ImageId!, ContainerType.Post).Data,
                    LikeCount = post.LikeCount,
                    DislikeCount = post.DislikeCount,
                    CreationUser = post.CreationUser,
                    Tags = post.Tags,
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

                    var permitted = await HasManagePermissionAsync(post.CreationUserId);
                    if (!permitted)
                    {
                        return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["InsufficientPrivileges"] },], };
                    }

                    var (imageId, errors) = await SaveImageAsync();
                    if (errors is not null)
                    {
                        return new(OperationResult.Failed)
                        {
                            Errors = errors,
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
                    var (imageId, errors) = await SaveImageAsync();
                    if (errors is not null)
                    {
                        return new(OperationResult.Failed)
                        {
                            Errors = errors,
                        };
                    }

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

                async Task<(string? ImageId, IEnumerable<Error>? Errors)> SaveImageAsync()
                {
                    if (requestDto.Image is null)
                    {
                        return (null, null);
                    }

                    using MemoryStream stream = new();
                    await requestDto.Image.CopyToAsync(stream);

                    var fileId = await fileService.Value.UploadFileAsync(new()
                    {
                        File = stream.ToArray(),
                        ContainerType = ContainerType.Post,
                        FileExtension = Path.GetExtension(requestDto.Image.FileName),
                    });

                    return fileId.OperationResult is OperationResult.Succeeded
                        ? ((string? ImageId, IEnumerable<Error>? Errors))(fileId.Data, null)
                        : new(null, fileId.Errors);
                }
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

                var result = await UpdatePostReactionsAsync(requestDto.PostId);

                return result;
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

                var result = await UpdatePostReactionsAsync(requestDto.PostId);

                return result;
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        public async Task<ResultData<bool>> RemovePostAsync([NotNull] ISpecification<Post> specification)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var postRepository = uow.GetRepository<Post>();
                var post = await postRepository.GetAsync(specification);
                if (post is null)
                {
                    return new(OperationResult.NotFound) { Errors = [new() { Message = Localizer.Value["PostNotFound"] },], };
                }

                var permitted = await HasManagePermissionAsync(post.CreationUserId);
                if (!permitted)
                {
                    return new(OperationResult.NotValid) { Errors = [new() { Message = Localizer.Value["InsufficientPrivileges"] },], };
                }

                //remove post
                postRepository.Remove(post);
                _ = await uow.SaveChangesAsync();

                //remove reactions
                var reactionSpecification = new IdentifierIdEqualsSpecification<Reaction>(post.Id)
                    .And(new CategoryTypeEqualsSpecification<Reaction>(CategoryType.Post));
                _ = reactionService.Value.RemoveReactionAsync(reactionSpecification);

                if (!string.IsNullOrEmpty(post.ImageId))
                {
                    //remove image
                    _ = await fileService.Value.RemoveFileAsync(new()
                    {
                        ContainerType = ContainerType.Post,
                        FileId = post.ImageId,
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

        #region Job

        public async Task<ResultData<bool>> UpdatePostReactionsAsync(long? postId = null)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var where = postId.HasValue ? $"WHERE p.Id={postId.Value}" : "";
                var query = $@"UPDATE p SET
                    LikeCount=(SELECT COUNT(1) FROM Reaction r WHERE r.CategoryType={CategoryType.Post.Value} AND r.IdentifierId=p.Id AND r.IsLike=1)
                    ,DislikeCount=(SELECT COUNT(1) FROM Reaction r WHERE r.CategoryType={CategoryType.Post.Value} AND r.IdentifierId=p.Id AND r.IsLike=0)
                FROM Posts p {where}";
                _ = await uow.ExecuteSqlCommandAsync(query);

                return new(OperationResult.Succeeded) { Data = true };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new() { Message = exc.Message, },] };
            }
        }

        #endregion

        private async Task<bool> HasManagePermissionAsync(int creationUserId)
        {
            var currentUserId = HttpContextAccessor.Value.HttpContext.UserId();
            if (creationUserId != currentUserId)
            {
                var hasAdminRole = await identityService.Value.UserIsInRoleAsync(currentUserId, nameof(Role.Admin));
                if (!hasAdminRole.Data)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
