namespace GamaEdtech.Backend.Common.Identity
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    using Microsoft.AspNetCore.Identity;

    [Injectable]
    public interface IApiDataProtectorTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser>
        where TUser : class
    {
    }
}
