namespace GamaEdtech.Test.Presentation.Api
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Domain.Entity.Identity;
    using GamaEdtech.Presentation.Api.Controllers;
    using GamaEdtech.Presentation.ViewModel.Referral;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Xunit;

    public class ReferralControllerUnitTest : TestBase
    {
        private readonly Lazy<IIdentityService> lazyIdentityService;
        private readonly Lazy<ILogger<ReferralController>> logger;

        public ReferralControllerUnitTest()
        {
            logger = Services.Value!.GetRequiredService<Lazy<ILogger<ReferralController>>>();
            lazyIdentityService = Services.Value!.GetRequiredService<Lazy<IIdentityService>>();
        }

        private ReferralController GetController() => new(logger, lazyIdentityService);


        [Fact]
        public async Task GenerateReferralUserShouldSucceedWhenUserHasNoReferralId()
        {
            // Arrange
            var controller = GetController();

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                          new[] { new Claim("sub", "2") }, "mock"))
            };

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await controller.GenerateRefferalId();

            // Assert
            var okResult = Assert.IsType<OkObjectResult<ReferralReponseViewModel>>(result);
            var apiResponse = Assert.IsType<ApiResponse<ReferralReponseViewModel>>(okResult.Value);
            Assert.True(apiResponse.Succeeded);

            var referralId = apiResponse.Data?.ReferralId;
            Assert.False(string.IsNullOrWhiteSpace(referralId));
            Assert.Equal(10, referralId!.Length);
            Assert.Contains(referralId, char.IsUpper);
            Assert.Contains(referralId, char.IsLower);
            Assert.Contains(referralId, char.IsDigit);
        }

        [Fact]
        public async Task GenerateReferralUserShouldFailWhenUserAlreadyHasReferralId()
        {
            // Arrange
            var controller = GetController();
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim("sub", "2") }, "mock"))
            };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var identityService = lazyIdentityService.Value;

            // Simulate existing user with referral ID
            var existingUser = new ApplicationUser
            {
                Id = 2,
                Email = "alreadyref@example.com",
                ReferralId = "ABC123xyz9"
            };
            _ = await identityService.CreateUserAsync(new()
            {
                Username = existingUser.Email,
                Email = existingUser.Email,
                Password = "TestPassword123",
                PhoneNumber = "1234567890"
            });

            // Act
            var result = await controller.GenerateRefferalId();

            // Assert
            var okResult = Assert.IsType<OkObjectResult<ReferralReponseViewModel>>(result);
            var apiResponse = Assert.IsType<ApiResponse<ReferralReponseViewModel>>(okResult.Value);
            Assert.False(apiResponse.Succeeded);
            Assert.NotNull(apiResponse.Errors);
            Assert.Contains(apiResponse.Errors,
                e => e.Message != null &&
                     e.Message.Contains("AlreadyHaveReferralId", StringComparison.OrdinalIgnoreCase));
        }

    }
}
