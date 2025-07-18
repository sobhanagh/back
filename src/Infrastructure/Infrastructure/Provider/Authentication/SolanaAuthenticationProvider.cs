namespace GamaEdtech.Infrastructure.Provider.Authentication
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography;
    using System.Text;
    using Solnet.Wallet;

    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.Mapping;
    using GamaEdtech.Data.Dto.Identity;
    using GamaEdtech.Domain.Entity.Identity;
    using GamaEdtech.Domain.Enumeration;
    using GamaEdtech.Infrastructure.Interface;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;


    using static GamaEdtech.Common.Core.Constants;

    public sealed class SolanaAuthenticationProvider(
        Lazy<ILogger<SolanaAuthenticationProvider>> logger,
        Lazy<SignInManager<ApplicationUser>> signInManager) : IAuthenticationProvider
    {
        private static readonly TimeSpan ChallengeExpiration = TimeSpan.FromMinutes(5);
        private static readonly Dictionary<string, (string Message, string Nonce, DateTime Timestamp)> Challenges = [];

        public AuthenticationProvider ProviderType => AuthenticationProvider.Solana;

        public Task<ResultData<AuthenticationResponseDto>> AuthenticateAsync([NotNull] AuthenticationRequestDto requestDto)
        {
            try
            {
                var solanaAuth = System.Text.Json.JsonSerializer.Deserialize<SolanaAuthenticationDto>(requestDto.Username);
                if (solanaAuth is null)
                {
                    return Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.NotValid)
                    {
                        Errors = [new Error { Message = "Invalid authentication data" }]
                    });
                }

                // Verify the challenge exists and hasn't expired
                if (!Challenges.TryGetValue(solanaAuth.WalletAddress, out var challenge) ||
                    DateTime.UtcNow - challenge.Timestamp > ChallengeExpiration)
                {
                    return Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.NotValid)
                    {
                        Errors = [new Error { Message = "Challenge expired or not found" }]
                    });
                }

                // Verify nonce matches
                if (challenge.Nonce != solanaAuth.Nonce)
                {
                    return Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.NotValid)
                    {
                        Errors = [new Error { Message = "Invalid nonce" }]
                    });
                }

                // Verify message matches
                if (challenge.Message != solanaAuth.Message)
                {
                    return Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.NotValid)
                    {
                        Errors = [new Error { Message = "Invalid message" }]
                    });
                }

                // Signature Verification
                var messageBytes = Encoding.UTF8.GetBytes(solanaAuth.Message);
                var signatureBytes = Convert.FromBase64String(solanaAuth.Signature);
                var pubKey = new PublicKey(solanaAuth.WalletAddress);

                return !pubKey.Verify(messageBytes, signatureBytes)
                    ? Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.NotValid)
                    {
                        Errors = [new Error { Message = "Invalid signature" }]
                    })
                    : CreateOrUpdateUserAsync(solanaAuth.WalletAddress);
            }
            catch (Exception exc)
            {
                logger.Value.LogException(exc);
                return Task.FromResult(new ResultData<AuthenticationResponseDto>(OperationResult.Failed)
                {
                    Errors = [new Error { Message = exc.Message }]
                });
            }
        }
        public static SolanaChallengeResponseDto GenerateChallenge(string walletAddress)
        {
            var nonce = GenerateNonce();
            var message = $"Sign this message to authenticate with GamaEdtech: {nonce}";

            Challenges[walletAddress] = (message, nonce, DateTime.UtcNow);

            return new SolanaChallengeResponseDto
            {
                Message = message,
                Nonce = nonce
            };
        }

        private async Task<ResultData<AuthenticationResponseDto>> CreateOrUpdateUserAsync(string walletAddress)
        {
            try
            {
                var user = await signInManager.Value.UserManager.FindByNameAsync(walletAddress);
                if (user is null)
                {
                    user = new ApplicationUser(walletAddress)
                    {
                        Email = $"{walletAddress}@solana.wallet", // Using a placeholder email
                        RegistrationDate = DateTime.UtcNow,
                        Enabled = true,
                        EmailConfirmed = true
                    };

                    var result = await signInManager.Value.UserManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new(OperationResult.NotValid)
                        {
                            Errors = result.Errors.Select(e => new Error { Message = e.Description })
                        };
                    }
                }

                var dto = user.AdaptData<ApplicationUser, ApplicationUserDto>();
                return new(OperationResult.Succeeded)
                {
                    Data = new() { User = dto }
                };
            }
            catch (Exception exc)
            {
                logger.Value.LogException(exc);
                return new(OperationResult.Failed) { Errors = [new Error { Message = exc.Message }] };
            }
        }

        private static string GenerateNonce()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }

    public class SolanaAuthenticationOptions
    {
        public string Network { get; set; } = "mainnet-beta";
    }
}
