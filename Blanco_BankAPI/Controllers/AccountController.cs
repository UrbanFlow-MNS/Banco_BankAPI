using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blanco_BankAPI.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blanco_BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("balance/{userId}")]
        public ActionResult<int> GetAccountAmount(int userId)
        {
            int amount = _accountService.GetAccountAmountByUserId(userId);

            return Ok(new
            {
                balance = amount
            });
        }

        [HttpPost("balance")]
        public ActionResult PostAccountAmount(int userId, int amount, string accNumber)
        {
            _accountService.CreateAccountBalance(userId, amount, accNumber);
            return Ok();
        }
    }
}

