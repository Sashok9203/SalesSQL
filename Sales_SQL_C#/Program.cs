using System.Configuration;
using System.Data.SqlClient;

namespace Sales_SQL_C_
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection connection = new(connStr))
            {
                connection.Open();
                Console.WriteLine("Connected!");

                //1 - Додати нову продажу/покупку
                int addedSales = AddNewSale(1, 1, 3500, DateTime.Now, connection);
                Console.WriteLine($"{addedSales} sales added...");
                Console.WriteLine();
            }

        }
        static int AddNewSale(int SellerId,int BuyerId,double SaleAmount,DateTime saleDate, SqlConnection connection)
        {
            string sqlQuery = "insert into Sales (SellerId, BuyerId, SaleAmount, SaleDate) values (@SellerId, @BuyerId, @SaleAmount, @SaleDate)";
            SqlCommand cmd = new SqlCommand(sqlQuery, connection);
            cmd.Parameters.AddWithValue("@SellerId", SellerId);
            cmd.Parameters.AddWithValue("@BuyerId", BuyerId);
            cmd.Parameters.AddWithValue("@SaleAmount", SaleAmount);
            cmd.Parameters.AddWithValue("@SaleDate", saleDate.Date);
            return cmd.ExecuteNonQuery();
        }


        //-------------Query methods--------------
        static int ExecuteNonQuery(string comStr, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand(comStr, connection);
            return command.ExecuteNonQuery();
        }
    }
   
}