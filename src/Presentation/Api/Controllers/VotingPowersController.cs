namespace GamaEdtech.Presentation.Api.Controllers
{
    using System.Diagnostics.CodeAnalysis;

    using Asp.Versioning;

    using GamaEdtech.Application.Interface;
    using GamaEdtech.Common.Core;
    using GamaEdtech.Common.Data;
    using GamaEdtech.Data.Dto.VotingPower;
    using GamaEdtech.Presentation.ViewModel.VotingPower;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class VotingPowersController(Lazy<ILogger<VotingPowersController>> logger, Lazy<IVotingPowerService> votingPowerService)
        : ApiControllerBase<VotingPowersController>(logger)
    {
        [HttpPost, Produces<ApiResponse<Void>>()]
        public async Task<IActionResult<Void>> CreateVotingPower([NotNull, FromBody] CreateVotingPowerRequestViewModel request)
        {
            try
            {
                //check signature
                var lst = request.Data.Select(t => new ManageVotingPowerRequestDto
                {
                    Amount = t.Amount.GetValueOrDefault(),
                    WalletAddress = t.WalletAddress,
                    TokenAccount = t.TokenAccount,
                    ProposalId = t.ProposalId,
                });
                var result = await votingPowerService.Value.BulkImportVotingPowersAsync(lst);

                return Ok<Void>(new(result.Errors)
                {
                    Data = new(),
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok<Void>(new() { Errors = [new() { Message = exc.Message }] });
            }
        }
    }
}
