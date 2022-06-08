using GQL.Workshop.App;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => 
        webBuilder.UseStartup<Startup>())
    .Build()
    .Run();