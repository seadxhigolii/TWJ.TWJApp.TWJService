using Grpc.Net.Client;

namespace TWJ.TWJApp.TWJService.MessageBroker.ChannelConfig.Client
{
    public interface IClient
    {
        GrpcChannel CoreChannel { get; }
    }
}
