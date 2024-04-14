namespace User.BL.Dto.Auth
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
