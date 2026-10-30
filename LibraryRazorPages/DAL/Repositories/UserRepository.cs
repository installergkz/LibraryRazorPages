using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.DTO;
using LibraryRazorPages.Models;
using System.Data;

namespace LibraryRazorPages.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<User> _logger;
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition
            (
               commandText: Queries.StoredProcedures.CreateUser,
               parameters: new { user.Login, user.PasswordHash, user.Role },
               commandType: CommandType.StoredProcedure,
               cancellationToken: cancellationToken
            );

            await using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(command);
        }

        public async Task<bool> ExistsAsync(string login, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition
            (
               commandText: Queries.StoredProcedures.CheckUserExists,
               parameters: new { Login = login },
               commandType: CommandType.StoredProcedure,
               cancellationToken: cancellationToken
            );

            await using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<bool>(command);
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition
            (
               commandText: Queries.StoredProcedures.GetUsers,
               commandType: CommandType.StoredProcedure,
               cancellationToken: cancellationToken
            );

            await using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<UserDto>(command);
            return result.AsList();
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition
            (
               commandText: Queries.StoredProcedures.GetUserByLogin,
               commandType: CommandType.StoredProcedure,
               parameters: new { Login = login },
               cancellationToken: cancellationToken
            );

            await using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(command);
        }

        public async Task<bool> UpdateRoleAsync(int id, string role, string currentUserLogin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition
            (
               commandText: Queries.StoredProcedures.UpdateUserRole,
               commandType: CommandType.StoredProcedure,
               parameters: new { Id = id, Role = role, CurrentUserLogin = currentUserLogin },
               cancellationToken: cancellationToken
            );

            await using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(command);
            return rowsAffected > 0;
        }

    }
}
