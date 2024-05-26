using Common.Dto.Applicant;
using Common.Dto.User;

namespace AdminPanel.Models
{
    public class ApplicantViewModel
    {
        public EditProfileDto EditProfile { get; set; }
        public ProfileDto Profile { get; set; }
        public EditApplicantDataDto EditApplicantData { get; set; }
        public ApplicantDataDto ApplicantData { get; set; }

    }
}
