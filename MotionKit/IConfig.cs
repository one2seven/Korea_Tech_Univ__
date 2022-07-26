using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motion; 

namespace MainControl
{
    public interface IConfig
    {
        double ZRSpd1 { get; set; }
        double ZRSpd2 { get; set; }
        double ZRSpd3 { get; set; }
        short ZRAcc1 { get; set; }
        short ZRAcc2 { get; set; }
        short ZRAcc3 { get; set; }
        short ZRDec1 { get; set; }
        short ZRDec2 { get; set; }
        short ZRDec3 { get; set; }
        double PosMovSpd { get; set; }
        short PosMovAcc { get ; set;}
        short PosMovDec { get; set; }
        ICfgServo[] CfgAxInfo { get; }
    }
}
