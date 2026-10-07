using B4B.Application.Interfaces;
using B4B.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly B4BDbContext _context;

        public TenantService(
            IHttpContextAccessor httpContextAccessor,
            B4BDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<Guid?> GetCurrentCompanyIdAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            // Frontend'in gönderdiği tenant domainini al
            var domain = httpContext.Request.Headers["X-Tenant-Domain"].FirstOrDefault();

            // Header gelmezse normal Host bilgisini kullan
            if (string.IsNullOrEmpty(domain))
            {
                domain = httpContext.Request.Host.Host;
            }

            // Domain'e göre firmayı bul
            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.Domain == domain);

            if (company == null)
                return null;

            // JWT'deki CompanyId'yi al
            var jwtCompanyId = httpContext.User.FindFirst("CompanyId")?.Value;

            if (!Guid.TryParse(jwtCompanyId, out var companyId))
                return null;

            // Domain ve JWT aynı firmaya ait mi?
            if (company.Id != companyId)
                return null;

            return company.Id;
        }
    }
}