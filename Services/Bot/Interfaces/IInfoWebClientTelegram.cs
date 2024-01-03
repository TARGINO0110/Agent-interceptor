namespace Agent_interceptor.Services.Bot.Interfaces
{
    public interface IInfoWebClientTelegram
    {
        Task SendTelegram(string message, string chatId);
    }
}
