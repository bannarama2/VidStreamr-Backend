var builder = WebApplication.CreateBuilder(args);

// Listen on all IPs (for LAN access)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Root test endpoint
app.MapGet("/", () => "Backend is running successfully");

// Get list of available videos
app.MapGet("/videos", () =>
{
    string vidDir = @"C:\ADD\YOUR\DIRECTORY\HERE"; //add your actual directory here
    string fileName;
    Dictionary<string, string> vidList = new Dictionary<string, string>();

    foreach (var file in Directory.EnumerateFiles(vidDir, "*.mp4")
    {
        fileName = Path.GetFileName(file);
        vidList.Add(fileName, file);
    }

    return vidList;
});

// Stream video file by name
app.MapGet("/videos/{filename}", (string filename) =>
{
    string vidDir = @"C:\ADD\YOUR\DIRECTORY\HERE";
    string filePath = Path.Combine(vidDir, filename);

    if (!System.IO.File.Exists(filePath))
    {
        return Results.NotFound();
    }

    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    return Results.File(stream, "video/mp4", filename, enableRangeProcessing: true);
});

app.Run();