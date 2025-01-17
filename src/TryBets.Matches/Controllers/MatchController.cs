using System;
using Microsoft.AspNetCore.Mvc;
using TryBets.Matches.Repository;

namespace TryBets.Matches.Controllers;

[Route("[controller]")]
public class MatchController : Controller
{
    private readonly IMatchRepository _repository;
    public MatchController(IMatchRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{MatchFinished}")]
    public IActionResult Get([FromRoute] bool MatchFinished)
    {
        var response = _repository.Get(MatchFinished);

        return Ok(response);
    }
}