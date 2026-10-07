namespace B4B.Application.DTOs.Admin
{
    public class AdminUserResponseDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }

        public bool IsActive { get; set; }
    }
}