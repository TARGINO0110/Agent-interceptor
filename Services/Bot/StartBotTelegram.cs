using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Agent_interceptor.Services.Bot.Interfaces;

namespace Agent_interceptor.Services.Bot
{
    public class StartBotTelegram : IStartBotTelegram
    {
        private readonly ILogger _logger;
        private readonly IAuthBotTelegram _authBot;

        public StartBotTelegram(ILogger<StartBotTelegram> logger, IAuthBotTelegram auth)
        {
            _logger = logger;
            _authBot = auth;
        }

        public async Task InitialTelegram()
        {
            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            var botClient = _authBot.TokenBot();

            botClient.StartReceiving(
               updateHandler: HandleUpdateAsync,
               pollingErrorHandler: HandlePollingErrorAsync,
               receiverOptions: receiverOptions,
               cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            _logger.LogInformation(200, $"[{DateTime.Now}] ****** Start BOT ({Guid.NewGuid()}) ******");

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                string textBody = $"User: {message.From.Username}\nMensagem: {messageText}";

                // Echo received message text
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: textBody,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"📛📛📛 Error [START BOT] 📛📛📛:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _logger.LogWarning(500, $"[{DateTime.Now}] - Error:\n[{ErrorMessage}");
                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }
    }
}
