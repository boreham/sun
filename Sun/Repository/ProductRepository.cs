using MySql.Data.MySqlClient;
using Sun.Models;
using System.Configuration;
using System.Data;

namespace Sun.Repository;

public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    //public ProductRepository(string connectionString)
    //{
    //    _connectionString = connectionString;
    //}

    // Получить все продукты
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var products = new List<Product>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Products", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Description = reader.GetString("Description"),
                        Price = reader.GetDecimal("Price"),
                        StockQuantity = reader.GetInt32("StockQuantity")
                    });
                }
            }
        }

        return products;
    }

    // Получить продукт по Id
    public async Task<Product> GetProductByIdAsync(int id)
    {
        Product product = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Products WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    product = new Product
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Description = reader.GetString("Description"),
                        Price = reader.GetDecimal("Price"),
                        StockQuantity = reader.GetInt32("StockQuantity")
                    };
                }
            }
        }

        return product;
    }

    // Добавить продукт
    public async Task AddProductAsync(Product product)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO Products (Name, Description, Price, StockQuantity) VALUES (@Name, @Description, @Price, @StockQuantity)", connection);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Обновить продукт
    public async Task UpdateProductAsync(Product product)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, StockQuantity = @StockQuantity WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Удалить продукт
    public async Task DeleteProductAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("DELETE FROM Products WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
