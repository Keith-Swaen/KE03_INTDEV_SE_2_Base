namespace DataAccessLayer.Models
{
    // Model voor een admin/gebruiker die kan inloggen.
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
