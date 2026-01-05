public record class LoginResponse : Response
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string UserType { get; set; }

}