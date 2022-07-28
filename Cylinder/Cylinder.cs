using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalIO;


namespace Motion
{
    public class Cylinder
    {
        DIO _IO;
        sts _on;

        public sts Sensor { get { return _on;} }
        public DIO DIO { get { return _IO; } }
        public Cylinder(DIO dio)
        {
            _IO = dio;        

        }
        /// <summary>
        /// 그리퍼 열기
        /// </summary>
        public void Open()
        {
            _IO.SetBit(0, sts.ON);
            _IO.SetBit(1, sts.OFF);
            _on = _IO.GetBit(0);


        }
        /// <summary>
        /// 그리퍼 닫기
        /// </summary>
        public void Close() 
        {
            _IO.SetBit(0, sts.OFF);
            _IO.SetBit(1, sts.ON);
            _on = _IO.GetBit(0);


        }
    }
}
