using B4B.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
    public bool IsActive { get; set; }
    public ICollection<User> Users { get; set; }
}