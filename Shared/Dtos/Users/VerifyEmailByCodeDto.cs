namespace ClinicaWeb.Shared.Dtos.Users
{
    public class VerifyEmailByCodeDto
    {
        public string Email { get; set; } = string.Empty;
        public Guid? VerificationCode { get; set; }
    }
}
