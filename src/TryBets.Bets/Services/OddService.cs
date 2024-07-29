using System.Net.Http;
using System.Text.Json;

namespace TryBets.Bets.Services;

public class OddService : IOddService
{
    private readonly HttpClient _client;
    private const string _baseUrl = "http://trybets.odds:5504";
    public OddService(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<object> UpdateOdd(int MatchId, int TeamId, decimal BetValue)
    {

        var req = new HttpRequestMessage(HttpMethod.Patch, $"/odd/{MatchId}/{TeamId}/{BetValue}");

        req.Headers.Add("Accept", "application/json");
        req.Headers.Add("User-Agent", "aspnet-user-agent");

        var res = await _client.SendAsync(req);

        if (res.IsSuccessStatusCode)
        {
            var patched = await res.Content.ReadFromJsonAsync<object>();
            return patched!;
        }
        else
        {
            return default!;
        }
    }
}