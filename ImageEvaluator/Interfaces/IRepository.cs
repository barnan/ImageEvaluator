using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    interface IRepository<T> where T : class
    {
        T GetDefaultInstance();

        T GetInstanceByName(string name);

        List<string> GetInstanceNames();

        bool StoreInstance(T instance, string name);

        bool DeleteInstance(T instance);

        bool Init();

        string RecipeDirectoryPath { get; }
    }
}
