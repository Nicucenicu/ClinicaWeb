namespace ClinicaWeb.Shared.Dtos.Users
{
    public class ResetPasswordRequestDto
    {
        public Guid ResetGuid { get; set; }
        public string Password { get; set; }
    }
}
