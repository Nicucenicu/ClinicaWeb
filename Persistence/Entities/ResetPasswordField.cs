namespace ClinicaWeb.Persistence.Entities
{
    public class ResetPasswordField
    {
        public int Id { get; set; }
        public Guid PasswordGuid { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
