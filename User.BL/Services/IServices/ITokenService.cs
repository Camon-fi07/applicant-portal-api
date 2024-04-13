namespace User.BL.Services.IServices
{
    public interface ITokenService
    {
        public string GenerateAccessToken(Guid userId);
        public string GenerateRefreshToken(Guid userId);
    }
}
