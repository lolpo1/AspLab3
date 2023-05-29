using AspDotNetLab3;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache().AddSession();
builder.Configuration.AddJsonFile("conf.json");
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), builder.Configuration.GetSection("LogFile").Value.ToString()));
var app = builder.Build();
app.UseSession();

app.MapGet("/", async (HttpContext context) => {
    app.Logger.LogInformation($"Time:{DateTime.Now.ToString()} - Path:{context.Request.Path} - IP:{context.Connection.RemoteIpAddress?.ToString()}");
});


app.MapGet("/{Controller}/{Action}/{id:int?}", (string Controller,string Action,int? id) => 
 $"Controller:{Controller}/Action:{Action}/{id}");
app.MapGet("/{Lang}/{Controller}/{Action}/{id:int?}", (string Lang, string Controller, string Action, int? id) =>
 $"Lang:{Lang}/Controller:{Controller}/Action:{Action}/{id}");

app.MapGet("/Session/Add/{Key}/{Value}", async (string Key, string Value,HttpContext context) =>
{
    if (context.Session.Keys.Contains(Key))
    {
        context.Session.SetString(Key, Value);
        await context.Response.WriteAsync("The Key exists. Was rewrited!");
    }
    else
    { 
        context.Session.SetString(Key, Value);
        await context.Response.WriteAsync("Session is updated with new value.");
    }
});

app.MapGet("/Session/View/{Key}", async (string Key,HttpContext context) =>
{
    if (context.Session.Keys.Contains(Key))
    {
        await context.Response.WriteAsync($"The Key exists. [{Key}]={context.Session.GetString(Key)}");
    }
    else
    {
        await context.Response.WriteAsync("The Key doesn't exist!");
    }
});


app.MapGet("/Cookie/Add/{Key}/{Value}", async (string Key, string Value, HttpContext context) =>
{
    if (context.Request.Cookies.Keys.Contains(Key))
    {
        context.Response.Cookies.Append(Key, Value);
        await context.Response.WriteAsync("The cookie exists. Was rewrited!");
    }
    else
    {
        context.Response.Cookies.Append(Key, Value);
        await context.Response.WriteAsync("Cookie is updated with new value.");
    }
});

app.MapGet("/Cookie/View/{Key}", async (string Key, HttpContext context) =>
{
    if (context.Request.Cookies.Keys.Contains(Key))
    {
        await context.Response.WriteAsync($"The cookie exists. [{Key}]={context.Request.Cookies[Key]}");
    }
    else
    {
        await context.Response.WriteAsync("The cookie doesn't exist!");
    }
});



app.Run();
