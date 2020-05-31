using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class ValueObjectGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public ValueObjectGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var voConfig = _configManager.AppConfig.ValueObjectConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
using Common;
using System;
using System.Collections.Generic;
namespace Domain.SharedKernel.ValueObjects
{"{"}
    public class {entityConfig.Name}ValueObject: ValueObject
    {"{"}
        {string.Join("\n        ", entityConfig.EntityEntryList.Select(x=>$"public {x.CSharpType} {x.Name} {"{ get; }"}"))}
        public {entityConfig.Name}ValueObject({string.Join(", ", entityConfig.EntityEntryList.Select(x => x.Name.FromPascalToCamelCase()))})
        {"{"}
            {string.Join("\n            ", entityConfig.EntityEntryList.Select(x => $"{x.Name} = {x.Name.FromPascalToCamelCase()};"))}
        {"}"}
        protected override IEnumerable<object> GetEqualityComponents()
        {"{"}
            {string.Join("\n            ", entityConfig.EntityEntryList.Select(x => $"yield return {x.Name};"))}
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}ValueObject.cs", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            var dir = $"{(dirs.FirstOrDefault(x => x.Contains("ValueObjects")) ?? _configManager.AppConfig.ServerRootFolder)}";
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
