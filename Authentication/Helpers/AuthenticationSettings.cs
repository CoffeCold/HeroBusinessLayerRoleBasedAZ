namespace Authentication.Helpers
{
    public class AuthenticationSettings
    {
        public string Secret { get; set; }
        public string[] AllowedRoles { get; set; }
    }
}