using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestordeTareas.UI.profiles
{
    public class AutoMapperRegistry
    {
        public static Type[] GetProfiles() => new[]
    {
        typeof(CategoriaProfile),
        // agrega todos los perfiles aqui
    };  
    }
}