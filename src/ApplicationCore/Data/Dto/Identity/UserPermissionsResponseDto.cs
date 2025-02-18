namespace GamaEdtech.Data.Dto.Identity
{
    using GamaEdtech.Data.Enumeration;

    public sealed class UserPermissionsResponseDto
    {
        public IEnumerable<string?>? Claims { get; set; }

        public Role? Roles { get; set; }
    }
}
