namespace Blanco_BankAPI.Service
{
    public interface IAccountService
    {
        decimal GetAccountAmountByUserId(int userId);
    }
}