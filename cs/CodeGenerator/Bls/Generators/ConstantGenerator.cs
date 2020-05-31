using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class ConstantGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public ConstantGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
export const Get_{entityConfig.Name} = '{_configManager.AppConfig.JsConstantPrefix}/Get_{entityConfig.Name}';
export const Get_{entityConfig.Name}_Success = '{_configManager.AppConfig.JsConstantPrefix}/Get_{entityConfig.Name}_Success';
export const Get_{entityConfig.Name}_Complete = '{_configManager.AppConfig.JsConstantPrefix}/Get_{entityConfig.Name}_Complete';

export const Create_{entityConfig.Name} = '{_configManager.AppConfig.JsConstantPrefix}/Create_{entityConfig.Name}';
export const Update_{entityConfig.Name} = '{_configManager.AppConfig.JsConstantPrefix}/Update_{entityConfig.Name}';
export const Delete_{entityConfig.Name} = '{_configManager.AppConfig.JsConstantPrefix}/Delete_{entityConfig.Name}';
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name.FromPascalToCamelCase()}Constant.js", str);
        }

        public async Task<string> GetDestinationFolderAsync()
        {
            var dir = _configManager.AppConfig.JsRootFolder;
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
