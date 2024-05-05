using Common.Dto;

namespace Applicant.BL.Dto
{
    public class EditApplicantProfileDto:EditProfileDto
    {
        public string? Citizenship { get; set; }
    }
}
