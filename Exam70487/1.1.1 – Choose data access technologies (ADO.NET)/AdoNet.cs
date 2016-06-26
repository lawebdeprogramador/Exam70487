using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Xunit;

namespace Exam70487._1._1._1___Choose_data_access_technologies__ADO.NET_
{
    public class AdoNet
    {
        [Theory]
        [InlineData(3)]
        public static void GetCustomersWithDataAdapter(int customerId)
        {
            DataSet customerData = new DataSet("CustomerData");
            DataTable customerTable = new DataTable("Customer");
            customerData.Tables.Add(customerTable);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FirstName, LastName, CustomerId, AccountId");
            sql.Append(" FROM [dbo].[Customer] WHERE CustomerId = @CustomerId ");

            using (SqlConnection mainConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString))
            {
                using (SqlCommand customerQuery = new SqlCommand(sql.ToString(), mainConnection))
                {
                    customerQuery.Parameters.AddWithValue("@CustomerId", customerId);
                    using (SqlDataAdapter customerAdapter = new SqlDataAdapter(customerQuery))
                    {
                        try
                        {
                            customerAdapter.Fill(customerData, "Customer");
                        }
                        finally
                        {
                            if (mainConnection.State != ConnectionState.Closed)
                            {
                                mainConnection.Close();
                            }
                        }
                    }
                }
            }
            //We expected exactly 1 record to be returned.
            Assert.Equal(1, customerTable.Rows.Count);
            //The record returned has an ID different than expected.
            Assert.Equal(customerId, customerTable.Rows[0].ItemArray[customerTable.Columns["customerId"].Ordinal]);
        }

        [Theory]
        [InlineData(3)]
        public static void GetCustomersWithDataReader(int customerId)
        {
            List<Tuple<string, string, int, int>> results = new List<Tuple<string, string, int, int>>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FirstName, LastName, CustomerId, AccountId");
            sql.Append(" FROM [dbo].[Customer] WHERE CustomerId = @CustomerId ");

            using (SqlConnection mainConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString))
            {
                using (SqlCommand customerQuery = new SqlCommand(sql.ToString(),
               mainConnection))
                {
                    customerQuery.Parameters.AddWithValue("@CustomerId", customerId);
                    mainConnection.Open();
                    using (SqlDataReader reader = customerQuery.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        try
                        {
                            int firstNameIndex = reader.GetOrdinal("FirstName");
                            int lastNameIndex = reader.GetOrdinal("LastName");
                            int customerIdIndex = reader.GetOrdinal("CustomerId");
                            int accountIdIndex = reader.GetOrdinal("AccountId");
                            while (reader.Read())
                            {
                                results.Add(new Tuple<string, string, int, int>(
                                (string)reader[firstNameIndex], (string)reader[lastNameIndex],
                                (int)reader[customerIdIndex], (int)reader[accountIdIndex]));
                            }
                        }
                        finally
                        {
                            if (mainConnection.State != ConnectionState.Closed)
                            {
                                mainConnection.Close();
                            }
                        }
                    }
                }
            }

            //We expected exactly 1 record to be returned.
            Assert.Equal(1, results.Count);
            //The record returned has an ID different than expected.
            Assert.Equal(customerId, results[0].Item3);
        }

    }
}
