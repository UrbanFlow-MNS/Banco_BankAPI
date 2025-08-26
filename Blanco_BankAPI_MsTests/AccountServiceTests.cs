using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blanco_BankAPI.Service;
using Blanco_BankAPI.Database;
using Blanco_BankAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Blanco_BankAPI.Tests.Service
{
    [TestClass]
    public class AccountServiceTests
    {
        private BlancoDbContext _context;
        private AccountService _accountService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BlancoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new BlancoDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _accountService = new AccountService(_context);
        }

        [TestMethod]
        public void GetAccountAmountByUserId_ShouldReturnBalance_WhenAccountExists()
        {
            // Arrange
            var account = new Account { UserId = 1, Balance = 500, AccountNumber = "ACC123" };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // Act
            int balance = _accountService.GetAccountAmountByUserId(1);

            // Assert
            Assert.AreEqual(500, balance);
        }

        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void GetAccountAmountByUserId_ShouldThrow_WhenAccountNotFound()
        {
            // Act
            var balance = _accountService.GetAccountAmountByUserId(99);
        }

        [TestMethod]
        public async Task CreateAccountBalance_ShouldCreateNewAccount()
        {
            // Act
            var account = await _accountService.CreateAccountBalance(2, 1000, "ACC456");

            // Assert
            Assert.IsNotNull(account);
            Assert.AreEqual(2, account.UserId);
            Assert.AreEqual(1000, account.Balance);
            Assert.AreEqual("ACC456", account.AccountNumber);

            var savedAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == 2);
            Assert.IsNotNull(savedAccount);
            Assert.AreEqual(1000, savedAccount.Balance);
        }

        [TestMethod]
        public async Task CreateAccountBalance_ShouldPersistMultipleAccounts()
        {
            // Act
            await _accountService.CreateAccountBalance(3, 200, "ACC789");
            await _accountService.CreateAccountBalance(4, 400, "ACC999");

            // Assert
            Assert.AreEqual(2, await _context.Accounts.CountAsync());
            var acc3 = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == 3);
            var acc4 = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == 4);

            Assert.AreEqual(200, acc3.Balance);
            Assert.AreEqual(400, acc4.Balance);
        }

        [TestMethod]
        public async Task CreateAccountBalance_ShouldAllowZeroInitialAmount()
        {
            // Act
            var account = await _accountService.CreateAccountBalance(5, 0, "ACC000");

            // Assert
            Assert.IsNotNull(account);
            Assert.AreEqual(0, account.Balance);
        }

        [TestMethod]
        public async Task GetAccountAmountByUserId_ShouldReturnUpdatedBalance_AfterAccountChange()
        {
            // Arrange
            var account = new Account { UserId = 6, Balance = 50, AccountNumber = "ACC777" };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Mise à jour du solde
            account.Balance = 999;
            await _context.SaveChangesAsync();

            // Act
            int balance = _accountService.GetAccountAmountByUserId(6);

            // Assert
            Assert.AreEqual(999, balance);
        }
    }
}
