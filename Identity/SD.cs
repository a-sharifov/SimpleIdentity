namespace Identity;

public class SD
{
    public const string DefaultCorsPolicyName = "DefaultCorsPolicy";
    public const string JwtSectionKey = "Jwt";
    public const string IdentitySectionKey = "Identity";
    public const string EmailSectionKey = "Email";

    public class Policy
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string UserAndAdmin = nameof(UserAndAdmin);

        public class Role
        {
            public const string Admin = nameof(Admin);
            public const string User = nameof(User);
        }
    }
}   
