using Microsoft.AspNetCore.Mvc;
using MediatR;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Auth.Commands;

namespace BaitaHora.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICookieService _cookieService;

        public AuthController(ISender mediator, ICookieService cookieService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
        }

        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerWithCompanyCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return result.ToActionResult(this);
        }
    }
}