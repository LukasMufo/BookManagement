using BookManagement.DbContexts;
using BookManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookManagement_Tests
{
    public class BooksCntrl_IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BooksCntrl_IntegrationTest(WebApplicationFactory<Program> factory)
        {
            var webHostBuilder = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            });
            _client = webHostBuilder.CreateClient();
        }
        private async Task<HttpResponseMessage> GetBookResponse(string title, string author)
        {
            var book = new Book { Title = title, Author = author };
            var json = JsonSerializer.Serialize(book);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/books", content);
        }

        [Fact]
        public async Task Create_ValidBook_ReturnsCreatedAsync()
        {
            // Act
            var response = await GetBookResponse("Test Book", "Test Book");

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains("/api/Books?id=", response.Headers.Location.ToString());

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var createdBook = await _client.GetAsync(response.Headers.Location);
                createdBook.EnsureSuccessStatusCode(); // Ensure the GET request was successful
            }
        }

        [Fact]
        public async Task Create_InvalidBook_ReturnsBadRequest()
        {
            //Act
            var response = await GetBookResponse(null, "Test Book");
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async Task GetByBook_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = -1;

            // Act
            var response = await _client.GetAsync($"/api/borrowedbooks/book?id={invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
