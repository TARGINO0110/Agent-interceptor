using Agent_interceptor.Services.Bot.Interfaces;
using Telegram.Bot;

namespace Agent_interceptor.Services.Bot
{
    public class AuthBotTelegram : IAuthBotTelegram
    {
        public TelegramBotClient TokenBot() => new("<TOKEN_BOT>");
    }
}
