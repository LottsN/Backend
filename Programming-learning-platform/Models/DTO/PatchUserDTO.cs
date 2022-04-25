namespace lab2.Models.DTO
{
    public class PatchUserDTO
    {
        public string password { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public PatchUserDTO(string password, string name, string surname)
        {
            this.password = password;
            this.name = name;
            this.surname = surname;
        }
    }
}
