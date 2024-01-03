using Agent_interceptor.Services.Bot.Interfaces;

namespace Agent_interceptor.Middlewares
{
    public static class TelegramMiddleware
    {
        public static void UseTelegram<T>(this IApplicationBuilder applicationBuilder)
         where T : IStartBotTelegram
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<T>();
            service.InitialTelegram();
        }
    }
}
