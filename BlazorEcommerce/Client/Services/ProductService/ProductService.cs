namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        public HttpClient _http { get; set; }
        public ProductService(HttpClient http)
        {
            this._http = http;
        }

        public List<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string LastSearchText { get; set; }

        public event Action ProductsChanged;

        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null 
                ? await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured")
                : await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
            if (result != null && result.Data != null)
                Products = result.Data;

            CurrentPage = 1;
            PageCount = 0;

            if (Products.Count == 0)
            {
                Message = "No products found";
            }

            ProductsChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;
        }

        public async Task SearchProducts(string searchText, int page)
        {
            LastSearchText = searchText;
            var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"api/product/search/{searchText}/{page}");

            if (result != null && result.Data != null)
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }

            if (Products.Count == 0)
            {
                Message = "No products found.";
            }

            ProductsChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
            return result.Data;
        }
        
    }
}
