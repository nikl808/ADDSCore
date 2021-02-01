using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ADDSCore.Service;
using ADDSCore.ViewModels.Command;
using ADDSCore.Models.Business;


namespace ADDSCore.ViewModels
{
    public class AutomaQuestEditVM : DialogViewBaseModel<AutomaSysQuestnaire>
    {
        public AutomaSysQuestnaire currList { get; set; }
        public ObservableCollection<HwCabinet> Cabinets { get; set; }
        
        public ObservableCollection<ControlParameter> Parameters { get; set; }
        private IDialogService dialogService;
        public AutomaQuestEditVM(string title, AutomaSysQuestnaire list) : base(title)
        {
            if(list != null)
            {
                currList = list;
                Cabinets = new ObservableCollection<HwCabinet>(list.Cabinet);
                Parameters = new ObservableCollection<ControlParameter>(list.Parameter);
            }
            else
            {
                currList = new AutomaSysQuestnaire();
                Cabinets = new ObservableCollection<HwCabinet>();
                Parameters = new ObservableCollection<ControlParameter>();
            }
            
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
        

        //apply button command
        private UICommand applyCommand;
        public UICommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                    (applyCommand = new UICommand(obj =>
                {
                    currList.Cabinet = new List<HwCabinet>(Cabinets);
                    currList.Parameter = new List<ControlParameter>(Parameters);
                    currList.Network = new string(networkSelectItem.Item);
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
        private HwCabinet selectedCab;
        public HwCabinet SelectedCab
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
                        var dialog = new AutomaEditHwVM("Добавить шкаф");
                        var result = dialogService.OpenDialog(dialog);
                        if (result != null)
                        {
                            Cabinets.Insert(Cabinets.Count, result);
                        }
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
                        HwCabinet cab = obj as HwCabinet;
                        if (cab != null)
                            Cabinets.Remove(cab);
                    },
                    (obj) => Cabinets.Count > 0));
            }
        }
        #endregion

        #region parameter listview commands
        //select item
        private ControlParameter selectedParam;
        public ControlParameter SelectedParam
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
                        var dialog = new AutomaEditParamVM("Добавить параметр");
                        var result = dialogService.OpenDialog(dialog);
                        if (result != null)
                        {
                           Parameters.Insert(Parameters.Count, result);
                        }
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
                        ControlParameter param = obj as ControlParameter;
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