using System;
using System.Collections.Generic;
using System.Text;

namespace ADDSCore.Service
{
    public interface IDialogWindow
    {
        bool? DialogResult { get; set; }
        object DataContext { get; set; }
        bool? ShowDialog();
    }
}
