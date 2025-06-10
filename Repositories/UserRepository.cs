using Cassandra;
using UsersService.Models;
using UsersService.Services;

namespace UsersService.Repositories
{
    public class UserRepository
    {
        private readonly CassandraSessionFactory _sessionFactory;

        public UserRepository(CassandraSessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task CreateAsync(User user)
        {
            // Llamamos a GetSession()
            var session = _sessionFactory.GetSession();

            var stmt = session.Prepare(@"
                INSERT INTO users_by_id (
                    id_user, first_name, last_name, email, password_hash,
                    phone_number, date_of_birth, gender, country, city,
                    created_at, updated_at
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            ");

            // Conversión a LocalDate para el tipo 'date' de Cassandra
            var cassandraDate = new LocalDate(user.DateOfBirth.Year, user.DateOfBirth.Month, user.DateOfBirth.Day);

            var bound = stmt.Bind(
                user.IdUser,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.PhoneNumber,
                cassandraDate,
                user.Gender,
                user.Country,
                user.City,
                user.CreatedAt,
                user.UpdatedAt
            );

            await session.ExecuteAsync(bound);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var session = _sessionFactory.GetSession();

            var rs = await session.ExecuteAsync(new SimpleStatement("SELECT * FROM users_by_id"));
            foreach (var row in rs)
            {
                if (row.GetValue<string>("email") == email)
                {
                    var localDate = row.GetValue<LocalDate>("date_of_birth");
                    var birthDate = new DateTime(localDate.Year, localDate.Month, localDate.Day);

                    return new User
                    {
                        IdUser = row.GetValue<Guid>("id_user"),
                        FirstName = row.GetValue<string>("first_name"),
                        LastName = row.GetValue<string>("last_name"),
                        Email = row.GetValue<string>("email"),
                        PasswordHash = row.GetValue<string>("password_hash"),
                        PhoneNumber = row.GetValue<string>("phone_number"),
                        DateOfBirth = birthDate,
                        Gender = row.GetValue<string>("gender"),
                        Country = row.GetValue<string>("country"),
                        City = row.GetValue<string>("city"),
                        CreatedAt = row.GetValue<DateTime>("created_at"),
                        UpdatedAt = row.GetValue<DateTime>("updated_at")
                    };
                }
            }

            return null;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var session = _sessionFactory.GetSession();

            var stmt = session.Prepare("SELECT * FROM users_by_id WHERE id_user = ?");
            var result = await session.ExecuteAsync(stmt.Bind(id));
            var row = result.FirstOrDefault();
            if (row == null) return null;

            var localDate = row.GetValue<LocalDate>("date_of_birth");
            var birthDate = new DateTime(localDate.Year, localDate.Month, localDate.Day);

            return new User
            {
                IdUser = row.GetValue<Guid>("id_user"),
                FirstName = row.GetValue<string>("first_name"),
                LastName = row.GetValue<string>("last_name"),
                Email = row.GetValue<string>("email"),
                PasswordHash = row.GetValue<string>("password_hash"),
                PhoneNumber = row.GetValue<string>("phone_number"),
                DateOfBirth = birthDate,
                Gender = row.GetValue<string>("gender"),
                Country = row.GetValue<string>("country"),
                City = row.GetValue<string>("city"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            };
        }

        public async Task UpdateAsync(User user)
        {
            var session = _sessionFactory.GetSession();

            var cassandraDate = new LocalDate(user.DateOfBirth.Year, user.DateOfBirth.Month, user.DateOfBirth.Day);

            var stmt = session.Prepare(@"
        UPDATE users_by_id SET 
            first_name = ?, 
            last_name = ?, 
            phone_number = ?, 
            gender = ?, 
            country = ?, 
            city = ?, 
            updated_at = ?
        WHERE id_user = ?
    ");

            var bound = stmt.Bind(
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.Gender,
                user.Country,
                user.City,
                user.UpdatedAt,
                user.IdUser
            );

            await session.ExecuteAsync(bound);
        }

        public async Task<string?> GetEmailByIdAsync(Guid id)
        {
            var session = _sessionFactory.GetSession();
            var stmt = session.Prepare("SELECT email FROM users_by_id WHERE id_user = ?");
            var result = await session.ExecuteAsync(stmt.Bind(id));
            var row = result.FirstOrDefault();
            return row?.GetValue<string>("email");
        }

    }
}