using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Maui.App.Frameworks.Services;

public class RestServiceBase
{

    private HttpClient _httpClient;
    private IConnectivity _connectivity;

    protected RestServiceBase(HttpClient httpClient,IConnectivity connectivity)
    {
        _httpClient = httpClient;
        _connectivity = connectivity;
    }
    
    
    protected void SetBaseUrl(string baseUrl)
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseUrl)
        };
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));
    }


    protected void AddHttpHeader(string key, string value) =>
        _httpClient.DefaultRequestHeaders.Add(key, value);


    protected async Task<T> GetAsync<T>(string resource)
    {
        var json = await GetJsonAsync(resource);

        return JsonSerializer.Deserialize<T>(json);
    }
    private async Task<string> GetJsonAsync(string resource)
    {

        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            throw new InternetConnectionException();
        // call the api end point 
        var response = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress , resource));

        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();


        return json;
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string uri,T payload)
    {
        var dataToPost = JsonSerializer.Serialize(payload);

        var content = new StringContent(dataToPost, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(new Uri(_httpClient.BaseAddress, uri), content);

        response.EnsureSuccessStatusCode();

        return response;

    }
    
}

