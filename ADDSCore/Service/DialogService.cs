using System;
using System.Collections.Generic;
using System.Text;

namespace ADDSCore.Service
{
    public class DialogService:IDialogService
    {
        public T OpenDialog<T>(DialogViewBaseModel<T> viewModel) where T:class
        {
            IDialogWindow window = new DialogWindow
            {
                DataContext = viewModel
            };
            window.ShowDialog();
            return viewModel.DialogResult;
        }
    }
}
