namespace ClinicaWeb.Shared.Dtos.Users
{
    public class ResetPasswordTokenDto
    {
        public int UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
