# B4B Multi-Tenant E-Commerce Project

B4B, birden fazla firmanın aynı sistem üzerinden ürün, sepet ve sipariş işlemlerini gerçekleştirebildiği çok kiracılı (Multi-Tenant) bir B2B e-ticaret uygulamasıdır.

## 🚀 Proje Özellikleri

- Multi-Tenant mimari
- Firma bazlı veri izolasyonu
- JWT Authentication
- Rol bazlı yetkilendirme
- Ürün ekleme, güncelleme ve silme
- Ürün arama
- Sepet yönetimi
- Sipariş oluşturma
- Sipariş geçmişi ve detayları
- Sipariş oluşturulduğunda stoktan otomatik düşme
- Stok kontrolü
- Redis ile cache yönetimi
- Elasticsearch ile ürün arama
- Admin paneli ve admin işlemleri

## 🏗️ Proje Mimarisi

Proje Clean Architecture yaklaşımına uygun olarak 4 ana katmandan oluşmaktadır:

```text
B4B
│
├── B4B.API
├── B4B.Application
├── B4B.Domain
└── B4B.Infrastructure
