namespace B4B.Application.Interfaces
{
    public interface ITenantService
    {
        Task<Guid?> GetCurrentCompanyIdAsync();
    }
}