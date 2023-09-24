namespace BusinessLayer.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
    }
}