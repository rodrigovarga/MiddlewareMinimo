var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("Hello from middleware!\n");
    await next.Invoke();
});

app.UseWhen(context => context.Request.Query.ContainsKey("tag"), HandleTag);

app.Map("/map1/seg1", HandleMultiSeg);
app.Map("/map1", HandleMapTest1);
app.Map("/map2", HandleMapTest2);

app.MapWhen(context => context.Request.Query.ContainsKey("branch"), HandleBranch);

app.Map("/level1", level1App =>
{
    level1App.Map("/level2a", level2AApp =>
    {
        level2AApp.Run(async context =>
        {
            await context.Response.WriteAsync("Level 1 / Level 2 - Item A");
        });
    });
    level1App.Map("/level2b", level2BApp =>
    {
        level2BApp.Run(async context =>
        {
            await context.Response.WriteAsync("Level 1 / Level 2 - Item B");
        });
    });
    level1App.Run(async context =>
    {
        await context.Response.WriteAsync("Level 1 - Default Handler");
    });
});

app.Run(async context =>
{
    await context.Response.WriteAsync("Hello from non-Map delegate.");
});

app.Run();

static void HandleMapTest1(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 1");
    });
}

static void HandleMapTest2(IApplicationBuilder app)
{
    app.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("Hello from middleware Map Test 2!\n");
        await next.Invoke();
    });

    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 2");
    });
}

static void HandleMultiSeg(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 1 of Segment 1");
    });
}

static void HandleBranch(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        var branchVer = context.Request.Query["branch"];
        await context.Response.WriteAsync($"Branch used = {branchVer}");
    });
}

static void HandleTag(IApplicationBuilder app)
{
    app.Use(async (context, next) =>
    {
        var tagVer = context.Request.Query["tag"];
        await context.Response.WriteAsync($"Tag used = {tagVer}\n");
        await next.Invoke();
    });
}
