﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    enum ListState {ADDED, EXIST_CHANGE, EXIST_DELETE }
    class ACSQuestionListViewModel:INotifyPropertyChanged
    {
        //...
        //Добавить окно диалога сохранения
        //Добавить окно поиска по бд

        private IDialogService dialogService;

        //display list to datagridview 
        public BindingList<AutomaSysQuestnaire> QuestionLists { get; set; }

        private Dictionary<int, ListState> listChanges;
        
        //current selection
        private AutomaSysQuestnaire selectedList;
        public AutomaSysQuestnaire SelectedList
        {
            get { return selectedList; }
            set
            {
                selectedList = value;
                OnPropertyChanged("SelectedList");
            }
        }
        
        public ACSQuestionListViewModel()
        {
            dialogService = new DialogService();
            listChanges = new Dictionary<int, ListState>();

            //Connect to database
            using (DbConnection connection = new DbConnection())
            {
                //load entities into EF Core
                connection.db.AutomaQuestnaire.Load();

                //bind to the source
                QuestionLists = new BindingList<AutomaSysQuestnaire>(connection.db.AutomaQuestnaire.Local.ToList());
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
                        //Create dialog
                        var dialog = new ACSQuestEditMainViewModel("Новый опросный лист", null);
                        //Open dialog
                        var result = dialogService.OpenDialog(dialog);
                        
                        if (result != null)
                        {
                            QuestionLists.Insert(QuestionLists.Count,result);
                            SelectedList = result;
                            listChanges.Add(result.Id, ListState.ADDED);
                        }
                    }));
            }
        }

        //Save database
        private UICommand saveCommand;
        public UICommand SaveCommand
        {
            get 
            {
                return saveCommand ??
                    (saveCommand = new UICommand(obj =>
                    {
                       //open connection to database 
                       var context = new DbConnection();

                        foreach (var it in listChanges)
                        {
                            //add new entry
                            if (it.Value == ListState.ADDED)
                            {
                                if(context.db.AutomaQuestnaire.Find(it.Key)==null)
                                {
                                    context.db.AutomaQuestnaire.Add(QuestionLists[it.Key]);
                                    
                                    context.db.SaveChanges();
                                }
                            }
                            //update exist entry
                            else if(it.Value == ListState.EXIST_CHANGE)
                            {
                                var autoQuest = context.db.AutomaQuestnaire.Find(it.Key);
                                
                                if (autoQuest != null)
                                {
                                    autoQuest.ListName = QuestionLists[it.Key].ListName;
                                    autoQuest.ObjName = QuestionLists[it.Key - 1].ObjName;
                                    autoQuest.ControlAnalog = QuestionLists[it.Key - 1].ControlAnalog;
                                    autoQuest.ControlStruct = QuestionLists[it.Key - 1].ControlStruct;
                                    autoQuest.Network = QuestionLists[it.Key - 1].Network;
                                    autoQuest.Software = QuestionLists[it.Key - 1].Software;
                                    autoQuest.Document = QuestionLists[it.Key - 1].Document;
                                    autoQuest.Extra = QuestionLists[it.Key - 1].Extra;
                                    autoQuest.Cabinet = QuestionLists[it.Key - 1].Cabinet;
                                    autoQuest.Parameter = QuestionLists[it.Key - 1].Parameter;
                                    context.db.SaveChanges();
                                }
                            }
                            
                        }
                    }
                    )); 
            }
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
                    AutomaSysQuestnaire list = obj as AutomaSysQuestnaire;
                    var index = QuestionLists.IndexOf(SelectedList);
                    
                        if (list != null)
                        {
                            //Create dialog
                            var dialog = new ACSQuestEditMainViewModel("Редактирование: " + list.ListName,new AutomaSysQuestnaire(list));
                            //Open dialog
                            var result = dialogService.OpenDialog(dialog);
                            if (result != null)
                            {
                                QuestionLists[index] = result;
                                SelectedList = result;
                                if (!listChanges.ContainsKey(result.Id)) listChanges.Add(result.Id, ListState.EXIST_CHANGE);
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