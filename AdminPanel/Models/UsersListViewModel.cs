using Common.Dto.User;
using Common.Enums;

namespace AdminPanel.Models
{
    public class UsersListViewModel
    {
        public List<ProfileWithRolesDto> Users { get; set; }
        public string EmailQuery { get; set; }
        public ManagerRole ManagerRole { get; set; }
    }
}
