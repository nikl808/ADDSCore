using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using ADDSCore.Command;
using ADDSCore.Model;
using ADDSCore.Service;
using ADDSCore.Dialog.ACSEditDialog;

namespace ADDSCore.View.ACSQuestionListPageView
{
    class ACSQuestionListViewModel:INotifyPropertyChanged
    {
        //...
        //Добавить окно диалога сохранения
        //Добавить окно поиска по бд

        private IDialogService dialogService;
        
        //display list to datagridview 
        public ObservableCollection<AutoConSysQuestList> QuestionLists { get; set; }

        //current selection
        private AutoConSysQuestList selectedList;
        public AutoConSysQuestList SelectedList
        {
            get
            {
                return selectedList;
            }
            set
            {
                selectedList = value;
                OnPropertyChanged("SelectedList");
            }
        }
        
        public ACSQuestionListViewModel()
        {
            dialogService = new DialogService();
            //Connect to database
            using (DbConnection connection = new DbConnection())
            {
                //load entities into EF Core
                connection.db.ACSQuestionList.Load();

                //bind to the source
                QuestionLists = new ObservableCollection<AutoConSysQuestList>(connection.db.ACSQuestionList.Local.ToObservableCollection());
            }
        }

        //new list command
        private UICommand addCommand;
        public UICommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new UICommand(obj =>
                    {
                        var connection = new DbConnection();
                        
                        AutoConSysQuestList list = new AutoConSysQuestList()
                        {
                            Name = "...",
                            ControlObject = "...",
                            ControlObjectAnalog = "...",
                            ControlStruct = "...",
                            Network = "...",
                            Software = "...",
                            Document = "..."
                        };

                        Hardware hw1 = new Hardware()
                        {
                            autoConSysQuestList = list,
                            Cabinet = "...",
                            SuppVoltage = "...",
                            ControlVoltage = "...",
                            MainFreq = "...",
                            Protect = "...",
                            Climate = "...",
                            CabCompos = "..."
                        };

                        Parameter param1 = new Parameter()
                        {
                            autoConSysQuestList = list,
                            ControlUnit = "...",
                            ControlParams = "..."
                        };

                        connection.db.ACSQuestionList.Add(list);
                        connection.db.hardware.Add(hw1);
                        connection.db.parameter.Add(param1);
                        connection.db.SaveChanges();

                        QuestionLists.Insert(QuestionLists.Count, list);

                        SelectedList = list;
                    }));
            }
        }

        //Save database
        private UICommand saveCommand;
        public UICommand SaveCommand
        {
            get { return saveCommand; }
        }

        //undo the last step
        private UICommand undoCommand;
        public UICommand UndoCommand
        {
            get
            {
                return undoCommand;
                //добавить условие доступности комманды (Если есть записи)
            }
        }

        //redo one step
        private UICommand redoCommand;
        public UICommand RedoCommand
        {
            get
            {
                return redoCommand;
            }
        }

        //Find database entry
        private UICommand findCommand;
        public UICommand FindCommand
        {
            get
            {
                return findCommand;
            }
        }

        //delete selected database entry
        private UICommand deleteCommand;
        public UICommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                (deleteCommand = new UICommand(obj =>
                {
                    AutoConSysQuestList list = obj as AutoConSysQuestList;
                    if (list != null)
                    {
                        //do smthg
                    }
                },
                (obj) => QuestionLists.Count > 0));
            }
        }

        //export entry to word and save
        private UICommand exportCommand;
        public UICommand ExportCommand
        {
            get
            {
                return exportCommand ??
                    (exportCommand = new UICommand(obj =>
                    {
                        AutoConSysQuestList list = obj as AutoConSysQuestList;
                        if (list != null)
                        {
                            //do smthg
                        }
                    },
                    (obj) => QuestionLists.Count > 0));
            }
        }

        private UICommand editCommand;
        public UICommand EditCommand
        {
            get
            {
                return editCommand ??
                    (editCommand = new UICommand(obj =>
                    {
                    AutoConSysQuestList list = obj as AutoConSysQuestList;
                    var index = QuestionLists.IndexOf(SelectedList);
                    
                        if (list != null)
                        {
                            //Create dialog
                            var dialog = new ACSQuestEditMainViewModel("Редактирование: " + list.Name,new AutoConSysQuestList(list));
                            //Open dialog
                            var result = dialogService.OpenDialog(dialog);
                            if (result != null)
                            {
                                QuestionLists[index] = result;
                                SelectedList = result;
                            }
                        }
                    },
                    (obj) => QuestionLists.Count > 0));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}