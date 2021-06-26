namespace GozemApi {
    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string SigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateLifeTime { get; set; }
        public bool ValidateAudience { get; set; }
        public int TokenExpirationInDays { get; set; }
    }
}