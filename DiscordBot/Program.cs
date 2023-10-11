using Discord.Addons.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Services;

namespace DiscordBot
{
    class Program
    {

        static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })

                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })

                .ConfigureDiscordHost((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        AlwaysDownloadUsers = false,
                        MessageCacheSize = 200,
                    };

                    config.Token = context.Configuration["Token"];

                })

                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = LogSeverity.Debug;
                    config.DefaultRunMode = RunMode.Sync;
                })

                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<CommandHandler>();
                    services.AddHostedService<BotStatusService>();
                    services.AddSingleton(new DiscordSocketConfig
                    {
                        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                    });


                })


                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }

        

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        
    }

}