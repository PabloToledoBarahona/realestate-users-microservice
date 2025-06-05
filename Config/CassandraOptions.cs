namespace UsersService.Config
{
    public class CassandraOptions
    {
        public string[]? ContactPoints { get; set; }
        public int Port { get; set; }
        public string? Keyspace { get; set; }
    }
}