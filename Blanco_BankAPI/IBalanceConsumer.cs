using Blanco_BankAPI.DTO;
using MassTransit;

namespace Blanco_BankAPI
{
    public interface IBalanceConsumer
    {
        Task Consume(ConsumeContext<WrappedMessage<UserBalanceDTO>> context);
    }
}