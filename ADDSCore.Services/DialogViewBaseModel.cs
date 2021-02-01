using System;
using System.Collections.Generic;
using System.Text;

namespace ADDSCore.Service
{
    public abstract class DialogViewBaseModel<T> where T:class
    {
        public string Title { get; set; }
        
        public T DialogResult { get; set; }

        public DialogViewBaseModel(string title = "")
        {
            Title = title;
        }

        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            DialogResult = result;
            if (dialog != null) dialog.DialogResult = true;
        }

    }
}
