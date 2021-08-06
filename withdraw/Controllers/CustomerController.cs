using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using withdraw.Models;
using withdraw.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace withdraw.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        private DBConnection dbConnection = new DBConnection();

        [AcceptVerbs("GET")]
        [Route("all")]
        public List<Customer> getAllCustomers()
        {
            return dbConnection.getAllCustomers();
        }

        [AcceptVerbs("GET")]
        [Route("withdraw/{id}/{value}")]
        public Withdraw setCustomerBankBalance(string id, int value)
        {
            int bankBalance = dbConnection.getCustomerBankBalance(id).First<int>();
            try
            {
               if (bankBalance >= value)
                {
                    CountBankNotes countBankNotes = new CountBankNotes();
                    return countBankNotes.GetWithdraw(value,id);
                }
                else {
                    Withdraw withdraw = new Withdraw();
                    withdraw.message = "insuficient limit";
                    withdraw.accountBalance = bankBalance;
                   return withdraw;
                }
            }
            catch (Exception ex)
            {
                Withdraw withdraw = new Withdraw();
                withdraw.message = "error: "+ex.ToString();
                withdraw.accountBalance = bankBalance;
                return withdraw;
            }
        }

        [AcceptVerbs("GET")]
        [Route("deposit/{idAdmin}/{idCustomer}/{addAditionalValue}")]
        public string setNewDepositBank(string idAdmin, string idCustomer, int addAditionalValue)
        {
            return dbConnection.setNewDepositBank(idAdmin, idCustomer, addAditionalValue);
        }
    }
}
