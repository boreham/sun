using MySql.Data.MySqlClient;
using Sun.Models;
using System.Configuration;
using System.Data;

namespace Sun.Repository;

public interface IRoleRepository
{
    Task<List<Role>> GetAllRolesAsync();
    Task<Role> GetRoleByIdAsync(int id);
    Task AddRoleAsync(Role role);
    Task UpdateRoleAsync(Role role);
    Task DeleteRoleAsync(int id);
    Task AssignRoleToUserAsync(int userId, int roleId);
    Task RemoveRoleFromUserAsync(int userId, int roleId);
}

public class RoleRepository : IRoleRepository
{
    private readonly string _connectionString;

    public RoleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    //public RoleRepository(string connectionString)
    //{
    //    _connectionString = connectionString;
    //}

    // Получить все роли
    public async Task<List<Role>> GetAllRolesAsync()
    {
        var roles = new List<Role>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Roles", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    roles.Add(new Role
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name")
                    });
                }
            }
        }

        return roles;
    }

    // Получить роль по Id
    public async Task<Role> GetRoleByIdAsync(int id)
    {
        Role role = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Roles WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    role = new Role
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name")
                    };
                }
            }
        }

        return role;
    }

    // Добавить роль
    public async Task AddRoleAsync(Role role)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO Roles (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", role.Name);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Обновить роль
    public async Task UpdateRoleAsync(Role role)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("UPDATE Roles SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", role.Id);
            command.Parameters.AddWithValue("@Name", role.Name);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Удалить роль
    public async Task DeleteRoleAsync(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("DELETE FROM Roles WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Назначить роль пользователю
    public async Task AssignRoleToUserAsync(int userId, int roleId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Удалить роль у пользователя
    public async Task RemoveRoleFromUserAsync(int userId, int roleId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand("DELETE FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            await command.ExecuteNonQueryAsync();
        }
    }
}