using System;
using Dapper;
using Npgsql;
using System.Collections.Generic;
using withdraw.Models;
using System.Linq;

namespace withdraw
{
    public class DBConnection
    {
        public String getStringConnection()
        {
            const string herokuConnectionString = @"
                                                      Host=ec2-3-233-43-103.compute-1.amazonaws.com;
                                                      Port=5432;
                                                      Username=ucrwnrhdvkpzdx;
                                                      Password=85cb02154b1e1de5e206d38509815dfe201080809ef1f596cd5d83940bea28e1;
                                                      Database=d43jo8egad9uj0;
                                                      Pooling=true;
                                                      SSL Mode=Require;
                                                      TrustServerCertificate=True;
                                                    ";
            return herokuConnectionString;
        }


        public List<Customer> getAllCustomers()
        {
            using (var connection = new NpgsqlConnection(getStringConnection()))
            {
                return (List<Customer>)connection.Query<Customer>("SELECT name, email, passwd, bankbalance, admin, id FROM public.customer order by id ");
            }
        }

        public List<int> getCustomerBankBalance(string id)
        {
            using (var connection = new NpgsqlConnection(getStringConnection()))
            {
                return (List<int>)connection.Query<int>("SELECT bankbalance FROM public.customer where id = " + id +" LIMIT 1");
            }
        }

        public List<int> setCustomerBankBalance(string id,int newBalance)
        {
            using (var connection = new NpgsqlConnection(getStringConnection()))
            {
                connection.Query("UPDATE public.customer SET bankbalance="+newBalance.ToString()+" WHERE id="+id);
                return (List<int>)connection.Query<int>("SELECT bankbalance FROM public.customer where id = " + id + " LIMIT 1");
            }
        }

        public string setNewDepositBank(string idAdmin, string idCustomer, int addAditionalValue)
        {
            using (var connection = new NpgsqlConnection(getStringConnection()))
            {
                int updateBalance = getCustomerBankBalance(idCustomer).First<int>();

                if (addAditionalValue > 0)
                {
                    connection.Query("UPDATE public.customer SET bankbalance=" + (addAditionalValue + updateBalance).ToString() + " WHERE public.customer.id=" + idCustomer + " and ((select public.customer.admin from public.customer where (public.customer.id=" + idAdmin + ") = true))");
                    int afterUpdateBalance = getCustomerBankBalance(idCustomer).First<int>();
                    if (updateBalance < afterUpdateBalance)
                    {
                        return "{\"status\":\"account update\",\"message\":\"updated with success!\",\"value\":" + afterUpdateBalance.ToString() + "}";
                    }
                    else
                    {
                        return "{\"status\":\"account not updated\",\"message\":\"is not admin\",\"value\":" + afterUpdateBalance.ToString() + "}";
                    }
                }
                else
                {
                    return "{\"status\":\"account not updated\",\"message\":\"invalid value\",\"value\":" + updateBalance.ToString() + "}";
                }
            }
        }
    }
}
