using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    public interface ICfgServo
    {
        double JogSpd { get; set; }
        short JogAcc { get; set; }
        short JogDec { get; set; }
        double InchSpd { get; set; }
        short InchAcc { get; set; }
        short InchDec { get; set; }
        double PosSpd { get; set; }
        short PosAcc { get; set; }
        short PosDec { get; set; }



    }
}
