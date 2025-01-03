using TextToAudioWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<HuggingFaceAPI>();

// Add the OpenAIAPI service.
builder.Services.AddSingleton<OpenAIAPI>(provider =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                   ?? builder.Configuration["OpenAI:ApiKey"];
    var apiEndpoint = "https://api.openai.com/v1/completions";
    return new OpenAIAPI(apiKey, apiEndpoint);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map the OpenAIAPI controller.
app.MapControllerRoute(
    name: "openai",
    pattern: "OpenAIAPI/{action}/{id?}");

app.Run();