using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangTienLoi.Compoment
{
    public partial class NotificationManager : Component
    {
        public NotificationManager()
        {
            InitializeComponent();
        }

        public NotificationManager(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
