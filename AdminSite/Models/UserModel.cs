namespace AdminSite.Models
{
    public class UserModel
    {
        public UserRequest UserRequest { get; set; } = null;
        public string request{ get; set; }
        public UserNote Note { get; set; }
    }

    public class UserNote
    {
        public uint UserId { get; set; }
        public string Note { get; set; }
    }
}
