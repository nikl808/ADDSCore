﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using ADDSCore.Service;
using ADDSCore.Command;
using ADDSCore.Model;

namespace ADDSCore.Dialog.ACSEditHwDialog
{
    public class ACSEditHwViewModel : DialogViewBaseModel<Hardware>
    {
        #region class fields
        public string ProtLev { get; set; }
        public string Climate { get; set; }
        public string Compos { get; set; }
        #endregion

        #region constructor
        public ACSEditHwViewModel(string title) : base(title)
        {
            CabinetComboItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem(){Id = 1, Item = "Нет"},
                new ComboBoxItem(){Id = 2, Item = "ШСА"},
                new ComboBoxItem(){Id = 3, Item = "ШАУ"},
                new ComboBoxItem(){Id = 4, Item = "ПМУ"},
            };

            SupVoltComboItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem(){Id = 1, Item = "Нет"},
                new ComboBoxItem(){Id = 2, Item = "220В"},
                new ComboBoxItem(){Id = 2, Item = "380В"}
            };

            ContrVoltComboItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem(){Id = 1, Item = "Нет"},
                new ComboBoxItem(){Id = 1, Item = "12В"},
                new ComboBoxItem(){Id = 1, Item = "24В"},
            };

            FreqComboItems = new ObservableCollection<ComboBoxItem>()
            {
                new ComboBoxItem(){Id = 1, Item = "Нет"},
                new ComboBoxItem(){Id = 1, Item = "50 Гц"},
                new ComboBoxItem(){Id = 1, Item = "60 Гц"},
            };

            CabinetSelectItem = CabinetComboItems[0];
            SupVoltSelectItem = SupVoltComboItems[0];
            ContrVoltSelectItem = ContrVoltComboItems[0];
            FreqSelectItem = FreqComboItems[0];
        }
        #endregion

        #region cabinet combobox properties
        private ObservableCollection<ComboBoxItem> cabinetComboItems;
        public ObservableCollection<ComboBoxItem> CabinetComboItems
        {
            get { return cabinetComboItems; }
            set { cabinetComboItems = value; }
        }

        private ComboBoxItem cabinetSelectItem;
        public ComboBoxItem CabinetSelectItem 
        {
            get { return cabinetSelectItem; }
            set { cabinetSelectItem = value; } 
        }
        #endregion

        #region supply voltage combobox properties
        private ObservableCollection<ComboBoxItem> supVoltComboItems;
        public ObservableCollection<ComboBoxItem> SupVoltComboItems
        {
            get { return supVoltComboItems; }
            set { supVoltComboItems = value; }
        }

        private ComboBoxItem supVoltSelectItem;
        public ComboBoxItem SupVoltSelectItem
        {
            get { return supVoltSelectItem; }
            set { supVoltSelectItem = value; }
        }
        #endregion

        #region control voltage combobox properties
        private ObservableCollection<ComboBoxItem> contrVoltComboItems;
        public ObservableCollection<ComboBoxItem> ContrVoltComboItems
        {
            get { return contrVoltComboItems; }
            set { contrVoltComboItems = value; }
        }

        private ComboBoxItem contrVoltSelectItem;
        public ComboBoxItem ContrVoltSelectItem
        {
            get { return contrVoltSelectItem; }
            set { contrVoltSelectItem = value; }
        }
        #endregion

        #region frequency combobox properties
        private ObservableCollection<ComboBoxItem> freqComboItems;
        public ObservableCollection<ComboBoxItem> FreqComboItems
        {
            get { return freqComboItems; }
            set { freqComboItems = value; }
        }

        private ComboBoxItem freqSelectItem;
        public ComboBoxItem FreqSelectItem
        {
            get { return freqSelectItem; }
            set { freqSelectItem = value; }
        }
        #endregion

        #region save button command
        private UICommand saveCommand;
        public UICommand SaveCommand
        {
            get
            {
                return saveCommand ??
                    (saveCommand = new UICommand(obj =>
                    {
                        CloseDialogWithResult(obj as IDialogWindow,
                            new Hardware
                            {
                                Cabinet = CabinetSelectItem.Item,
                                SuppVoltage = SupVoltSelectItem.Item,
                                ControlVoltage = ContrVoltSelectItem.Item,
                                MainFreq = FreqSelectItem.Item,
                                Protect = ProtLev,
                                Climate = Climate,
                                CabCompos = Compos
                            });
                    }));
            }
        }
        #endregion

        #region cancel button command
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