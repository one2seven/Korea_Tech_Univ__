using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    public interface ITeachServo
    {
        void JogMove(Dir dir);
        void JogStop();
        void InchMove(double dist);
    }
}
