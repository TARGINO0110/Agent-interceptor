using Agent_interceptor.Services.Bot.Interfaces;
using System.Reflection;
using System.Text;

namespace Agent_interceptor.Middlewares
{
    public class InterceptorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IInfoWebClientTelegram _infoWebClientTelegram;

        public InterceptorMiddleware(RequestDelegate next, IInfoWebClientTelegram infoWebClientTelegram)
        {
            _next = next;
            _infoWebClientTelegram = infoWebClientTelegram;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);

            await CollectWebClientInfo(httpContext);
        }

        private async Task CollectWebClientInfo(HttpContext httpRequest)
        {
            StringBuilder bodyBot = new();
            StringBuilder footerBot = new();
            bodyBot.AppendLine("##### Info Web Client #####\n");
            foreach (var item in httpRequest.Request.Headers.Where(item =>
            !string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value)))
            {
                bodyBot.AppendLine($"{item.Key}:{item.Value}\n");
            }
            footerBot.AppendLine("\n\n##### Connection request #####\n");
            foreach (PropertyInfo inf in httpRequest.Connection.GetType().GetProperties())
            {
                if (inf.GetValue(httpRequest.Connection) is not null)

                {
                    footerBot.AppendLine($"{inf.Name}:{inf.GetValue(httpRequest.Connection)}\n");
                }
            }
            string text = await Task.FromResult(string.Concat(bodyBot.ToString(), footerBot.ToString()));
            Console.WriteLine(text);
            await _infoWebClientTelegram.SendTelegram(text, "<ID_CHAT_GRUPO>");
        }
    }
}
