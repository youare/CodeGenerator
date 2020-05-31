using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Bls.Generators
{
    public interface IGenerator
    {
        Task GenerateAsync();
        Task<string> GetDestinationFolderAsync();
    }
}
