using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ADDSCore.Service;
using ADDSCore.Command;
using ADDSCore.Model;
using ADDSCore.Dialog.ACSEditHwDialog;
using ADDSCore.Dialog.ACSEditParamDialog;

namespace ADDSCore.Dialog.ACSEditDialog
{
    public class ACSQuestEditMainViewModel : DialogViewBaseModel<AutoConSysQuestList>
    {
        public AutoConSysQuestList currList { get; set; }
        private IDialogService dialogService;
        public ACSQuestEditMainViewModel(string title, AutoConSysQuestList list) : base(title)
        {
            currList = list;
            dialogService = new DialogService();

            NetworkComboItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem(){Id = 1, Item = "Нет"},
                new ComboBoxItem(){Id = 2, Item = "Modbus"},
                new ComboBoxItem(){Id = 3, Item = "PROFINET"},
                new ComboBoxItem(){Id = 4, Item = "EtherCAT"},
            };
            NetworkSelectItem = NetworkComboItems[0];
        }

        #region network combobox properties
        private ObservableCollection<ComboBoxItem> networkComboItems;
        public ObservableCollection<ComboBoxItem> NetworkComboItems
        {
            get { return networkComboItems; }
            set { networkComboItems = value; }
        }

        private ComboBoxItem networkSelectItem;
        public ComboBoxItem NetworkSelectItem
        {
            get { return networkSelectItem; }
            set { networkSelectItem = value; }
        }
        #endregion

        private UICommand applyCommand;
        public UICommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                    (applyCommand = new UICommand(obj =>
                {
                    CloseDialogWithResult(obj as IDialogWindow, currList);
                }
                ));
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
                }
                ));
            }
        }

        private UICommand addCabCommand;
        public UICommand AddCabCommand
        {
            get
            {
                return addCabCommand ??
                    (addCabCommand = new UICommand(obj =>
                    {
                        var dialog = new ACSEditHwViewModel("Добавить шкаф");
                        var result = dialogService.OpenDialog(dialog);
                    }));
            }
        }

        private UICommand removeCabCommand;
        public UICommand RemoveCabCommand
        {
            get
            {
                return removeCabCommand ??
                    (removeCabCommand = new UICommand(obj =>
                    {
                        //do smth
                    }));
            }
        }

        private UICommand addParamCommand;
        public UICommand AddParamCommand
        {
            get
            {
                return addParamCommand ??
                    (addParamCommand = new UICommand(obj =>
                    {
                        var dialog = new ACSEditParamViewModel("Добавить параметр");
                        var result = dialogService.OpenDialog(dialog);
                    }
                    ));
            }
        }

        private UICommand removeParamCommand;
        public UICommand RemoveParamCommand
        {
            get
            {
                return removeParamCommand ??
                    (removeParamCommand = new UICommand(obj =>
                    {
                        //do smth
                    }));
            }
        }
        #region combobox item class
        public class ComboBoxItem
        {
            private int id;
            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            private string item;
            public string Item
            {
                get { return item; }
                set { item = value; }
            }
        }
        #endregion
    }
}
