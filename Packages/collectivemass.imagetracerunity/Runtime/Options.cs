using System;
using System.Linq;
using System.Reflection;
using CollectiveMass.ImageTracerUnity.OptionTypes;

namespace CollectiveMass.ImageTracerUnity
{
    [Serializable]
    public class Options
    {
        public Tracing tracing = new Tracing();
        public ColorQuantization colorQuantization = new ColorQuantization();
        public SvgRendering svgRendering = new SvgRendering();
        public Blur blur = new Blur();

        public void SetOptionByName(string optionName, object value)
        {
            var optionType = GetOptionTypeFromName(optionName);
            var property = optionType.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single(i => String.Equals(i.Name, optionName, StringComparison.CurrentCultureIgnoreCase));
            
            property.SetValue(optionType, Convert.ChangeType(value, property.PropertyType));
        }

        private object GetOptionTypeFromName(string optionName)
        {
            object[] options = {tracing, colorQuantization, svgRendering, blur};
            return options.Single(o => o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Any(i => String.Equals(i.Name, optionName, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}
