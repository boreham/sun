using MySql.Data.MySqlClient;
using Sun.Models;
using System.Data;

namespace Sun.Repository;

public interface IProfileRepository
{
    Task<User> GetUserByUsernameAsync(string username);
    Task<int> UpdateUserAsync(User user);
    Task<int> DeleteUserAsync(string username);
}

public class ProfileRepository : IProfileRepository
{
    private readonly string _connectionString;

    public ProfileRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
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
