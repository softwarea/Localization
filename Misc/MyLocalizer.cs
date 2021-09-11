using Localization.Resources;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Localization.Misc
{
    public class MyLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public MyLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString this[string key] => _localizer[key];       
    }
}