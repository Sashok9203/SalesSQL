using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace Sales_SQL_C_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection connection = new(connStr))
            {
                Console.Write("Connection to database....");
                connection.Open();
                Console.WriteLine("Connected!");
                Console.ReadKey();
                Console.Clear();

                //1 - Додати нову продажу/покупку
                int sId, bId;
                double sAmmount;
                Console.WriteLine("\t1 - Додати нову продажу/покупку");
                sId =  GetInt("\tEnter seller ID : ");
                bId = GetInt("\tEnter buyer ID : ");
                sAmmount = GetDouble("\tSale ammount : ");
                try
                {
                    AddNewSale(sId, bId, sAmmount, DateTime.Now, connection);
                    Console.WriteLine("\tSale added...");
                }
                catch (SqlException sqlex) { Console.WriteLine(sqlex.Message);}
                finally { WClear(); }

                //2. Відобразити інформацію про всі продажі за певний період
                Console.WriteLine("\t2. Відобразити інформацію про всі продажі за певний період");
                int y1, y2, m1, m2, d1, d2;
                DateTime startTime, endTime;
                try
                {
                    y1 = GetInt("\tEnter start date year : ");
                    m1 = GetInt("\tEnter start date month : ");
                    d1 = GetInt("\tEnter start date day : ");
                    startTime = new DateTime(y1, m1, d1);
                    y2 = GetInt("\tEnter end date year : ");
                    m2 = GetInt("\tEnter end date month : ");
                    d2 = GetInt("\tEnter end date day : ");
                    endTime = new DateTime(y2, m2, d2);
                    GetSalesInfo(startTime, endTime, connection);
                }
                catch (SqlException sqlex) { Console.WriteLine(sqlex.Message);}
                catch (Exception ex) { Console.WriteLine(ex.Message);}
                finally { WClear(); }

                //3. Показати останню покупку певного покупця по імені та прізвищу
                Console.WriteLine("\t3. Показати останню покупку певного покупця по імені та прізвищу");
                string name, surname;
                Console.Write("Enter buyer name : ");
                name = Console.ReadLine() ?? "";
                Console.Write("Enter buyer surname : ");
                surname = Console.ReadLine() ?? "";

                try { GetLastBuy(name, surname, connection); }
                catch (SqlException sqlex) { Console.WriteLine(sqlex.Message);}
                finally { WClear(); }

                //4. Видалити продавця або покупця по id
                Console.WriteLine("\t4. Видалити продавця або покупця по id");
                bool key = GetBool("Delete ", "seller", "buyer");
                int Id = GetInt($"\tEnter {(key ? "seller" : "buyer")} ID : ");
                try
                {
                    int deleted = DelSellelBuyer(Id, connection, key);
                    Console.WriteLine($"{deleted} {(key ? "seller(s)" : "buyer(s)")} deleted...");
                }
                catch (SqlException sqlex) { Console.WriteLine(sqlex.Message); }
                finally { WClear(); }

                //5. Показати продавця, загальна сума продаж якого є найбільшою
                Console.WriteLine("\t5. Показати продавця, загальна сума продаж якого є найбільшою");
                try
                {
                    GetTopBuyer(connection);
                    Console.WriteLine();
                }
                catch (SqlException sqlex) { Console.WriteLine(sqlex.Message); }
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
            string sqlQuery = "select concat(sl.Name,' ',sl.Surname ) as [Seller], concat(b.Name,' ',b.Surname ) as [Buyer],s.SaleDate as [Sale Date],s.SaleAmount as [Sale Amount] " +
                              "from Sales as s join Sellers as sl on sl.Id = s.SellerId " +
                              "join Buyers as  b on b.Id = s.BuyerId " +
                              "where s.SaleDate between @startDate and @endDate order by s.SaleDate";
            SqlCommand cmd = new (sqlQuery, connection);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);
            SqlDataReader reader = cmd.ExecuteReader();
            ReaderShow(reader);
        }

        //3
        static void GetLastBuy(string name, string surname, SqlConnection connection)
        {
            string sqlQuery = "select top 1  concat(b.Name,' ',b.Surname) as [Buyer], s.SaleDate as [Last Buy Date],s.SaleAmount as [Sale Amount] " +
                              "from Sales as s join Buyers as b on b.Id = s.BuyerId " +
                              "where  concat(b.Name,b.Surname) = concat(@name,@surname) order by s.SaleDate desc";
            SqlCommand cmd = new (sqlQuery, connection);
            cmd.Parameters.AddWithValue("@name",name);
            cmd.Parameters.AddWithValue("@surname", surname);
            SqlDataReader reader = cmd.ExecuteReader();
            ReaderShow(reader);
        }

        //4
        static int DelSellelBuyer(int Id , SqlConnection connection , bool sellerId = true)
        {
            string sqlQuery = sellerId ? $"delete from Sellers where Sellers.Id = {Id}" : $"delete from Buyers where Buyers.Id = {Id}";
            SqlCommand cmd = new(sqlQuery, connection);
            return cmd.ExecuteNonQuery();
        }

        //5 
        static void GetTopBuyer(SqlConnection connection)
        {
            string sqlQuery = "select top 1  concat(b.Name,' ',b.Surname) as [Top Buyer],sum(s.SaleAmount) as [Sale Amount] " +
                              "from Sales as s join Buyers as b on b.Id = s.SellerId " +
                              "group by b.Name,b.Surname order by 'Sale Amount' desc";
            SqlCommand cmd = new(sqlQuery, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            ReaderShow(reader);
        }


        //------------- Help methods --------------
        static void ReaderShow(SqlDataReader reader)
        {
            string divider = "  " + new string('-', reader.FieldCount * 25 + 1) ;
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
            reader.Close();
        }

        static bool GetBool(string message,string variant1,string variant2)
        {
            int choose = 0;
            while (choose !=1 && choose != 2){ choose = GetInt($"\tYou wont {message} {variant1}[1] or {variant2}[2] : "); };
            return choose == 1;
        }

        static int GetInt(string message)
        {
            int res;
            do { Console.Write(message);}
            while (!int.TryParse(Console.ReadLine(), out res));
            return res;
        }

        static double GetDouble(string message)
        {
            double res;
            do { Console.Write(message); }
            while (!double.TryParse(Console.ReadLine(), out res));
            return res;
        }

        static void WClear()
        {
            Console.ReadKey();
            Console.Clear();
        }

    }
   
}