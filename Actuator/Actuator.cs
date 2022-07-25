using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    public abstract class Actuator
    {
        /// <summary>
        /// 설정 된 서보 모터 축 번호
        /// </summary>
        public readonly short _axNo;
        public Actuator(short axNo)
        {
            _axNo = axNo;
        }
    }
}
