using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Common.Enums
{
    public enum RoleType
    {
        Applicant,
        Manager,
        Administrator,
        GeneralManager
    }
}
