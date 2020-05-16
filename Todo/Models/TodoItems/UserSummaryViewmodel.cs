using Todo.Services;

namespace Todo.Models.TodoItems
{
    public class UserSummaryViewmodel
    {
        public string Id { get; set; }
        public string UserName { get; }
        public string Email { get; }
        public string FullName { get; set; }
        public string GravatarHash { get; }
        
        public UserSummaryViewmodel(string id, string userName, string email)
        {
            Id = id;
            UserName = userName;
            Email = email;
            GravatarHash = Gravatar.GetHash(email);
        }
    }
}