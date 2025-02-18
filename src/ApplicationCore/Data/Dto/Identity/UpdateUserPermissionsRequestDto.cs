namespace GamaEdtech.Data.Dto.Identity
{
    using GamaEdtech.Data.Enumeration;

    using System.Collections.Generic;

    public sealed class UpdateUserPermissionsRequestDto
    {
        public int UserId { get; set; }

        public IEnumerable<string>? Claims { get; set; }

        public Role? Roles { get; set; }
    }
}
