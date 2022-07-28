using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMCWHPNET;

namespace DigitalIO
{
    public class DIO : IDIO
    {

        sts[] _In, _Out;

        public sts[] In { get => _In; }
        public sts[] Out { get => _Out; }

        public DIO()
        {
            _In = new sts[32];
            _Out = new sts[32];
        }

        public void SetBit(short bitNo, sts val)
        {
            switch (val)
            {
                case sts.OFF:
                    MMCLib.set_option_bit(bitNo);
                    break;
                case sts.ON:
                    MMCLib.reset_option_bit(bitNo);
                    break;
            }
        }
        public void SetPort(int value)
        {
            MMCLib.set_option_io(0, value);
        }
        public void BitAllOff()
        {
            SetPort(0);
        }
        public void BitAllOn()
        {
            SetPort(-1);
        }

        public void Update()
        {
            int inBuf = 0, outBuf = 0, bitMast = 1;
            MMCLib.get_option_io(0, ref inBuf);
            MMCLib.get_option_out_io(0, ref outBuf);
            inBuf = ~inBuf;
            outBuf = ~outBuf;
            for (int bitNo = 0; bitNo < In.Length; bitNo++)
            {
                _In[bitNo] = (inBuf & (bitMast << bitNo)) != 0 ? sts.ON : sts.OFF;
                _Out[bitNo] = (outBuf & (bitMast << bitNo)) != 0 ? sts.ON : sts.OFF;
            }
        }
        public sts GetBit(int bitNo)
        {
            int inBuf = 0, bitMast = 1;
            MMCLib.get_option_io(0, ref inBuf);
            inBuf = ~inBuf;
            inBuf = (inBuf & (bitMast << bitNo)) != 0 ? 1 : 0;

            return inBuf == 0? sts.OFF: sts.ON; 


        }
        public sts GetOutBit(int bitNo)
        {
            int buf = 0, bitMast = 1;
            MMCLib.get_option_out_io(0, ref buf);
            buf = ~buf;
            buf = (buf & (bitMast << bitNo)) != 0 ? 1 : 0;

            return buf == 0 ? sts.OFF : sts.ON;

        }




    }
    public enum sts
    {
        OFF, ON
    }
}
