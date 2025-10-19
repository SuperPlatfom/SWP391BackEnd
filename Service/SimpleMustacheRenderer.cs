using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using Service.Interfaces;

namespace Service
{
    public class SimpleMustacheRenderer : IMustacheRenderer
    {
        public string Render(string template, object data)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var result = template;

            var sectionRegex = new Regex(@"{{\#(\w+)}}(.*?){{\/\1}}", RegexOptions.Singleline);
            result = sectionRegex.Replace(result, match =>
            {
                var groupName = match.Groups[1].Value;     
                var inner = match.Groups[2].Value;         
                var prop = data.GetType().GetProperty(groupName);
                if (prop == null) return ""; 

                var val = prop.GetValue(data);
                if (val is System.Collections.IEnumerable col && !(val is string))
                {
                    var acc = "";
                    foreach (var item in col)
                    {
                        acc += Render(inner, item); 
                    }
                    return acc;
                }
                return "";
            });

            var varRegex = new Regex(@"{{\s*(\w+)\s*}}");
            result = varRegex.Replace(result, match =>
            {
                var name = match.Groups[1].Value;
                var prop = data.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) return ""; 
                var value = prop.GetValue(data);
                return value?.ToString() ?? "";
            });

            return result;
        }
    }
}
