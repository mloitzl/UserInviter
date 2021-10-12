namespace com.loitzl.userinviter.Models
{
    public static class GraphConstants
    {
        // Defines the permission scopes used by the app
        public readonly static string[] Scopes =
        {
            "User.Read",
            "MailboxSettings.Read",
            "User.Invite.All", 
            "User.ReadWrite.All", 
            "Directory.ReadWrite.All"
        };
    }
}