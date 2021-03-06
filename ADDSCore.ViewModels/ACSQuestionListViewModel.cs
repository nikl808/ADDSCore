﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using ADDSCore.ViewModels.Command;
using ADDSCore.Models.Business;
using ADDSCore.Services;
using ADDSCore.ViewModels.Business.Enums;
using ADDSCore.DataProviders;


namespace ADDSCore.ViewModels
{
    public class ACSQuestionListViewModel : INotifyPropertyChanged
    {
        //display list to datagridview 
        public BindingList<AutomaSysQuestnaire> QuestionLists { get; set; }
        //generic dialog
        private IDialogService dialogService;
        //export dialog
        private IDefaultDialogService exportDialog;
        //provider to AutomaSysQuestnaire db
        private AutomaQuestnaireRepo AutomaRepo;

        private Dictionary<int, ListState> actualChanges;
        private Dictionary<int, ListState> StateBeforeDelete;
        private Stack<Tuple<int, AutomaSysQuestnaire, ListState>> undoChanges;
        private Stack<Tuple<int, AutomaSysQuestnaire, ListState>> redoChanges;

        public ACSQuestionListViewModel()
        {
            dialogService = new DialogService();
            exportDialog = new DefaultDialogService();
            actualChanges = new Dictionary<int, ListState>();
            StateBeforeDelete = new Dictionary<int, ListState>();
            undoChanges = new Stack<Tuple<int, AutomaSysQuestnaire, ListState>>();
            redoChanges = new Stack<Tuple<int, AutomaSysQuestnaire, ListState>>();
            AutomaRepo = new AutomaQuestnaireRepo();

            //bind to the source
            QuestionLists = AutomaRepo.GetEntitiesList();
        }

        #region Commands
        //datagrid line selection
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
                        var dialog = new AutomaQuestEditVM("Новый опросный лист", null);
                        //Open dialog
                        var result = dialogService.OpenDialog(dialog);

