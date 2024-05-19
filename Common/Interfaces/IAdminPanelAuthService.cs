using Common.Dto;
using Common.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAdminPanelAuthService
    {
        Task<TokenResponseDto> Login(LoginDto loginDto, string userAgent);
        Task Logout(Guid userId, string userAgent);

    }
}
