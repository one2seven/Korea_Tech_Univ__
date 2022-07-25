using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigitalIO; 

namespace PC_Senior_Day12_MotionKit
{
    public partial class FormDIO : Form
    {
        IDIO _dio; 
        public FormDIO(IDIO dio)
        {
            InitializeComponent();
            _dio = dio;
            _dio.In[0] = sts.ON;
        }
    }
}
