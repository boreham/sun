using MySql.Data.MySqlClient;
using Sun.Models;
using System.Data;

namespace Sun.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task AddUserAsync(User user);
    Task<User> GetUserByUsernameAsync(string username);
    Task<int> UpdateUserAsync(User user);
    Task<int> DeleteUserAsync(string username);
}

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = new List<User>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Users", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32("Id"),
                        UserName = reader.GetString("UserName"),
                        Email = reader.GetString("Email"),
                        PasswordHash = reader.GetString("PasswordHash")
                    });
                }
            }
        }

        return users;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        User user = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    user = new User
                    {
                        Id = reader.GetInt32("Id"),
                        UserName = reader.GetString("UserName"),
                        Email = reader.GetString("Email"),
                        PasswordHash = reader.GetString("PasswordHash")
                    };
                }
            }
        }

        return user;
    }

    public async Task AddUserAsync(User user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO Users (UserName, Email, PasswordHash) VALUES (@UserName, @Email, @PasswordHash)", connection);
            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Асинхронно обновить данные пользователя
    public async Task<int> UpdateUserAsync(User user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = "UPDATE Users SET Email = @Email, Role = @Role WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@UserName", user.UserName);

                return await command.ExecuteNonQueryAsync();
            }
        }
    }

    // Асинхронно получить пользователя по имени
    public async Task<User> GetUserByUsernameAsync(string username)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = "SELECT * FROM Users WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", username);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32("Id"),
                            UserName = reader.GetString("UserName"),
                            PasswordHash = reader.GetString("PasswordHash"),
                            Email = reader.GetString("Email"),
                            Role = reader.GetString("Role")
                        };
                    }
                    return null;
                }
            }
        }
    }

    // Асинхронный метод для удаления пользователя
    public async Task<int> DeleteUserAsync(string username)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = "DELETE FROM Users WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", username);

                return await command.ExecuteNonQueryAsync();  // Возвращает количество удаленных строк
            }
        }
    }
}
