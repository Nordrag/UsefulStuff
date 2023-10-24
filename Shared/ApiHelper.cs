using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


public class Response<T>
{
    public bool Success { get; set; }
    public T ResponseObject { get; set; }
    public string Message { get; set; }

    public Response(bool success, T obj, string msg)
    {
        Success = success;
        ResponseObject = obj;
        Message = msg;
    }

    public Response() { }
}

public class ApiHelper
{
    string baseAddress = "https://localhost:7230/";
    HttpClient client;
    string token;

    public ApiHelper(string token)
    {
        SetAuthHeader(token);
    }

    public ApiHelper()
    {
        
    }

    public void SetAddressToServer()
    {
        SetUrl("https://arduinowebapi.azure-api.net/");      
    }

    public async Task<HttpStatusCode> GetResponseCode(string apiRoute)
    {
        RenewClient();
        HttpResponseMessage responseMessage = await client.GetAsync(apiRoute);
        return responseMessage.StatusCode;
    }

    public async Task<Response<T>> GetRequest<T>(string apiRoute)
    {
        RenewClient();       
        HttpResponseMessage responseMessage = await client.GetAsync(apiRoute);

        Response<T> response = new(responseMessage.IsSuccessStatusCode, await responseMessage.Content.ReadFromJsonAsync<T>(), responseMessage.ReasonPhrase);

        return response;
    }
    public async Task<HttpResponseMessage> PostRequest<T>(string apiRoute, T obj)
    {
        RenewClient();       
        return await client.PostAsJsonAsync<T>(apiRoute, obj);      
    }

    public async Task<Response<R>> Post<R,T>(string apiRoute, T obj)
    {
        RenewClient();
        var res = await client.PostAsJsonAsync<T>(apiRoute, obj);
        Response<R> response = new();
        response.Success = res.IsSuccessStatusCode;
        response.ResponseObject = await res.Content.ReadFromJsonAsync<R>();
        response.Message = res.ReasonPhrase;
        return response;
    }

    public async Task<HttpResponseMessage> DeleteRequest(string apiRoute)
    {
        RenewClient();
        return await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, apiRoute));
    }

    public async Task<Response<R>> PutRequest<T, R>(string apiRoute, T obj)
    {
        RenewClient();      
        var res = await client.PutAsJsonAsync<T>(apiRoute, obj);
        return new Response<R>(res.IsSuccessStatusCode, await res.Content.ReadFromJsonAsync<R>(), res.ReasonPhrase);
    }

    public async Task<HttpResponseMessage> Put(string apiRoute)
    {
        RenewClient();
        return await client.PutAsync(apiRoute, null);
    }  

    public void SetBaseUrlDebug()
    {
        baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5209" : "https://localhost:7230/";
    }

    /// <summary>
    /// default url = 192.168.100.6/
    /// </summary>
    /// <param name="url"></param>
    public void SetUrl(string url)
    {
        baseAddress = url;
    }

    public void SetAuthHeader(string Token)
    {      
        //thats kekw son
        var res = Token.Replace('"', ' ');
        token = res.Trim();
    }

    void RenewClient()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(baseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

