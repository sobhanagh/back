namespace GamaEdtech.Backend.Common.Mvc.Routing
{
    using System.Collections.Generic;

    using GamaEdtech.Backend.Common.DataAnnotation;

    [Injectable]
    public interface IEndpointDataSource
    {
        IEnumerable<Endpoint>? GetEndpoints(IEnumerable<string?>? claims = null, IEnumerable<string?>? roles = null);
    }
}
