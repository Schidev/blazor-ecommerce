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

        public async Task GetProducts()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
            if (result != null && result.Data != null)
                Products = result.Data;
        }
    }
}
