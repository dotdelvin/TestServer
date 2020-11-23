namespace TestServer.Data.Entities
{
    public class AccountEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public AccountEntity()
        {

        }

        public AccountEntity(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}