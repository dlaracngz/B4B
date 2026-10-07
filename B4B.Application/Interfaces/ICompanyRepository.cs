using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByDomainAsync(string domain);
        Task<List<Company>> GetAllAsync();
    }
}