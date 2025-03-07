using RestSharp;
using System.Text.Json;

public class SalesforceService
{
    private readonly IConfiguration _configuration;
    private string _accessToken;
    private string _instanceUrl;

    public SalesforceService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // 🔹 Authenticate to get Access Token
    public async Task AuthenticateAsync()
    {
        var clientId = _configuration["Salesforce:ClientId"];
        var clientSecret = _configuration["Salesforce:ClientSecret"];
        var username = _configuration["Salesforce:Username"];
        var password = _configuration["Salesforce:Password"] + _configuration["Salesforce:SecurityToken"];

        var client = new RestClient("https://login.salesforce.com/services/oauth2/token");
        var request = new RestRequest(Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "password");
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("username", username);
        request.AddParameter("password", password);

        var response = await client.ExecuteAsync(request);
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(response.Content);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Salesforce authentication failed: {response.Content}");
        }

        _accessToken = jsonResponse.GetProperty("access_token").GetString();
        _instanceUrl = jsonResponse.GetProperty("instance_url").GetString();
    }

    // 🔹 Create an Account in Salesforce
    public async Task<string> CreateAccountAsync(string accountName)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            await AuthenticateAsync();
        }

        var client = new RestClient($"{_instanceUrl}/services/data/v58.0/sobjects/");
        var request = new RestRequest("Account", RestSharp.Method.Post);
        request.AddHeader("Authorization", $"Bearer {_accessToken}");
        request.AddJsonBody(new { Name = accountName });

        var response = await client.ExecuteAsync<CreateResponse>(request);
        if (!response.IsSuccessful)
        {
            throw new Exception($"Salesforce Account creation failed: {response.Content}");
        }

        return response.Data.Id; // Returns the Salesforce Account ID
    }

    // 🔹 Create a Contact in Salesforce & Link to Account
    public async Task CreateContactAsync(string accountId, string firstName, string lastName, string email)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            await AuthenticateAsync();
        }

        var client = new RestClient($"{_instanceUrl}/services/data/v58.0/sobjects/");
        var request = new RestRequest("Contact", RestSharp.Method.Post);
        request.AddHeader("Authorization", $"Bearer {_accessToken}");
        request.AddJsonBody(new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            AccountId = accountId
        });

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception($"Salesforce Contact creation failed: {response.Content}");
        }
    }

    // 🔹 Helper class to parse Salesforce API response
    private class CreateResponse
    {
        public string Id { get; set; }
    }
}
