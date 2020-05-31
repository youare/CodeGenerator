using CodeGenerator.Bls;
using CodeGenerator.Bls.Generators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CodeGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfigManager, ConfigManager>()
                .AddScoped<IMainBl, MainBl>()
                .AddScoped<IGenerator, DbScriptGenerator>()
                .AddScoped<IGenerator, DaoGenerator>()
                .AddScoped<IGenerator, ValueObjectGenerator>()
                .AddScoped<IGenerator, ReadDtoGenerator>()
                .AddScoped<IGenerator, CreateDtoGenerator>()
                .AddScoped<IGenerator, UpdateDtoGenerator>()
                .AddScoped<IGenerator, DeleteDtoGenerator>()
                .AddScoped<IGenerator, AggregateGenerator>()
                .AddScoped<IGenerator, AggregateFactoryGenerator>()
                .AddScoped<IGenerator, QueryGenerator>()
                .AddScoped<IGenerator, ConstantGenerator>()
                .AddScoped<IGenerator,ReducerGenerator>()
                .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var mainBl = scope.ServiceProvider.GetService<IMainBl>();
            await mainBl.RunAsync();

        }
    }
}
