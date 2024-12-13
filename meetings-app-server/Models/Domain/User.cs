//using System.ComponentModel.DataAnnotations;

//namespace meetings_app_server.Models.Domain;
//public class User
//{
//    [Key]
//    public Guid UserId { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public string PasswordHash { get; set; }
//    public string? SessionKey { get; set; }
//}


using System.ComponentModel.DataAnnotations;

namespace meetings_app_server.Models.Domain
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? SessionKey { get; set; }
    }
}

