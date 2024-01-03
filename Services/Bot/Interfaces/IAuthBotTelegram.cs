using Telegram.Bot;

namespace Agent_interceptor.Services.Bot.Interfaces
{
    public interface IAuthBotTelegram
    {
        TelegramBotClient TokenBot();
    }
}
