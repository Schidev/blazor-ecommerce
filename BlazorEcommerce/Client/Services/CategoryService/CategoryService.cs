namespace BlazorEcommerce.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        public HttpClient _http { get; set; }
        public CategoryService(HttpClient http)
        {
            this._http = http;
        }

        public List<Category> Categories { get; set; }

        public async Task GetCategories()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
            if (response != null && response.Data != null)
                Categories = response.Data;
        }
    }
}
