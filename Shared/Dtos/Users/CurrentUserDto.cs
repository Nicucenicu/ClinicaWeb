namespace ClinicaWeb.Shared.Dtos.Users
{
    public class CurrentUserDto
    {
        public bool IsAuthenticated { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
