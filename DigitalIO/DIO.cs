using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalIO
{
    public class DIO : IDIO
    {
        sts[] _in, _out;
        public DIO()
        {
            _in = new sts[32];
            _out = new sts[32];
        }
        public sts[] In { get { return _in; } }
        public sts[] Out { get { return _out; } }

        public void TestMethod()
        {

        }
    }
    public enum sts
    {
        OFF, ON
    }
}
