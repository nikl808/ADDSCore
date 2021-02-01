using ADDSCore.Service;
using ADDSCore.ViewModels.Command;
using ADDSCore.Models.Business;

namespace ADDSCore.ViewModels
{
    public class AutomaEditParamVM:DialogViewBaseModel<ControlParameter>
    {
        public string ControlUnit { get; set; }
        public string ControlParams { get; set; }

        public AutomaEditParamVM(string title) : base(title)
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
                            new ControlParameter
                            { 
                                ControlHwName = ControlUnit,
                                Parameter = ControlParams
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
