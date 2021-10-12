using Microsoft.Graph;

namespace com.loitzl.userinviter.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public UserViewModel(User graphUser)
        {
            Id = graphUser.Id;
            DisplayName = graphUser.DisplayName;
        }
    }
}