public record class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }

}