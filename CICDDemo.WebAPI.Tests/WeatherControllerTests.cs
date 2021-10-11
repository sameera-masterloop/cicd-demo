using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CICDDemo.WebAPI.Tests
{
    public class WeatherControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WeatherControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ByDefault_ShouldReturnOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("weatherforecast");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_ByDefault_ShouldReturnListOfForecast()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("weatherforecast");
            var result = await GetContent<WeatherForecast[]>(response);

            response.EnsureSuccessStatusCode();
            Assert.Equal(5, result.Length);
        }

        [Fact]
        public async Task Get_WhenInvalidDayIsProvided_ShouldReturnBadRequest()
        {
            var day = 6;
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"weatherforecast/{day}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_WhenDayIsProvided_ShouldReturnForecastForThatDay()
        {
            var day = 1;
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"weatherforecast/{day}");
            var result = await GetContent<WeatherForecast>(response);
            var expectedDate = DateTime.Now.AddDays(day).Day;

            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
            Assert.Equal(expectedDate, result.Date.Day);
        }


        private async Task<T> GetContent<T>(HttpResponseMessage response) where T : class
        {
            if (response == null) return null;

            var contentAsString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentAsString);
        }
    }
}