using Cassandra;
using Microsoft.Extensions.Options;
using UsersService.Config;

namespace UsersService.Services
{
    public class CassandraSessionFactory
    {
        private readonly CassandraOptions _options;
        private Cassandra.ISession? _session;

        public CassandraSessionFactory(IOptions<CassandraOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Cassandra.ISession> GetSessionAsync()
        {
            if (_session is null)
            {
                var cluster = Cluster.Builder()
                    .AddContactPoints(_options.ContactPoints)
                    .WithPort(_options.Port)
                    .Build();

                _session = await cluster.ConnectAsync(_options.Keyspace);
            }

            return _session;
        }
    }
}