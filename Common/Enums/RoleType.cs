using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text.Json.Serialization;

namespace Common.Enums
{
    [JsonConverter((typeof(JsonStringEnumConverter)))]
    public enum RoleType
    {
        Applicant,
        Manager,
        Administrator,
        GeneralManager
    }
}
