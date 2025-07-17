namespace GamaEdtech.Presentation.Api.Controllers
{
    using System;

    using Asp.Versioning;

    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class GeneralController(Lazy<ILogger<GeneralController>> logger)
        : ApiControllerBase<GeneralController>(logger)
    {
        [HttpGet("system-claims"), Produces(typeof(ApiResponse<IEnumerable<string>?>))]
        public IActionResult<IEnumerable<string>?> GetSystemClaims()
        {
            try
            {
                var lst = Common.Data.Enumeration.FlagsEnumerationExtensions.GetNames<Domain.Enumeration.SystemClaim>();
                return Ok<IEnumerable<string>?>(new()
                {
                    Data = lst,
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<IEnumerable<string>?>(new(new Error { Message = exc.Message }));
            }
        }
    }
}
