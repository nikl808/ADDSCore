using System;
using System.Collections.Generic;
using System.Text;

namespace ADDSCore.Service
{
    public interface IDialogService
    {
        public T OpenDialog<T>(DialogViewBaseModel<T> viewModel) where T : class;
    }
}
