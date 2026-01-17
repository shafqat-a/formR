# FormR Sample Application

This is a sample console application that demonstrates using the FormR API to create and manage form templates.

## What it does

The sample app performs the following tests:

1. **Fetches the Control Library** - Retrieves all available control types from the API
2. **Creates a Sample Template** - Creates a "Contact Form" template with 4 controls:
   - Full Name (TextInput with validation)
   - Email Address (EmailInput, required)
   - Phone Number (PhoneInput, optional)
   - Message (MultilineText with min/max length validation)
3. **Retrieves the Template** - Fetches the created template by ID
4. **Lists All Templates** - Shows all templates in the system

## Running the Sample App

### Option 1: Using the run-test.sh script (Recommended)

From the repository root:

```bash
# Full test (starts everything, runs tests, opens browser)
./run-test.sh full

# Just run the sample app (requires API to be running)
./run-test.sh test

# Start services without running tests
./run-test.sh start

# Stop all services
./run-test.sh stop

# Show service status
./run-test.sh status
```

### Option 2: Manual execution

1. Make sure the API is running:
   ```bash
   cd src/FormR.API
   dotnet run
   ```

2. Run the sample app:
   ```bash
   cd tests/SampleApp
   dotnet run
   ```

## Expected Output

```
FormR Sample Application
========================

API Base URL: http://localhost:5000
Testing API connectivity...

Test 1: Fetching Control Library...
✓ Success! Found 21 control types
  Sample controls: TextInput, MultilineText, NumberInput, EmailInput, PhoneInput

Test 2: Creating a sample contact form template...
✓ Success! Created template with ID: <guid>
  Name: Contact Form
  Controls: 4

Test 3: Retrieving template <guid>...
✓ Success! Retrieved template: Contact Form
  Version: 1
  Controls: 4

Test 4: Listing all templates...
✓ Success! Found 1 template(s)
  - Contact Form (v1, 4 controls)

========================
All tests completed!
========================
```

## Customizing the Sample

You can modify `Program.cs` to:
- Create different template types
- Add more controls
- Test update and delete operations
- Experiment with different validation rules
- Test different control types from the library

## Troubleshooting

**Error: "Make sure the API is running"**
- Ensure PostgreSQL is running on port 5400
- Start the API with `dotnet run` in src/FormR.API
- Check that the API is accessible at http://localhost:5000

**Connection refused**
- Verify PostgreSQL is running: `pg_isready -h localhost -p 5400`
- Check the connection string in src/FormR.API/appsettings.json

**401 Unauthorized**
- The sample app uses a temporary tenant ID header for testing
- In production, you'd need proper authentication
