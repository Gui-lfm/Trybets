using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TryBets.Bets.Repository;
using TryBets.Bets.DTO;
using TryBets.Bets.Services;


namespace TryBets.Bets.Controllers;

[Route("[controller]")]
public class BetController : Controller
{
    private readonly IBetRepository _repository;
    private readonly IOddService _oddService;
    public BetController(IBetRepository repository, IOddService oddService)
    {
        _repository = repository;
        _oddService = oddService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public async Task<IActionResult> Post([FromBody] BetDTORequest request)
    {
        try
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var email = token?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized(new { message = "Valid user email is required" });
            }

            var response = _repository.Post(request, email);
            await _oddService.UpdateOdd(response.MatchId, response.TeamId, response.BetValue);
            
            return Created("", response);
        }
        catch (Exception e)
        {

            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("{BetId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public IActionResult Get([FromRoute] int BetId)
    {
        try
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var email = token?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized(new { message = "Valid user email is required" });
            }

            var response = _repository.Get(BetId, email);
            return Ok(response);
        }
        catch (Exception e)
        {

            return BadRequest(new { message = e.Message });
        }
    }
}