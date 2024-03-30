namespace ClinicaWeb.Shared.Dtos.Users
{
    public class LoginResult
    {
        public string Token { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public bool IsSuccessfull { get; set; } = true;
        public bool IsFirstLogin { get; set; } = false;
    }

}
