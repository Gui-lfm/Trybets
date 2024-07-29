using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        string BetValueConverted = BetValue.Replace(',', '.');
        decimal BetValueDecimal = Decimal.Parse(BetValueConverted, CultureInfo.InvariantCulture);

        var match = _context.Matches.Where(match => match.MatchId == MatchId).FirstOrDefault() ?? throw new Exception("Match not founded");

        if (match.MatchTeamAId != TeamId && match.MatchTeamBId != TeamId)
        {
            throw new Exception("Team is not in this match");
        }

        if (match.MatchTeamAId == TeamId)
        {
            match.MatchTeamAValue += BetValueDecimal;
        }
        else
        {
            match.MatchTeamBValue += BetValueDecimal;
        }

        _context.Matches.Update(match);
        _context.SaveChanges();

        return match;
    }
}