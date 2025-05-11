using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Compoment
{
    public partial class PrintService : Component
    {
        public PrintService()
        {
            InitializeComponent();
        }

        public PrintService(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
