namespace ClinicaWeb.Shared.Dtos.Users
{
    public class VerifyEmailByLinkDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; }
    }
}
