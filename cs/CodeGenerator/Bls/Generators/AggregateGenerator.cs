using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class AggregateGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;
        private static List<string> IgnoredProperties = new List<string> {
            "Id",
            "AuditTime",
            "AuditBy",
            "EventType"
        };
        public AggregateGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var aggConfig = _configManager.AppConfig.AggregateConfig;
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
using Common;
using Domain.Resume.DomainEvents;
using Domain.SharedKernel.DomainErrors;
using Domain.SharedKernel.DomainEvents;
using Domain.SharedKernel.Entities;
namespace Domain.{entityConfig.Name}
{"{"}
    public class {entityConfig.Name}Aggregate: Aggregate
    {"{"}
        private readonly List<{entityConfig.Name}ValueObject> _existing;
        public {entityConfig.Name}Aggregate(IList<{entityConfig.Name}ValueObject> existing)
        {"{"}
            _existing = existing;
        {"}"}
        public Either<DomainError, {entityConfig.Name}ValueObject> Add({string.Join(", ", entityConfig.EntityEntryList.Where(x=>!IgnoredProperties.Contains(x.Name)).Select(x=>x.Name))}, string user)
        {"{"}
            var item = new {entityConfig.Name}ValueObject(default({entityConfig.EntityEntryList.First().CSharpType}), {string.Join(", ", entityConfig.EntityEntryList.Where(x => !IgnoredProperties.Contains(x.Name)).Select(x => x.Name))}, DateTime.Now, user, SharedConstants.EventTypeInsert)
            _existing.Add(item);
            return item;
        {"}"}
        public Either<DomainError, {entityConfig.Name}ValueObject> Update({entityConfig.EntityEntryList.First().CSharpType} id, {string.Join(", ", entityConfig.EntityEntryList.Where(x => !IgnoredProperties.Contains(x.Name)).Select(x => x.Name))}, string user)
        {"{"}
            var matches - _existing.Where(x=>x.Id == id).ToList();
            if(matches.Count > 1) return new DuplicateItemDomainError<{entityConfig.EntityEntryList.First().CSharpType}>(nameof(id), id);
            if(matches.Count == 0) return new ItemNotFoundDomainError<{entityConfig.EntityEntryList.First().CSharpType}>(nameof(id), id);
            var match = matches.Single();
            var item = new {entityConfig.Name}ValueObject(match.Id, {string.Join(", ", entityConfig.EntityEntryList.Where(x => !IgnoredProperties.Contains(x.Name)).Select(x => $"match.{x.Name}"))}, DateTime.Now, user, SharedConstants.EventTypeUpdate)
            _existing.Remove(item);
            _existing.Add(item);
            return item;
        {"}"}
        public Either<DomainError, {entityConfig.Name}ValueObject> Delete({entityConfig.EntityEntryList.First().CSharpType} id, string user)
        {"{"}
            var matches - _existing.Where(x=>x.Id == id).ToList();
            if(matches.Count > 1) return new DuplicateItemDomainError<{entityConfig.EntityEntryList.First().CSharpType}>(nameof(id), id);
            if(matches.Count == 0) return new ItemNotFoundDomainError<{entityConfig.EntityEntryList.First().CSharpType}>(nameof(id), id);
            var match = matches.Single();
            _existing.Remove(item);
            return item;
        {"}"}
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name}Aggregate.cs", str);
        }
        public async Task<string> GetDestinationFolderAsync()
        {
            var dirs = Directory.GetDirectories(_configManager.AppConfig.ServerRootFolder, "*.*", SearchOption.AllDirectories);
            var dir = $"{(dirs.FirstOrDefault(x => x.Contains($"Domain.{_configManager.AppConfig.EntityConfig.Name}")) ?? _configManager.AppConfig.ServerRootFolder)}";
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
