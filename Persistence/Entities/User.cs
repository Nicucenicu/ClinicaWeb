using ClinicaWeb.Shared.Enums;

namespace ClinicaWeb.Persistence.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsEmailVerified{ get; set; }

        public Role Role { get; set; }
        public int? LoginAttempts { get; set; }
        public int LoginsCount { get; set; }

        public Guid? VerificationCode { get; set; }
        public string VerificationToken { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public int? PhotoId { get; set; }

        public DateTime? BirthdayDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        
    }
}
