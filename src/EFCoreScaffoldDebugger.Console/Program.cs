// See https://aka.ms/new-console-template for more information
using EFCoreScaffoldDebugger.Console;

using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EFCoreScaffoldDebugger.Console
{
    class Program
    {
        
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                .SetBasePath(new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).DirectoryName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddJsonFile($"appsettings.custom.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();
        static void Main(string[] args)
        {
            const string dbContextClassName = "testContext";
            const string outputContextDir = "Data";
            const string outputDir = "tmp/scaffolded";
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, recursive: true);

            using var ctx = new TestContext(Configuration.GetConnectionString("ReverseEngineer"));
            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddDbContextDesignTimeServices(ctx);
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var scaffolder = serviceProvider.GetRequiredService<IReverseEngineerScaffolder>();
            
            var scaffoldedModel = scaffolder.ScaffoldModel(
                Configuration.GetConnectionString("ReverseEngineer"),
                new DatabaseModelFactoryOptions(tables: new List<string>() { "AssetEntXref" }, schemas: new List<string>() { "dashboard" }),
                new ModelReverseEngineerOptions { UseDatabaseNames = true, NoPluralize = true },
                new ModelCodeGenerationOptions
                {
                    UseDataAnnotations = false,
                    RootNamespace = "Scaffolded",
                    ModelNamespace = "Scaffolded.Models",
                    ContextNamespace = "Scaffolded.Data",
                    Language = null,
                    ContextDir = MakeDirRelative(outputDir, outputContextDir),
                    ContextName = dbContextClassName,
                    SuppressOnConfiguring = false
                });

            scaffolder.Save(
                scaffoldedModel,
                outputDir: outputDir,
                overwriteFiles: true);
        }

        private static string MakeDirRelative(string outputDir, string outputContextDir)
        {
            return Path.Combine(outputDir, outputContextDir);
        }
    }
}