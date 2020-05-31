using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class CreateDtoGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;
        private static List<string> IgnoredProperties = new List<string> { 
            "Id",
            "AuditTime",
            "AuditBy",
            "EventType"
        };
        public CreateDtoGenerator(IConfigManager configManager)
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
    public class {entityConfig.Name}CreateDto
    {"{"}
        {string.Join("\n        ", entityConfig.EntityEntryList.Where(x=> !IgnoredProperties.Contains(x.Name)).Select(x => $"public {x.CSharpType} {x.Name} {"{ get; set;}"}"))}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}CreateDto.cs", str);
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
