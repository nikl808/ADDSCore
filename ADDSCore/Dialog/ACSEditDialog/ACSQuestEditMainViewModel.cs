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
        public ObservableCollection<Hardware> Cabinets { get; set; }
        public ObservableCollection<Parameter> Parameters { get; set; }
        private IDialogService dialogService;
        public ACSQuestEditMainViewModel(string title, AutoConSysQuestList list) : base(title)
        {
            currList = list;
            Cabinets = new ObservableCollection<Hardware>(list.ControlCab);
            Parameters = new ObservableCollection<Parameter>(list.Param);
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

        #region toolbar commands
        //print command
        private UICommand printCommand;
        public UICommand PrintCommand
        {
            get {
                return printCommand ??
                   (printCommand = new UICommand(obj =>
                   {
                       //implementation
                   }));
            }
        }

        //send email command
        private UICommand sendMessCommand;
        public UICommand SendMessCommand
        {
            get
            {
                return sendMessCommand ??
                   (sendMessCommand = new UICommand(obj =>
                   {
                       //implementation
                   }));
            }
        }

        //apply button command
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
        

        //cancel button command
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
        #endregion

        #region cabinet listview commands
        //select item
        private Hardware selectedCab;
        public Hardware SelectedCab
        {
            get { return selectedCab; }
            set { selectedCab = value; }
        }
        //add button command
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
                        if (result != null)
                            Cabinets.Insert(Cabinets.Count, result);
                    }));
            }
        }
        //remove button command
        private UICommand removeCabCommand;
        public UICommand RemoveCabCommand
        {
            get
            {
                return removeCabCommand ??
                    (removeCabCommand = new UICommand(obj =>
                    {
                        Hardware cab = obj as Hardware;
                        if (cab != null)
                            Cabinets.Remove(cab);
                    },
                    (obj) => Cabinets.Count > 0));
            }
        }
        #endregion

        #region parameter listview commands
        //select item
        private Parameter selectedParam;
        public Parameter SelectedParam
        {
            get { return selectedParam; }
            set { selectedParam = value; }
        }
        //add button command
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
                        if (result != null)
                            Parameters.Insert(Parameters.Count, result);
                    }
                    ));
            }
        }
        //remove button command
        private UICommand removeParamCommand;
        public UICommand RemoveParamCommand
        {
            get
            {
                return removeParamCommand ??
                    (removeParamCommand = new UICommand(obj =>
                    {
                        Parameter param = obj as Parameter;
                        if (param != null)
                            Parameters.Remove(param);
                    },
                    (obj) => Parameters.Count > 0));
            }
        }
        #endregion

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