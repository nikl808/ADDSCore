using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace ADDSCore.Services
{
    public interface IDialogService
    {
        public T OpenDialog<T>(DialogViewBaseModel<T> viewModel) where T : class;
    }

    public interface IPrintDialogService
    {
        public void OpenDialog(FlowDocument doc);
    }
}
