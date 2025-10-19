using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMustacheRenderer
    {
        string Render(string template, object data);
    }
}
