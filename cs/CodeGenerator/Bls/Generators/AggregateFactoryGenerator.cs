using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class AggregateFactoryGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public AggregateFactoryGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var afConfig = _configManager.AppConfig.AggregateFactoryConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
using System;
using System.Threading.Tasks;
using Domain.Resume;
using Domain.SharedKernel.DomainEvents;

namespace BusinessLayer.Command.AggregateFactories
{"{"}
    public interface I{entityConfig.Name}AggregateFactory
    {"{"}
        Task<{entityConfig.Name}Aggregate> GetAggregatAsync();
    {"}"}

    public class {entityConfig.Name}AggregateFactory: I{entityConfig.Name}AggregateFactory
    {"{"}
        public async Task<{entityConfig.Name}Aggregate> GetAggregateAsync()
        {"{"}
            reutrn new Task<{entityConfig.Name}Aggregate>();
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}AggregateFactory.cs", str);
        }

        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            var dir = $"{(dirs.FirstOrDefault(x => x.Contains("AggregateFactories")) ?? _configManager.AppConfig.ServerRootFolder)}";
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
