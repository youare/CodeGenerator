using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class QueryGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public QueryGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var afConfig = _configManager.AppConfig.QueryConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
using Cache;
using Data.Contracts.Daos;
using Data.Reader.Readers.Generic;
using Domain.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Query.Queries
{"{"}
    public interface I{entityConfig.Name}Query
    {"{"}
        Task<IList<{entityConfig.Name}ValueObject>> GetAllValueObjectsAsync();
        Task<IList<{entityConfig.Name}ReadDto>> GetAllAsync();
        Task<IDictionary<{entityConfig.EntityEntryList.First().CSharpType},{entityConfig.Name}ValueObject>> GetDictionaryAsync();
    {"}"}

    public class {entityConfig.Name}Query: I{entityConfig.Name}Query
    {"{"}
        private readonly I{_configManager.AppConfig.DaoConfig.ParentName.Replace("Base","")}Reader<{entityConfig.EntityEntryList.First().CSharpType}, {entityConfig.Name}> _{entityConfig.Name.FromPascalToCamelCase()}Reader;
        private readonly ICache _cache;
        public {entityConfig.Name}Query(I{_configManager.AppConfig.DaoConfig.ParentName.Replace("Base", "")}Reader<{entityConfig.EntityEntryList.First().CSharpType}, {entityConfig.Name}> {entityConfig.Name.FromPascalToCamelCase()}Reader, ICache cache)
        {"{"}
            _{entityConfig.Name.FromPascalToCamelCase()}Reader = {entityConfig.Name.FromPascalToCamelCase()}Reader;
            _cache = cache;
        {"}"}
        public async Task<IList<{entityConfig.Name}ValueObject>> GetAllValueObjectsAsync()
        {"{"}
            var data = (await _{entityConfig.Name.FromPascalToCamelCase()}Reader.GetAllAsync()).Select(x=>x.ToValueObject).ToList();
            return data;
        {"}"}
        public async Task<IList<{entityConfig.Name}ReadDto>> GetAllAsync()
        {"{"}
            return (await GetAllValueObjectsAsync()).Select({entityConfig.Name}ReadDto.MapFrom).ToList();
        {"}"}
        public async Task<IDictionary<{entityConfig.EntityEntryList.First().CSharpType},{entityConfig.Name}ValueObject>> GetDictionaryAsync()
        {"{"}
            return (await GetAllValueObjectsAsync()).GroupBy(x=>x.Id).ToDictionary(x=>x.key, v=>v.First());
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}Query.cs", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            var dir = $"{(dirs.FirstOrDefault(x => x.Contains("Queries")) ?? _configManager.AppConfig.ServerRootFolder)}";
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
