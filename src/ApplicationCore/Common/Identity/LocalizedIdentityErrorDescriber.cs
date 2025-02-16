namespace GamaEdtech.Backend.Common.Identity
{
    using GamaEdtech.Backend.Common.Resources;

    using Microsoft.AspNetCore.Identity;

    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string? email) => new()
        {
            Code = nameof(DuplicateEmail),
            Description = string.Format(GlobalResource.IdentityError_DuplicateEmail, email),
        };

        public override IdentityError DuplicateUserName(string? userName) => new()
        {
            Code = nameof(DuplicateUserName),
            Description = string.Format(GlobalResource.IdentityError_DuplicateUserName, userName),
        };

        public override IdentityError InvalidEmail(string? email) => new()
        {
            Code = nameof(InvalidEmail),
            Description = string.Format(GlobalResource.IdentityError_InvalidEmail, email),
        };

        public override IdentityError DuplicateRoleName(string? role) => new()
        {
            Code = nameof(DuplicateRoleName),
            Description = string.Format(GlobalResource.IdentityError_DuplicateRoleName, role),
        };

        public override IdentityError InvalidRoleName(string? role) => new()
        {
            Code = nameof(InvalidRoleName),
            Description = string.Format(GlobalResource.IdentityError_InvalidRoleName, role),
        };

        public override IdentityError InvalidToken() => new()
        {
            Code = nameof(InvalidToken),
            Description = GlobalResource.IdentityError_InvalidToken,
        };

        public override IdentityError InvalidUserName(string? userName) => new()
        {
            Code = nameof(InvalidUserName),
            Description = string.Format(GlobalResource.IdentityError_InvalidUserName, userName),
        };

        public override IdentityError LoginAlreadyAssociated() => new()
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = GlobalResource.IdentityError_LoginAlreadyAssociated,
        };

        public override IdentityError PasswordMismatch() => new()
        {
            Code = nameof(PasswordMismatch),
            Description = GlobalResource.IdentityError_PasswordMismatch,
        };

        public override IdentityError PasswordRequiresDigit() => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = GlobalResource.IdentityError_PasswordRequiresDigit,
        };

        public override IdentityError PasswordRequiresLower() => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = GlobalResource.IdentityError_PasswordRequiresLower,
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = GlobalResource.IdentityError_PasswordRequiresNonAlphanumeric,
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = string.Format(GlobalResource.IdentityError_PasswordRequiresUniqueChars, uniqueChars),
        };

        public override IdentityError PasswordRequiresUpper() => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = GlobalResource.IdentityError_PasswordRequiresUpper,
        };

        public override IdentityError PasswordTooShort(int length) => new()
        {
            Code = nameof(PasswordTooShort),
            Description = string.Format(GlobalResource.IdentityError_PasswordTooShort, length),
        };

        public override IdentityError UserAlreadyHasPassword() => new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = GlobalResource.IdentityError_UserAlreadyHasPassword,
        };

        public override IdentityError UserAlreadyInRole(string? role) => new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = string.Format(GlobalResource.IdentityError_UserAlreadyInRole, role),
        };

        public override IdentityError UserNotInRole(string? role) => new()
        {
            Code = nameof(UserNotInRole),
            Description = string.Format(GlobalResource.IdentityError_UserNotInRole, role),
        };

        public override IdentityError UserLockoutNotEnabled() => new()
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = GlobalResource.IdentityError_UserLockoutNotEnabled,
        };

        public override IdentityError RecoveryCodeRedemptionFailed() => new()
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = GlobalResource.IdentityError_RecoveryCodeRedemptionFailed,
        };

        public override IdentityError ConcurrencyFailure() => new()
        {
            Code = nameof(ConcurrencyFailure),
            Description = GlobalResource.IdentityError_ConcurrencyFailure,
        };

        public override IdentityError DefaultError() => new()
        {
            Code = nameof(DefaultError),
            Description = GlobalResource.IdentityError_DefaultIdentityError,
        };
    }
}
