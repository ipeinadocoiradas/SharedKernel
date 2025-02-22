namespace BankAccounts.Acceptance.Tests.Shared.Authentication
{
    /// <summary>
    /// Default values used by bearer authentication.
    /// </summary>
    public static class FakeJwtBearerDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme property in the JwtBearerAuthenticationOptions
        /// </summary>
        public const string AuthenticationScheme = "FakeBearer";
    }
}
