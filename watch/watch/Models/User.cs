using SQLite;

namespace watch
{
    public class User
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool DelFlag { get; set; } = false;
    }
}