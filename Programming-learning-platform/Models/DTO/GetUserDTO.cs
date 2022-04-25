namespace lab2.Models.DTO
{
    public class GetUserDTO
    {
        public int userId { get; set; }
        public string username { get; set; }
        public int roleId { get; set; }

        public GetUserDTO(int userId, string username, int  roleId)
        {
            this.userId = userId;
            this.username = username;
            this.roleId = roleId;
        }
    }
}
