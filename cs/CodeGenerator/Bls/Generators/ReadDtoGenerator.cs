using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class ReadDtoGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public ReadDtoGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var dtoConfig = _configManager.AppConfig.DtoConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
namespace {dtoConfig.NamespacePrefix}
{"{"}
    public class {entityConfig.Name}ReadDto
    {"{"}
        {string.Join("\n        ", entityConfig.EntityEntryList.Select(x => $"public {x.CSharpType} {x.Name} {"{ get; }"}"))}
        public {entityConfig.Name}ReadDto({string.Join(", ", entityConfig.EntityEntryList.Select(x => x.Name.FromPascalToCamelCase()))})
        {"{"}
            {string.Join("\n            ", entityConfig.EntityEntryList.Select(x => $"{x.Name} = {x.Name.FromPascalToCamelCase()};"))}
        {"}"}
        public static MapFrom({entityConfig.Name}ValueObject item)
        {"{"}
            return new {entityConfig.Name}ReadDto({string.Join(", ", entityConfig.EntityEntryList.Select(x=> $"item.{x.Name}"))});
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}ReadDto.cs", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            var dir = $"{(dirs.FirstOrDefault(x => x.Contains("Dtos")) ?? _configManager.AppConfig.ServerRootFolder)}\\{_configManager.AppConfig.EntityConfig.Name}";
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
