using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        var user = _context.Users.Where(user => user.Email == email).FirstOrDefault() ?? throw new Exception("User not founded");

        var match = _context.Matches.Where(match => match.MatchId == betRequest.MatchId).FirstOrDefault() ?? throw new Exception("Match not founded");

        var team = _context.Teams.Where(team => team.TeamId == betRequest.TeamId).FirstOrDefault() ?? throw new Exception("Team not founded");

        if (match.MatchFinished)
        {
            throw new Exception("Match finished");
        }

        if (match.MatchTeamAId != betRequest.TeamId && match.MatchTeamBId != betRequest.TeamId)
        {
            throw new Exception("Team is not in this match");
        }

        var newBet = new Bet
        {
            UserId = user.UserId,
            MatchId = match.MatchId,
            TeamId = team.TeamId,
            BetValue = betRequest.BetValue
        };

        _context.Bets.Add(newBet);
        _context.SaveChanges();

        Bet createdBet = _context.Bets
        .Include(b => b.Team)
        .Include(b => b.Match)
        .Where(b => b.BetId == newBet.BetId).FirstOrDefault()!;

        if (match.MatchTeamAId == betRequest.TeamId)
        {
            match.MatchTeamAValue += betRequest.BetValue;
        }
        else
        {
            match.MatchTeamBValue += betRequest.BetValue;
        }

        _context.Matches.Update(match);
        _context.SaveChanges();

        BetDTOResponse betResponse = new()
        {
            BetId = createdBet.BetId,
            MatchId = createdBet.MatchId,
            TeamId = createdBet.TeamId,
            BetValue = createdBet.BetValue,
            MatchDate = createdBet.Match!.MatchDate,
            TeamName = createdBet.Team!.TeamName,
            Email = createdBet.User!.Email
        };

        return betResponse;
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        var user = _context.Users.Where(user => user.Email == email).FirstOrDefault() ?? throw new Exception("User not founded");

        var bet = _context.Bets
        .Include(bet => bet.Team)
        .Include(bet => bet.Match)
        .Where(bet => bet.BetId == BetId).FirstOrDefault() ?? throw new Exception("Bet not founded");

        if (bet.User!.Email != email)
        {
            throw new Exception("Bet view not allowed");
        }

        BetDTOResponse response = new()
        {
            BetId = bet.BetId,
            MatchId = bet.MatchId,
            TeamId = bet.TeamId,
            BetValue = bet.BetValue,
            MatchDate = bet.Match!.MatchDate,
            TeamName = bet.Team!.TeamName,
            Email = bet.User!.Email
        };

        return response;
    }
}