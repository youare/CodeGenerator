using CodeGenerator.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public class SagaGenerator : IGenerator
    {
        private readonly IConfigManager _configManager;

        public SagaGenerator(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task GenerateAsync()
        {
            var entityConfig = _configManager.AppConfig.EntityConfig;

            string str = $@"
import {"{"}take,call,put, select, takeLatest{"}"} from 'redux-saga/effects';
import Toastr from '../../services/toastrService/toastrService';
import UrlService from '../../services/urlService/urlService';
import {"{"}
    Get_{entityConfig.Name},
    Get_{entityConfig.Name}_Success,
    Get_{entityConfig.Name}_Complete,
    Create_{entityConfig.Name},
    Update_{entityConfig.Name},
    Delete_{entityConfig.Name},
{"}"} from './{entityConfig.Name.FromPascalToCamelCase()}Constant';

export function* get{entityConfig.Name}Saga(){"{"}
    yield takeLatest(Get_{entityConfig.Name}, function*(){"{"}
        const response = yield call(()=>UrlService.Get('{_configManager.AppConfig.ServiceName}','api/v1/{entityConfig.Name.FromPascalToCamelCase()}'));
        if(response.status == 200) {"{"}
            if(result.data && response.data.success == false) Toastr.All(response.data);
            else yield put({"{"} type:Get_{entityConfig.Name}_Success, data: result.data {"}"});
        {"}"}
        yield put({"{"} type: Get_{entityConfig.Name}_Complete {"}"});
    {"}"});
{"}"}

export function* create{entityConfig.Name}Saga(){"{"}
    yield takeLatest(Create_{entityConfig.Name}, function*(){"{"}
        const response = yield call(()=>UrlService.Post({_configManager.AppConfig.ServiceName}, 'api/v1/{entityConfig.Name.FromPascalToCamelCase()}'));
        if(response.status == 200) {"{"}
            if(result.data && response.data.success == false) Toastr.All(response.data);
            else yield put({"{"} type:Get_{entityConfig.Name}_Success, data: result.data {"}"});
        {"}"}
        yield put({"{"} type: Get_{entityConfig.Name}_Complete {"}"});
    {"}"});
{"}"}
            ";
            await File.WriteAllTextAsync($@"{await GetDestinationFolderAsync()}\{entityConfig.Name.FromPascalToCamelCase()}Saga.js", str);
        }

        public async Task<string> GetDestinationFolderAsync()
        {
            var dir = _configManager.AppConfig.JsRootFolder;
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
