using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class DbScriptGenerator: IGenerator
    {
        private readonly IConfigManager _configManager;

        public DbScriptGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var entityConfig = _configManager.AppConfig.EntityConfig;
            var lines = string.Join(",\n    ", entityConfig.EntityEntryList.Select(x => x.DbLine));
            var str = $@"
create table if not exists {entityConfig.Name.FromPascalToSnakeCase()} (
    {lines}
);
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\Script.Sql", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            return _configManager.AppConfig.ServerRootFolder;
        }
    }
}
