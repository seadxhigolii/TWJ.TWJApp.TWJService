using TWJ.TWJApp.TWJService.Common.Helpers;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.MessageBroker.ChannelConfig.Client
{
    public class Client : IClient
    {
        private readonly IConfiguration _configuration;
        public Client(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public GrpcChannel CoreChannel => GrpcChannel.ForAddress(_configuration["Services:CoreUrl"], new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Create(new SslCredentials(), BuildCallCredentials())
        });

        private static CallCredentials BuildCallCredentials()
        {
            return CallCredentials.FromInterceptor((context, metadata) =>
            {
                var token = AuthHelper.Token;

                if (!string.IsNullOrEmpty(token))
                {
                    metadata.Add("Authorization", token);
                }

                return Task.CompletedTask;
            });
        }
    }
}
