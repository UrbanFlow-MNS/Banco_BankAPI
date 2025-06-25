using Blanco_BankAPI.DTO;
using MassTransit;

namespace Blanco_BankAPI
{
    public interface IGetBalanceConsumer
    {
        Task Consume(ConsumeContext<WrappedMessage<UserBalanceDTO>> context);
    }
}