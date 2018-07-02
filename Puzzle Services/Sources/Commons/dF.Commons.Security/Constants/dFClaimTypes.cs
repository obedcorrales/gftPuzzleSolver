namespace dF.Commons.Security.Constants
{
    public static class dFClaimTypes
    {
        public const string Resource = "rsrc";
        public const string IpAddress = "ip";

        public static class JWT
        {
            public const string Issuer = "iss";
            public const string Subject = "sub";
            public const string Audience = "aud";
            public const string ExpirationTime = "exp";
            public const string NotBefore = "nbf";
            public const string IssuedAt = "iat";
            public const string JwtID = "jti";
            public const string TokenType = "typ";
            public const string ContentType = "cty";
            public const string Algorithm = "alg";
        }

        public static class Injected
        {
            public const string ForUserId = "iUsrId";
            public const string Role = "iRole";
            public const string Name = "iName";
            public const string Authentication = "iAuthentication";
        }
    }
}
