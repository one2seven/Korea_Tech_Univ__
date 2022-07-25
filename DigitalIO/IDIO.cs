using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalIO
{
    public interface IDIO
    {
        sts[] In { get; }
        sts[] Out { get; }
    }
}
