using B4B.Application.DTOs.Product;
using B4B.Application.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace B4B.Infrastructure.Services
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ElasticsearchClient _client;

        private const string IndexName = "products";

        private readonly IProductRepository _productRepository;
        private readonly ICompanyRepository _companyRepository;

        public ProductSearchService(
            ElasticsearchClient client,
            IProductRepository productRepository,
            ICompanyRepository companyRepository)
        {
            _client = client;
            _productRepository = productRepository;
            _companyRepository = companyRepository;
        }

        public async Task IndexProductAsync(
            ProductResponseDto product,
            Guid companyId)
        {
            var document = new ProductSearchDocument
            {
                Id = product.Id,
                CompanyId = companyId,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive
            };

            await _client.IndexAsync(
                document,
                x => x
                .Index(IndexName)
                .Id(product.Id)
            );
        }

        public async Task<List<ProductResponseDto>> SearchAsync(
            Guid companyId,
            string search)
        {
            // Kullanıcı hiçbir şey yazmadıysa Elasticsearch'e boşuna istek atma
            if (string.IsNullOrWhiteSpace(search))
            {
                return new List<ProductResponseDto>();
            }

            // Arama işlemi
            var response = await _client.SearchAsync<ProductSearchDocument>(
                s => s
                    .Indices(IndexName)
                    .Size(50)
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                sh => sh.Match(m => m
                                    .Field(f => f.Name)
                                    .Query(search)
                                    .Operator(Operator.And)
                                ),
                                sh => sh.Match(m => m
                                    .Field(f => f.ProductCode)
                                    .Query(search)
                                    .Operator(Operator.And)
                                )
                            )
                            .MinimumShouldMatch(1)
                            .Filter(
                                f => f.Term(t => t
                                .Field("companyId.keyword")
                                .Value(companyId.ToString())
                                )
                            )
                        )
                    )
            );

            if (!response.IsValidResponse)
                return new List<ProductResponseDto>();

            return response.Documents
                .Select(x => new ProductResponseDto
                {
                    Id = x.Id,
                    ProductCode = x.ProductCode,
                    Name = x.Name,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public async Task IndexAllProductsAsync()
        {
            // Eski Elasticsearch indexini sil
            await _client.Indices.DeleteAsync(IndexName);

            var companies = await _companyRepository.GetAllAsync();

            foreach (var company in companies)
            {
                var products = await _productRepository
                    .GetByCompanyIdAsync(company.Id);

                foreach (var product in products)
                {
                    var document = new ProductSearchDocument
                    {
                        Id = product.Id,
                        CompanyId = product.CompanyId,
                        ProductCode = product.ProductCode,
                        Name = product.Name,
                        Price = product.Price,
                        StockQuantity = product.StockQuantity,
                        IsActive = product.IsActive
                    };

                    var response = await _client.IndexAsync(
    document,
    x => x
        .Index(IndexName)
        .Id(product.Id)
);

                    if (!response.IsValidResponse)
                    {
                        throw new Exception(
                            $"Elasticsearch ürün indexleme hatası: {response.DebugInformation}"
                        );
                    }
                }
            }
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var response = await _client.DeleteAsync<ProductSearchDocument>(
                productId,
                x => x.Index(IndexName)
            );

            if (!response.IsValidResponse)
            {
                throw new Exception(
                    $"Elasticsearch ürün silme hatası: {response.DebugInformation}"
                );
            }
        }

    }
}