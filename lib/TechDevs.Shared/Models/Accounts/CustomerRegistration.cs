namespace TechDevs.Shared.Models
{
    public class AuthUserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string Password { get; set; }
        public bool ChangePasswordOnFirstLogin { get; set; }

        /// <summary>
        /// An invite should be only a partial registration where the user is required to validate by email and set password
        /// </summary>
        public bool IsInvite { get; set; }
    }
    
    public class UserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string Password { get; set; }
        public bool ChangePasswordOnFirstLogin { get; set; }
        public GibsonUserType UserType { get; set; }
        
        /// <summary>
        /// An invite should be only a partial registration where the user is required to validate by email and set password
        /// </summary>
        public bool IsInvite { get; set; }

        public UserRegistration()
        {
            UserType = GibsonUserType.NotSet;
        }
    }
}