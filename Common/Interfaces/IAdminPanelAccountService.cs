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
    public interface IAdminPanelAccountService
    {
        Task<TokenResponseDto> Login(LoginDto loginDto);
    }
}
