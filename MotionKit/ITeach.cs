using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motion;

namespace MainControl
{
    public interface ITeach
    {
        ITeachServo[] IAxes { get; }

        Point[] XY_Pt { get;  }
        int[] ZPt { get; }

        void XY_PosMove(int ptNo);
        void XY_PosSave();
        void Z_PosMove(int ptNo);
        void Z_PosSave(int ptNo);
        void PickupPlace(int startPtNo, int destPtNo);

        void Teaching_Save();

        //void Set_NSW_Limit(double pos);
        //void Set_PSW_Limit(double pos);

        bool Origin_Check { get;  }


    }
}
