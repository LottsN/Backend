namespace lab2.Models.DTO
{
    public class RegisterDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }
                           

        public RegisterDTO(string username, string password, string name = null, string surname = null)
        {
            this.username = username;
            this.password = password;
            this.name = name;
            this.surname = surname;
        }
    }
}
