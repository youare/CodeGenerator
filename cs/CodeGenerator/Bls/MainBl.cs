using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Bls.Generators;
using CodeGenerator.Extensions;
namespace CodeGenerator.Bls
{
    public interface IMainBl
    {
        Task RunAsync();
    }
    public class MainBl: IMainBl
    {
        private readonly IEnumerable<IGenerator> _generators;

        public MainBl(IEnumerable<IGenerator> generators)
        {
            _generators = generators;
        }

        public async Task RunAsync()
        {
            foreach(var generator in _generators)
            {
                await generator.GenerateAsync();
            }
        }
    }
}