                        if (result != null)
                        {
                            QuestionLists.Insert(QuestionLists.Count, result);
                            SelectedList = result;
                            actualChanges.Add(QuestionLists.Count - 1, ListState.ADDED_FIRST);

                            //push new list
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(QuestionLists.Count - 1, result, ListState.ADDED_FIRST));
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
                        foreach (var it in actualChanges)
                        {
                            //add new entry
                            if (it.Value == ListState.ADDED_FIRST || it.Value == ListState.ADDED_CHANGE)
                            {
                                AutomaRepo.Create(QuestionLists[it.Key]);
                                AutomaRepo.Save();
                            }
                            //update exist entry
                            else if (it.Value == ListState.EXIST_CHANGE)
                            {
                                AutomaRepo.Update(QuestionLists[it.Key]);
                                AutomaRepo.Save();
                            }

                            else if (it.Value == ListState.EXIST_DELETE)
                            {
                                AutomaRepo.Delete(AutomaRepo.GetEntity(it.Key));
                                AutomaRepo.Save();
                            }
                        }
                        actualChanges.Clear();
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
                return undoCommand ??
                    (undoCommand = new UICommand(obj =>
                    {
                        //pop first value
                        var currPop = undoChanges.Pop();

                        switch (currPop.Item3)
                        {
                            case ListState.ADDED_FIRST:
                                redoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(currPop.Item1, QuestionLists[currPop.Item1], currPop.Item3));
                                QuestionLists.Remove(currPop.Item2);
                                actualChanges.Remove(currPop.Item1);
                                break;
                            case ListState.ADDED_CHANGE:
                            case ListState.EXIST_CHANGE:
                                redoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(currPop.Item1, QuestionLists[currPop.Item1], currPop.Item3));
                                QuestionLists[currPop.Item1] = currPop.Item2;
                                break;
                            case ListState.EXIST_FIRST_CHANGE:
                                redoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(currPop.Item1, QuestionLists[currPop.Item1], currPop.Item3));//return current list value
                                QuestionLists[currPop.Item1] = currPop.Item2;//return to origin
                                actualChanges.Remove(currPop.Item1);
                                break;
                            case ListState.ADDED_DELETE:
                                redoChanges.Push(currPop);
                                QuestionLists.Add(currPop.Item2);
                                actualChanges.Add(QuestionLists.Count - 1, StateBeforeDelete[currPop.Item1]);
                                break;
                            case ListState.EXIST_DELETE:
                                redoChanges.Push(currPop);

                                if (QuestionLists.Count - 1 < currPop.Item1) //undo last item
                                    QuestionLists.Add(currPop.Item2);
                                else //undo middle item
                                    QuestionLists.Insert(currPop.Item1, currPop.Item2);

                                //actualChanges
                                if (StateBeforeDelete.ContainsKey(currPop.Item1))
                                {
                                    actualChanges[currPop.Item1] = StateBeforeDelete[currPop.Item1];
                                    StateBeforeDelete.Remove(currPop.Item1);
                                }
                                else actualChanges.Remove(currPop.Item1);
                                break;
                        }
                        if (undoChanges.Count == 0) actualChanges.Clear();
                    },
                    (obj) => undoChanges.Count > 0));
            }
            
        }

        //redo the last step
        private UICommand redoCommand;
        public UICommand RedoCommand
        {
            get
            {
                return redoCommand ??
                    (redoCommand = new UICommand(obj =>
                    {
                        //pop first value
                        var currPop = redoChanges.Pop();

                        //push to undo stack
                        //undoChanges.Push(currPop);

                        switch (currPop.Item3)
                        {
                            case ListState.ADDED_FIRST:
                                undoChanges.Push(currPop);
                                QuestionLists.Add(currPop.Item2);
                                actualChanges.Add(currPop.Item1, currPop.Item3);
                                break;
                            case ListState.ADDED_CHANGE:
                            case ListState.EXIST_CHANGE:
                                undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(currPop.Item1, QuestionLists[currPop.Item1], currPop.Item3));
                                QuestionLists[currPop.Item1] = currPop.Item2;
                                break;
                            case ListState.EXIST_FIRST_CHANGE:
                                undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(currPop.Item1, QuestionLists[currPop.Item1], currPop.Item3));//return origin value
                                QuestionLists[currPop.Item1] = currPop.Item2;//return to last value
                                actualChanges.Add(currPop.Item1, ListState.EXIST_CHANGE);
                                break;
                            case ListState.ADDED_DELETE:
                                QuestionLists.Remove(currPop.Item2);
                                actualChanges.Remove(QuestionLists.Count - 1);
                                break;
                            case ListState.EXIST_DELETE:
                                undoChanges.Push(currPop);
                                StateBeforeDelete.Add(currPop.Item1, currPop.Item3);
                                if (QuestionLists.Count - 1 < currPop.Item1) //undo last item
                                    QuestionLists.RemoveAt(currPop.Item1);
                                else //undo middle item
                                    QuestionLists.RemoveAt(currPop.Item1);
                                actualChanges.Remove(currPop.Item1);
                                break;
                        }
                    },
                    (obj) => redoChanges.Count > 0));
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
                    //get selected list
                    AutomaSysQuestnaire list = obj as AutomaSysQuestnaire;
                    //get the index of an element in current BindingList
                    var index = QuestionLists.IndexOf(SelectedList);

                    if (list != null)
                    {
                        //if item exist
                        if (list.Id != 0)
                        {
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.EXIST_DELETE));
                            if (!actualChanges.ContainsKey(index)) actualChanges.Add(index, ListState.EXIST_DELETE);
                            else
                            {
                                StateBeforeDelete.Add(index, actualChanges[index]);
                                actualChanges[index] = ListState.EXIST_DELETE;
                            }
                            QuestionLists.RemoveAt(index);
                        }

                        //if new addition
                        else
                        {
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.ADDED_DELETE));
                            StateBeforeDelete.Add(index, actualChanges[index]);
                            QuestionLists.RemoveAt(index);
                            actualChanges.Remove(index);
                        }
                    }
                },
                (obj) => QuestionLists.Count > 0));
            }
        }

        //export command
        private UICommand exportCommand;
        public UICommand ExportCommand
        {
            get
            {
                return exportCommand ??
                   (exportCommand = new UICommand(obj =>
                   {
                       AutomaSysQuestnaire list = obj as AutomaSysQuestnaire;
                       if (list != null)
                       {
                           try
                           {
                               if(exportDialog.ExportFileDialog() == true)
                               {
                                   //create .docx document
                                   AutomaSysDocTemplate docx = new AutomaSysDocTemplate(exportDialog.FilePath);
                                   docx.CreatePackage(list);
                               }
                           }
                           catch (Exception ex)
                           {
                               exportDialog.ShowMessage(ex.Message);
                           }
                       }
                   },
                   (obj) => QuestionLists.Count > 0));
            }
        }
                
        //Edit current selected item
        private UICommand editCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public UICommand EditCommand
        {
            get
            {
                return editCommand ??
                    (editCommand = new UICommand(obj =>
                    {
                        //get selected list
                        AutomaSysQuestnaire list = obj as AutomaSysQuestnaire;
                        //get the index of an element in current BindingList
                        var index = QuestionLists.IndexOf(SelectedList);

                        if (list != null)
                        {
                            //Create edit dialog
                            var dialog = new AutomaQuestEditVM("Редактирование: " + list.ListName, new AutomaSysQuestnaire(list));
                            //Open dialog
                            var result = dialogService.OpenDialog(dialog);

                            //if edited
                            if (result != null)
                            {
                                //if first edit exist item
                                if (!actualChanges.ContainsKey(index))
                                {
                                    undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.EXIST_FIRST_CHANGE));//save old item
                                    actualChanges.Add(index, ListState.EXIST_CHANGE);
                                }
                                //if edit added item
                                else if (actualChanges[index].Equals(ListState.ADDED_FIRST))
                                    undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.ADDED_CHANGE));//save old item

                                //if second edit exist item
                                else undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.EXIST_CHANGE));

                                //Bind new values
                                QuestionLists[index] = result;
                                
                                SelectedList = result;
                            }
                        }
                    },(obj) => QuestionLists.Count > 0));
            }
        }
        #endregion

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}