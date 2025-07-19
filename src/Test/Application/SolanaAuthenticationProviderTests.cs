// Add this to your .Net Test Project
// Project: GamaEdtech.Test/Application/SolanaAuthenticationProviderTests.cs

namespace GamaEdtech.Test.Application
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using GamaEdtech.Data.Dto.Identity;
    using GamaEdtech.Domain.Entity.Identity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Infrastructure.Provider.Authentication;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    using Moq;

    using Solnet.Wallet;

    using Xunit;

    using static GamaEdtech.Common.Core.Constants;

    public class SolanaAuthenticationProviderTests
    {
        private readonly Mock<ILogger<SolanaAuthenticationProvider>> loggerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly SolanaAuthenticationProvider provider;

        public SolanaAuthenticationProviderTests()
        {
            // Mock UserManager and its dependencies
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                Enumerable.Empty<IUserValidator<ApplicationUser>>(),
                Enumerable.Empty<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            // Mock SignInManager
            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());

            // Mock Logger
            loggerMock = new Mock<ILogger<SolanaAuthenticationProvider>>();

            // Instantiate the provider with mocked dependencies
            var lazyLogger = new Lazy<ILogger<SolanaAuthenticationProvider>>(() => loggerMock.Object);
            var lazySignInManager = new Lazy<SignInManager<ApplicationUser>>(() => signInManagerMock.Object);
            provider = new SolanaAuthenticationProvider(lazyLogger, lazySignInManager);
        }

        /// <summary>
        /// Test case for a complete, valid authentication flow.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsyncWithValidSignatureAndChallengeShouldSucceed()
        {
            // Arrange
            // 1. Generate a new temporary Solana account for this test.
            var account = new Account();
            var walletAddress = account.PublicKey.ToString();

            // 2. Simulate the client requesting a challenge from the server.
            var challenge = SolanaAuthenticationProvider.GenerateChallenge(walletAddress);

            // 3. Simulate the client signing the message with their private key.
            var messageBytes = Encoding.UTF8.GetBytes(challenge.Message);
            var signatureBytes = account.Sign(messageBytes);
            var signatureBase64 = Convert.ToBase64String(signatureBytes);

            // 4. Prepare the DTO for the authentication request.
            var solanaAuthDto = new SolanaAuthenticationDto
            {
                WalletAddress = walletAddress,
                Signature = signatureBase64,
                Message = challenge.Message,
                Nonce = challenge.Nonce
            };

            var requestDto = new AuthenticationRequestDto
            {
                Username = System.Text.Json.JsonSerializer.Serialize(solanaAuthDto),
                AuthenticationProvider = AuthenticationProvider.Solana,
            };

            // 5. Mock the user creation process for a new user.
            _ = userManagerMock.Setup(um => um.FindByNameAsync(walletAddress)).ReturnsAsync(default(ApplicationUser));
            _ = userManagerMock.Setup(um => um.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == walletAddress)))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await provider.AuthenticateAsync(requestDto);

            // Assert
            Assert.Equal(OperationResult.Succeeded, result.OperationResult);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.User);
            // Assuming the DTO property is UserName, matching IdentityUser.
            Assert.Equal(walletAddress, result.Data.User.UserName);
            // **FIXED HERE**: Removed the underscore from userManagerMock
            userManagerMock.Verify(um => um.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == walletAddress)), Times.Once);
        }

        /// <summary>
        /// Tests that authentication fails if the signature is invalid.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsyncWithInvalidSignatureShouldFail()
        {
            // Arrange
            var account = new Account();
            var walletAddress = account.PublicKey.ToString();
            var challenge = SolanaAuthenticationProvider.GenerateChallenge(walletAddress);

            // Generate an invalid signature using a secure random generator.
            var invalidSignature = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(invalidSignature);
            }

            var solanaAuthDto = new SolanaAuthenticationDto
            {
                WalletAddress = walletAddress,
                Signature = Convert.ToBase64String(invalidSignature),
                Message = challenge.Message,
                Nonce = challenge.Nonce
            };

            var requestDto = new AuthenticationRequestDto
            {
                Username = System.Text.Json.JsonSerializer.Serialize(solanaAuthDto),
                AuthenticationProvider = AuthenticationProvider.Solana
            };

            // Act
            var result = await provider.AuthenticateAsync(requestDto);

            // Assert
            Assert.Equal(OperationResult.NotValid, result.OperationResult);
            Assert.NotNull(result.Errors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Invalid signature", result.Errors.First().Message);
        }

        /// <summary>
        /// Tests that authentication fails if the nonce is mismatched.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsyncWithMismatchedNonceShouldFail()
        {
            // Arrange
            var account = new Account();
            var walletAddress = account.PublicKey.ToString();
            var challenge = SolanaAuthenticationProvider.GenerateChallenge(walletAddress);

            var messageBytes = Encoding.UTF8.GetBytes(challenge.Message);
            var signature = Convert.ToBase64String(account.Sign(messageBytes));

            var solanaAuthDto = new SolanaAuthenticationDto
            {
                WalletAddress = walletAddress,
                Signature = signature,
                Message = challenge.Message,
                Nonce = "this-is-a-wrong-nonce" // Invalid nonce
            };

            var requestDto = new AuthenticationRequestDto
            {
                Username = System.Text.Json.JsonSerializer.Serialize(solanaAuthDto),
                AuthenticationProvider = AuthenticationProvider.Solana
            };

            // Act
            var result = await provider.AuthenticateAsync(requestDto);

            // Assert
            Assert.Equal(OperationResult.NotValid, result.OperationResult);
            Assert.NotNull(result.Errors);
            Assert.Equal("Invalid nonce", result.Errors.First().Message);
        }

        /// <summary>
        /// Tests that authentication fails if no challenge exists for the wallet.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsyncWhenChallengeNotFoundShouldFail()
        {
            // Arrange
            var walletAddress = "4pUQS4kCLg3V2a1ZAd3a1W2P6t1L9y4J2s3a4K5b6c7d"; // A random address
            var solanaAuthDto = new SolanaAuthenticationDto
            {
                WalletAddress = walletAddress,
                Signature = "dummySignature",
                Message = "dummyMessage",
                Nonce = "dummyNonce"
            };
            var requestDto = new AuthenticationRequestDto
            {
                Username = System.Text.Json.JsonSerializer.Serialize(solanaAuthDto),
                AuthenticationProvider = AuthenticationProvider.Solana
            };

            // Act
            var result = await provider.AuthenticateAsync(requestDto);

            // Assert
            Assert.Equal(OperationResult.NotValid, result.OperationResult);
            Assert.NotNull(result.Errors);
            Assert.Equal("Challenge expired or not found", result.Errors.First().Message);
        }

        /// <summary>
        /// Tests that authentication fails if the signed message is different from the challenge message.
        /// </summary>
        [Fact]
        public async Task AuthenticateAsyncWithMismatchedMessageShouldFail()
        {
            // Arrange
            var account = new Account();
            var walletAddress = account.PublicKey.ToString();
            var challenge = SolanaAuthenticationProvider.GenerateChallenge(walletAddress);
            var signedMessage = "This is not the message from the challenge.";
            var signature = Convert.ToBase64String(account.Sign(Encoding.UTF8.GetBytes(signedMessage)));

            var solanaAuthDto = new SolanaAuthenticationDto
            {
                WalletAddress = walletAddress,
                Signature = signature,
                Message = signedMessage, // Send the wrong message to the server
                Nonce = challenge.Nonce
            };

            var requestDto = new AuthenticationRequestDto
            {
                Username = System.Text.Json.JsonSerializer.Serialize(solanaAuthDto),
                AuthenticationProvider = AuthenticationProvider.Solana
            };

            // Act
            var result = await provider.AuthenticateAsync(requestDto);

            // Assert
            Assert.Equal(OperationResult.NotValid, result.OperationResult);
            Assert.NotNull(result.Errors);
            Assert.Equal("Invalid message", result.Errors.First().Message);
        }
    }
}
