using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using YourNamespace;
//Создайте в SSMS таблицу инетренат магазина, которая будет хранить в себе поля по продукту.
//Наполните 5 продуктами.
//После чего установите отсоединенный режим подключения к данной БД и таблице. 
//Добавьте 5 продуктов. Измените продукты которые уже были в бд. Изменить 5 продуктов(по одному свойству)


//CREATE DATABASE InternetShopProducts
//USE  InternetShopProducts

//CREATE TABLE ShopProducts
//(
//    ProductID INT PRIMARY KEY IDENTITY(1,1),
//    ProductName NVARCHAR(255) NOT NULL,
//    Price DECIMAL(10, 2) NOT NULL,
//    Category NVARCHAR(100) NULL
//);


//INSERT INTO ShopProducts (ProductName, Price, Category)
//VALUES
//('Смартфон XYZ', 199, 'Электроника'),
//('Кофта', 250, 'Одежда'),
//('Ноутбук ', 600, 'Электроника'),
//('Холодильник ', 300, 'Бытовая техника'),
//('Книга ', 30, 'Книги');

//SELECT* FROM ShopProducts;



namespace YourNamespace
{
    public class DatabaseManager
    {
        private string _connectionString = @"Data Source=WIN-RO38MGBKCE5;Initial Catalog=InternetShopProducts;Trusted_Connection=True;TrustServerCertificate=True";

        public async Task ConnectToDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Подключение октрыто");
                Console.WriteLine(connection.DataSource.ToString());
            }

        }

        public async Task<IEnumerable<string>> GetTableData()
        {
            List<string> results = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM ShopProducts"; 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                StringBuilder row = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Append(reader[i].ToString() + "\t"); 
                                }
                                results.Add(row.ToString());
                            }
                        }

                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Ошибка при выполнении SQL-запроса: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла ошибка: " + ex.Message);
                    }

                }
            }
            return results;
        }
        public async Task<int> InsertProductsAsync()
        {
            string sqlExpression = @"
INSERT INTO ShopProducts (ProductName,  Price,  Category) VALUES
('Часы', 100, 'Электроника'),
('Тостер', 150, 'Электроника'),
('Джинсы', 200, 'Одежда'),
('Футболка', 250, 'Одежда'),
('Телевизор', 300, 'Электроника')";


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int numberOfInsertedRows = await command.ExecuteNonQueryAsync();
                return numberOfInsertedRows;
            }
        }
        public async Task<int> UpdateProductsAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                string updateExpression = @"
        UPDATE ShopProducts SET Price = 105 WHERE ProductName = 'Смартфон ';
        UPDATE ShopProducts SET Price = 155 WHERE ProductName = 'Кофта';
        UPDATE ShopProducts SET Price = 205 WHERE ProductName = 'Ноутбук';
        UPDATE ShopProducts SET Price = 255 WHERE ProductName = 'Холодильник';
        UPDATE ShopProducts SET Price = 305 WHERE ProductName = 'Книга';";

                SqlCommand command = new SqlCommand(updateExpression, connection);
                int numberOfUpdatedRows = await command.ExecuteNonQueryAsync();
                return numberOfUpdatedRows;
            }
        }


    }

    class Program
    {
        static async Task Main()
        {
            DatabaseManager dbManager = new DatabaseManager();

           
            await dbManager.ConnectToDatabase();

           
            Console.WriteLine("Данные из таблицы ShopProducts до внесения изменений:");
            var data = await dbManager.GetTableData();
            PrintData(data);

            // Добавление новых продуктов
            int insertedProducts = await dbManager.InsertProductsAsync();
            Console.WriteLine($"\nДобавлено продуктов: {insertedProducts}");

          
            Console.WriteLine("\nДанные из таблицы ShopProducts после добавления новых продуктов:");
            data = await dbManager.GetTableData();
            PrintData(data);

          
            int updatedProducts = await dbManager.UpdateProductsAsync();
            Console.WriteLine($"\nОбновлено продуктов: {updatedProducts}");

          
            Console.WriteLine("\nДанные из таблицы ShopProducts после обновления:");
            data = await dbManager.GetTableData();
            PrintData(data);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void PrintData(IEnumerable<string> data)
        {
            foreach (var item in data)
            {
                Console.WriteLine(item);
            }
        }
    }

}

