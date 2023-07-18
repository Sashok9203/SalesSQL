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

                //2. Відобразити інформацію про всі продажі за певний період
                GetSalesInfo(new DateTime(2023,7,1), new DateTime(2023, 7, 18),  connection);
                Console.WriteLine();
            }

        }
        //1
        static int AddNewSale(int SellerId,int BuyerId,double SaleAmount,DateTime saleDate, SqlConnection connection)
        {
            string sqlQuery = "insert into Sales (SellerId, BuyerId, SaleAmount, SaleDate) values (@SellerId, @BuyerId, @SaleAmount, @SaleDate)";
            SqlCommand cmd = new (sqlQuery, connection);
            cmd.Parameters.AddWithValue("@SellerId", SellerId);
            cmd.Parameters.AddWithValue("@BuyerId", BuyerId);
            cmd.Parameters.AddWithValue("@SaleAmount", SaleAmount);
            cmd.Parameters.AddWithValue("@SaleDate", saleDate.Date);
            return cmd.ExecuteNonQuery();
        }
        //2
        static void GetSalesInfo(DateTime startDate, DateTime endDate, SqlConnection connection)
        {
            string sqlQuery = "select concat(sl.Name,' ',sl.Surname ) as [Seller], concat(b.Name,' ',b.Surname ) as [Buyer],s.SaleDate as [Sale Date],s.SaleAmount as [Sale Amount] from Sales as s join Sellers as sl on sl.Id = s.SellerId join Buyers as  b on b.Id = s.BuyerId where s.SaleDate between @startDate and @endDate order by s.SaleDate";
            SqlCommand cmd = new (sqlQuery, connection);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);
            SqlDataReader reader = cmd.ExecuteReader();
            ReaderShow(reader);
            reader.Close();
        }

        //-------------Query help methods--------------
        static void ReaderShow(SqlDataReader reader)
        {
            string divider = "  " + new string('-', 101) ;
            string startformater = "  |  {0,-20}";
            string endFormater = startformater + "  |\n";
            Console.WriteLine(divider);
            for (int i = 0; i < reader.FieldCount; i++)
                 Console.Write(String.Format((i == reader.FieldCount - 1) ? endFormater : startformater, reader.GetName(i)));
            Console.WriteLine(divider);
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string result,formater = (i == reader.FieldCount - 1) ? endFormater : startformater;
                    switch (reader[i])
                    {
                        case decimal:
                            result = String.Format(formater, Math.Round((decimal)reader[i], 2));
                            break;
                        case DateTime:
                            result = String.Format(formater, ((DateTime)reader[i]).ToShortDateString());
                            break;
                        default:
                            result = String.Format(formater, reader[i]);
                            break;
                    }
                     Console.Write(result);
                }
            }
            Console.Write(divider);
        }
    }
   
}