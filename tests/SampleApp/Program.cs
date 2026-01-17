using System.Net.Http.Json;
using System.Text.Json;

Console.WriteLine("FormR Sample Application");
Console.WriteLine("========================\n");

var apiBaseUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";
var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

Console.WriteLine($"API Base URL: {apiBaseUrl}");
Console.WriteLine($"Testing API connectivity...\n");

try
{
    // Test 1: Get Control Library
    Console.WriteLine("Test 1: Fetching Control Library...");
    var libraryResponse = await httpClient.GetAsync("/api/v1/controls/library");

    if (libraryResponse.IsSuccessStatusCode)
    {
        var library = await libraryResponse.Content.ReadFromJsonAsync<List<ControlLibraryDto>>();
        Console.WriteLine($"✓ Success! Found {library?.Count ?? 0} control types");
        Console.WriteLine($"  Sample controls: {string.Join(", ", library?.Take(5).Select(c => c.Type) ?? Array.Empty<string>())}\n");
    }
    else
    {
        Console.WriteLine($"✗ Failed: {libraryResponse.StatusCode}\n");
    }

    // Test 2: Create a Sample Template
    Console.WriteLine("Test 2: Creating a sample contact form template...");

    var createTemplateDto = new
    {
        name = "Contact Form",
        description = "A simple contact form with name, email, and message",
        controls = new[]
        {
            new
            {
                type = "TextInput",
                label = "Full Name",
                placeholder = "Enter your full name",
                defaultValue = "",
                isRequired = true,
                validationRules = new { minLength = 2, maxLength = 100 },
                position = new { x = 20, y = 20, width = 300, height = 40 },
                properties = new { },
                order = 0
            },
            new
            {
                type = "EmailInput",
                label = "Email Address",
                placeholder = "your.email@example.com",
                defaultValue = "",
                isRequired = true,
                validationRules = new { },
                position = new { x = 20, y = 80, width = 300, height = 40 },
                properties = new { },
                order = 1
            },
            new
            {
                type = "PhoneInput",
                label = "Phone Number",
                placeholder = "(123) 456-7890",
                defaultValue = "",
                isRequired = false,
                validationRules = new { },
                position = new { x = 20, y = 140, width = 300, height = 40 },
                properties = new { },
                order = 2
            },
            new
            {
                type = "MultilineText",
                label = "Message",
                placeholder = "Enter your message here...",
                defaultValue = "",
                isRequired = true,
                validationRules = new { minLength = 10, maxLength = 500 },
                position = new { x = 20, y = 200, width = 300, height = 120 },
                properties = new { rows = 6 },
                order = 3
            }
        }
    };

    var createResponse = await httpClient.PostAsJsonAsync("/api/v1/templates", createTemplateDto);

    if (createResponse.IsSuccessStatusCode)
    {
        var createdTemplate = await createResponse.Content.ReadFromJsonAsync<TemplateDetailDto>();
        Console.WriteLine($"✓ Success! Created template with ID: {createdTemplate?.Id}");
        Console.WriteLine($"  Name: {createdTemplate?.Name}");
        Console.WriteLine($"  Controls: {createdTemplate?.Controls.Count}\n");

        // Test 3: Retrieve the Template
        if (createdTemplate != null)
        {
            Console.WriteLine($"Test 3: Retrieving template {createdTemplate.Id}...");
            var getResponse = await httpClient.GetAsync($"/api/v1/templates/{createdTemplate.Id}");

            if (getResponse.IsSuccessStatusCode)
            {
                var retrievedTemplate = await getResponse.Content.ReadFromJsonAsync<TemplateDetailDto>();
                Console.WriteLine($"✓ Success! Retrieved template: {retrievedTemplate?.Name}");
                Console.WriteLine($"  Version: {retrievedTemplate?.Version}");
                Console.WriteLine($"  Controls: {retrievedTemplate?.Controls.Count}\n");
            }
            else
            {
                Console.WriteLine($"✗ Failed: {getResponse.StatusCode}\n");
            }

            // Test 4: List All Templates
            Console.WriteLine("Test 4: Listing all templates...");
            var listResponse = await httpClient.GetAsync("/api/v1/templates");

            if (listResponse.IsSuccessStatusCode)
            {
                var templates = await listResponse.Content.ReadFromJsonAsync<List<TemplateListDto>>();
                Console.WriteLine($"✓ Success! Found {templates?.Count ?? 0} template(s)");
                foreach (var template in templates ?? Enumerable.Empty<TemplateListDto>())
                {
                    Console.WriteLine($"  - {template.Name} (v{template.Version}, {template.ControlCount} controls)");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"✗ Failed: {listResponse.StatusCode}\n");
            }
        }
    }
    else
    {
        var errorContent = await createResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"✗ Failed: {createResponse.StatusCode}");
        Console.WriteLine($"  Error: {errorContent}\n");
    }

    Console.WriteLine("\n========================");
    Console.WriteLine("All tests completed!");
    Console.WriteLine("========================");
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ Error: {ex.Message}");
    Console.WriteLine($"  Make sure the API is running at {apiBaseUrl}");
    Environment.Exit(1);
}

// DTOs to match the API
record ControlLibraryDto(string Type, string Category, string Icon, JsonDocument ConfigSchema, JsonDocument DefaultProps);
record TemplateListDto(Guid Id, string Name, string? Description, int Version, DateTime CreatedDate, DateTime ModifiedDate, int ControlCount);
record TemplateDetailDto(Guid Id, string Name, string? Description, int Version, Guid? BaseTemplateId, DateTime CreatedDate, DateTime ModifiedDate, List<ControlDto> Controls);
record ControlDto(Guid Id, string Type, string Label, string? Placeholder, string? DefaultValue, bool IsRequired, JsonDocument? ValidationRules, JsonDocument Position, JsonDocument? Properties, Guid? ParentControlId, int Order);
