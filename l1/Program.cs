
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Labotory work 1!");
app.Map("/IsLoginFree", IsLoginFree);
app.Map("/AllLogins", AllLogins);
app.Map("/AddLogin", AddLogin);

app.Run();
static bool IsFreeLogin(string login,string filePath)
{
    bool isFreeLogin = true;
    using (StreamReader fileObject = new StreamReader(filePath))
    {
        string s = fileObject.ReadLine()!;
        while (s != null)
        {
            if (s == login)
            {
                isFreeLogin = false;
            }
            s = fileObject.ReadLine()!;
        }
    }
    return isFreeLogin;
}
static string NewLogin(string login,string filePath)
{
    string text = "";
    if (IsFreeLogin(login,filePath))
    {
        using (StreamWriter fileObject = new StreamWriter(filePath, true))
        {
            fileObject.WriteLine(login);
        }
        text = string.Format("This login {0} is succesful added", login);
    }
    else
    {
        text = string.Format("This login {0} is already exist", login);
    }
    return text;
}
static List<string> Users(string filePath)
{
    List<string> users = new List<string>();
    using (StreamReader fileObject = new StreamReader(filePath))
    {
        string line = fileObject.ReadLine()!;
        while (line != null)
        {
            users.Add(line);
            line = fileObject.ReadLine()!;
        }
    }
    return users;
}
 static void IsLoginFree(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        string filePath = "/usr/src/users.txt";
        string paramLogin = context.Request.Query["login"]!;
        await context.Response.WriteAsync(JsonSerializer.Serialize(IsFreeLogin(paramLogin, filePath)));
    });
}
 static void AddLogin(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        string filePath = "/usr/src/users.txt";
        var request = context.Request.Body;
        string paramLogin;
        using (StreamReader reader = new StreamReader(request))
        {
            string body = await reader.ReadToEndAsync();
            paramLogin = JsonDocument.Parse(body).RootElement.GetProperty("login").ToString();

        }
        await context.Response.WriteAsync(JsonSerializer.Serialize(NewLogin(paramLogin, filePath)));
    });
}
 static void AllLogins(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        string filePath = "/usr/src/users.txt";
        await context.Response.WriteAsync(JsonSerializer.Serialize(Users(filePath)));
    });
}