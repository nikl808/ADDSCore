using System;
using System.Collections.Generic;
using System.Text;
using ADDSCore.Service;
using ADDSCore.Command;
using ADDSCore.Model;

namespace ADDSCore.Dialog.ACSEditParamDialog
{
    public class ACSEditParamViewModel:DialogViewBaseModel<Parameter>
    {
        public string ControlUnit { get; set; }
        public string ControlParams { get; set; }

        public ACSEditParamViewModel(string title) : base(title)
        {
            ControlUnit = "test";
            ControlParams = "test";
        }

        private UICommand saveCommand;
        public UICommand SaveCommand
        {
            get
            {
                return saveCommand ??
                    (saveCommand = new UICommand(obj =>
                    {
                        CloseDialogWithResult(obj as IDialogWindow,
                            new Parameter
                            { 
                                ControlUnit = ControlUnit,
                                ControlParams = ControlParams
                            });
                    }));
            }
        }

        private UICommand cancelCommand;
        public UICommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                    (cancelCommand = new UICommand(obj =>
                    {
                        CloseDialogWithResult(obj as IDialogWindow, null);
                    }));
            }
        }

    }
}
