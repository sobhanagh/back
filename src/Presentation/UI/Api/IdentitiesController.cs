namespace GamaEdtech.Backend.UI.Web.Api
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.Data.Enumeration;
    using Farsica.Framework.Identity;

    using GamaEdtech.Backend.Data.Dto.Identity;
    using GamaEdtech.Backend.Data.Enumeration;
    using GamaEdtech.Backend.Data.ViewModel.Identity;
    using GamaEdtech.Backend.Shared.Service;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using static Farsica.Framework.Core.Constants;

    using Void = Farsica.Framework.Data.Void;

    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class IdentitiesController(Lazy<ILogger<IdentitiesController>> logger, Lazy<IIdentityService> identityService) : ApiControllerBase<IdentitiesController>(logger)
    {
        [HttpPost("login"), Produces(typeof(ApiResponse<AuthenticationResponseViewModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([NotNull] AuthenticationRequestViewModel request)
        {
            try
            {
                var authenticateResult = await identityService.Value.AuthenticateAsync(new AuthenticationRequestDto { Domain = request.Domain, Password = request.Password!, Username = request.Username! });
                if (authenticateResult.Data?.User is null)
                {
                    return Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Errors = authenticateResult.Errors,
                    });
                }

                var signInResult = await identityService.Value.SignInAsync(new SignInRequestDto { RememberMe = request.RememberMe, User = authenticateResult.Data.User });
                return signInResult.OperationResult is not OperationResult.Succeeded
                    ? Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Errors = signInResult.Errors,
                    })
                    : Ok(new ApiResponse<AuthenticationResponseViewModel>
                    {
                        Data = new AuthenticationResponseViewModel
                        {
                            Roles = signInResult.Data?.Roles?.ListToFlagsEnum<Role>(),
                        },
                    });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<AuthenticationResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("logout"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await identityService.Value.SignOutAsync();

                return Ok(new ApiResponse<Void>
                {
                    Data = result.Data,
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("password"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        public async Task<IActionResult> ChangePassword([NotNull] ChangePasswordRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.ChangePasswordAsync(new ChangePasswordRequestDto
                {
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword,
                });
                return Ok(new ApiResponse<Void>
                {
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost("tokens"), Produces(typeof(ApiResponse<GenerateTokenResponseViewModel>))]
        public async Task<IActionResult> GenerateToken([NotNull] GenerateTokenRequestViewModel request)
        {
            try
            {
                var authenticateResult = await identityService.Value.AuthenticateAsync(new AuthenticationRequestDto { Password = request.Password!, Username = request.Username! });
                if (authenticateResult.Data?.User is null)
                {
                    return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                    {
                        Errors = authenticateResult.Errors,
                    });
                }
                var signInResult = await identityService.Value.SignInAsync(new SignInRequestDto { RememberMe = false, User = authenticateResult.Data.User });
                if (signInResult.OperationResult is not OperationResult.Succeeded)
                {
                    return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                    {
                        Errors = signInResult.Errors,
                    });
                }
                var result = await identityService.Value.GenerateUserTokenAsync(new GenerateUserTokenRequestDto
                {
                    UserId = authenticateResult.Data.User.Id,
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<GenerateTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new GenerateTokenResponseViewModel
                    {
                        Token = result.Data?.Token,
                        ExpirationTime = result.Data?.ExpirationTime,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<GenerateTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPost("tokens/revoke"), Produces(typeof(ApiResponse<RevokeTokenResponseViewModel>))]
        [Permission(policy: null)]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var result = await identityService.Value.RemoveUserTokenAsync(new RemoveUserTokenRequestDto
                {
                    UserId = User.UserId<int>(),
                    TokenProvider = PermissionConstants.ApiDataProtectorTokenProvider,
                    Purpose = PermissionConstants.ApiDataProtectorTokenProviderAccessToken,
                });
                return Ok(new ApiResponse<RevokeTokenResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = new RevokeTokenResponseViewModel()
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<RevokeTokenResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("authenticated"), Produces(typeof(ApiResponse<bool>))]
        public IActionResult Authenticated()
        {
            try
            {
                return Ok(new ApiResponse<bool>
                {
                    Data = User.Identity?.IsAuthenticated is true,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<bool> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("profiles"), Produces(typeof(ApiResponse<ProfileSettingsResponseViewModel>))]
        [Permission(policy: null)]
        public async Task<IActionResult> GetProfileSettings()
        {
            try
            {
                var result = await identityService.Value.GetProfileSettingsAsync();

                return Ok(new ApiResponse<ProfileSettingsResponseViewModel>
                {
                    Data = new ProfileSettingsResponseViewModel
                    {
                        TimeZoneId = result.Data?.TimeZoneId,
                    },
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ProfileSettingsResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpPut("profiles"), Produces(typeof(ApiResponse<Void>))]
        [Permission(policy: null)]
        public async Task<IActionResult> UpdateProfileSettings([NotNull] ProfileSettingsRequestViewModel request)
        {
            try
            {
                var result = await identityService.Value.UpdateProfileSettingsAsync(new ProfileSettingsDto
                {
                    TimeZoneId = request.TimeZoneId,
                });

                return Ok(new ApiResponse<Void>
                {
                    Errors = result.Errors,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<Void> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }
    }
}
