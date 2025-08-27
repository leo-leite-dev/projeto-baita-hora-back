namespace BaitaHora.Application.Abstractions.Integrations
{
    public interface IInstagramApi
    {
        Task SendTextAsync(string igBusinessId, string recipientPsid, string text, CancellationToken ct = default);
   
           Task DiagnoseAsync(
            string pageAccessToken,
            string appId,
            string appSecret,
            string igUserId,
            CancellationToken ct = default);
    }
}