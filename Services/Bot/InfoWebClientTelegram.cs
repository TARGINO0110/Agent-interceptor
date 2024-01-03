using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Agent_interceptor.Services.Bot.Interfaces;

namespace Agent_interceptor.Services.Bot
{
    public class InfoWebClientTelegram : IInfoWebClientTelegram
    {
        private readonly ILogger _logger;
        private readonly IAuthBotTelegram _authBot;

        public InfoWebClientTelegram(ILogger<InfoWebClientTelegram> logger, IAuthBotTelegram authBot)
        {
            _logger = logger;
            _authBot = authBot;
        }

        public async Task SendTelegram(string message, string chatId)
        {
            using CancellationTokenSource cts = new();

            var clientBot = _authBot.TokenBot();
            var bot = await clientBot.GetMeAsync();
            try
            {
                await Send(message, clientBot, cts.Token);

                async Task Send(string message, ITelegramBotClient botClient, CancellationToken cancellationToken)
                {

                    _ = await botClient.SendTextMessageAsync(
                     chatId: chatId,
                     text: message,
                     parseMode: ParseMode.Html,
                     cancellationToken: cancellationToken);
                }

                cts.Cancel();
            }
            catch (Exception exception)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"📛📛📛 Error [SEND MESSAGE BOT] 📛📛📛:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _logger.LogWarning(500, $"[{DateTime.Now}] - Error:\n[{ErrorMessage}");
                Console.WriteLine(ErrorMessage);
            }
        }
    }
}
