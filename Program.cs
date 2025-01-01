using TextToAudioWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<HuggingFaceAPI>();

// Add the OpenAIAPI service.
builder.Services.AddSingleton<OpenAIAPI>(provider =>
{
    var apiKey = "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";
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