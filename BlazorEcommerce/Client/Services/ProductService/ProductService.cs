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

        public event Action ProductsChanged;

        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null 
                ? await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured")
                : await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
            if (result != null && result.Data != null)
                Products = result.Data;

            ProductsChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;
        }

        public async Task SearchProducts(string searchText)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchText}");

            if (result != null && result.Data != null)
            {
                Products = result.Data;
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
