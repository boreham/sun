using MySql.Data.MySqlClient;
using Sun.Models;
using System.Data;

namespace Sun.Repository;

public interface IAccountRepository
{
    Task<User> LoginUserByUsernameAsync(string username);
    Task<int> RegisterUserAsync(User user);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByUsernameAsync(string username);
    Task SavePasswordResetTokenAsync(string username, string token);
}

public class AccountRepository : IAccountRepository
{
    private readonly string _connectionString;

    public AccountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    //public UserRepository(string connectionString)
    //{
    //    _connectionString = connectionString;
    //}

    // Вход
    public async Task<User> LoginUserByUsernameAsync(string username)
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

    // Регистрация
    public async Task<int> RegisterUserAsync(User user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = "INSERT INTO Users (UserName, PasswordHash, Email, Role) VALUES (@UserName, @PasswordHash, @Email, @Role)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);

                return await command.ExecuteNonQueryAsync();
            }
        }
    }

    // Получение пользователя по email
    public async Task<User> GetUserByEmailAsync(string email)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT * FROM Users WHERE Email = @Email";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString()
                    };
                }
            }
        }

        return null;
    }

    // Сохранение токена для сброса пароля
    public async Task SavePasswordResetTokenAsync(string username, string token)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "UPDATE Users SET ResetToken = @Token WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@UserName", username);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    // Удаление токена после сброса пароля
    public async Task DeletePasswordResetTokenAsync(string username)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "UPDATE Users SET ResetToken = NULL WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", username);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    // Обновление пароля пользователя
    public async Task UpdateUserPasswordAsync(string username, string newPassword)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "UPDATE Users SET PasswordHash = @Password WHERE UserName = @UserName";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Password", newPassword); // Простой пример. В реальном приложении необходимо использовать хэширование паролей
                command.Parameters.AddWithValue("@UserName", username);

                await command.ExecuteNonQueryAsync();
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
}