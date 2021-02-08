using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace ADDSCore.Services
{
    public interface IDialogService
    {
        T OpenDialog<T>(DialogViewBaseModel<T> viewModel) where T : class;
    }

    public interface IDefaultDialogService
    {
        string FilePath { get; set; }
        bool ExportFileDialog();
        bool ImportFileDialog();
        void ShowMessage(string message);
    }
}
