using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class ReducerGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public ReducerGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
import {"{"}
    Get_{entityConfig.Name},
    Get_{entityConfig.Name}_Success,
    Get_{entityConfig.Name}_Complete,
    Create_{entityConfig.Name},
    Update_{entityConfig.Name},
    Delete_{entityConfig.Name},
{"}"} from './{entityConfig.Name.FromPascalToCamelCase()}Constant';
import {"{"} fromJS {"}"} from 'immutable';
const initialState = fromJS({"{"}
    {entityConfig.Name.FromPascalToCamelCase()}:{"{"}
        data: [],
        isLoading: false
    {"}"},
{"}"});

export default function LandingReducer(state = initialState, action){"{"}
    switch(action.type){"{"}
        case Get_{entityConfig.Name}:{"{"}
            return state.setIn(['isLoading'], false);
        {"}"}
        case Get_{entityConfig.Name}_Success:{"{"}
            return state.setIn(['{entityConfig.Name.FromPascalToCamelCase()}'], action.data);
        {"}"}
        case Get_{entityConfig.Name}_Complete:{"{"}
            return state.setIn(['isLoading'], false);
        {"}"}
        case Create_{entityConfig.Name}:{"{"}
            return state.setIn(['isLoading'], false);
        {"}"}
        case Update_{entityConfig.Name}:{"{"}
            return state.setIn(['isLoading'], false);
        {"}"}
        case Delete_{entityConfig.Name}:{"{"}
            return state.setIn(['isLoading'], false);
        {"}"}
        default:
            return state;
    {"}"}
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name.FromPascalToCamelCase()}Reducer.js", str);
        }

        public async Task<string> GetDestinationFolderAsync()
        {
            var dir = _configManager.AppConfig.JsRootFolder;
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
