using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests;

public class OcelotIntegrationTests
{
    private string _mealId;
    private string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiJiODIyNTJmNi0xMjkyLTQyNzYtOTJhNC1iMzM0NjhlMWJjNzUiLCJleHAiOjE3MzQ3NTExMDJ9.EqiglxrVALmHTsCxk5qEWDa8cDLG9NWM--OFDLjKvcE";

    [Test]
    public async Task GetAllMeals_ReturnsOk()
    {
        WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        HttpClient client = webHost.CreateClient();

        // Act
        var response = await client.GetAsync("/Meals");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var meals = await response.Content.ReadFromJsonAsync<IEnumerable<Meal>>();
        _mealId = meals.First().Id;
        Assert.NotNull(meals);
    }

    [Test]
    public async Task GetMealById_ReturnsOk()
    {
        WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        HttpClient client = webHost.CreateClient();

        // Arrange
        var mealId = _mealId; 
        var request = new HttpRequestMessage(HttpMethod.Get, $"/Meals/{mealId}");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var meal = await response.Content.ReadFromJsonAsync<Meal>(); 
        Assert.NotNull(meal);
    }

    [Test]
    public async Task CreateMeal_ReturnsCreated()
    {
        WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        HttpClient client = webHost.CreateClient();

        // Arrange
        var newMeal = new MealDto("Name", "Ingredients", "Calories");
        var jwt = this._token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new HttpRequestMessage(HttpMethod.Post, "/Meals")
        {
            Content = JsonContent.Create(newMeal)
        };

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var createdMeal = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(createdMeal);
    }

    [Test]
    public async Task DeleteMeal_ReturnsOk()
    {
        await GetAllMeals_ReturnsOk();
        WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        HttpClient client = webHost.CreateClient();

        // Arrange
        var mealId = _mealId;
        var jwt = this._token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/Meals/{mealId}");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UnauthorizedRequest_ReturnsUnauthorized()
    {
        WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        HttpClient client = webHost.CreateClient();

        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Delete, "/Meals/123");

        // Simulate no authentication header

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    private record Meal(string Id, string Name, string Ingredients, string Calories);
    private record MealDto(string Name, string Ingredients, string Calories);
}