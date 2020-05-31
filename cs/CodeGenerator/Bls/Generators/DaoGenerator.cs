using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class DaoGenerator: IGenerator
    {
        private readonly IConfigManager _configManager;

        public DaoGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var daoConfig = _configManager.AppConfig.DaoConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            var str = $@"
using Domain.SharedKernel.ValueObjects;
using System;
namespace Data.Contracts.Daos
{"{"}
    public class {entityConfig.Name} : {daoConfig.ParentName}(
    {"{"}
        public {entityConfig.Name}({string.Join(", ",entityConfig.EntityEntryList.Select(x=>x.Name.FromPascalToCamelCase()))})
        {"{"}
            {string.Join("\n            ", entityConfig.EntityEntryList.Select(x => $"{x.Name} = {x.Name.FromPascalToCamelCase()};"))}
        {"}"}
        private {entityConfig.Name}(){"{}"}
        public {entityConfig.Name}ValueObject ToValueObject()
        {"{"}
            return new {entityConfig.Name}ValueObject({string.Join(", ", entityConfig.EntityEntryList.Select(x => x.Name))});
        {"}"}
        public static {entityConfig.Name} MapFrom({entityConfig.Name}ValueObject item)
        {"{"}
            return new {entityConfig.Name}({string.Join(", ", entityConfig.EntityEntryList.Select(x => $"item.{x.Name}"))});
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}Dao.cs", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            return dirs.FirstOrDefault(x=>x.Contains("Daos")) ?? _configManager.AppConfig.ServerRootFolder;
        }
    }
}
