using System;
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
    enum ListState {ADDED, ADDED_CHANGE, ADDED_DELETE, EXIST_CHANGE, EXIST_DELETE }
    class ACSQuestionListViewModel:INotifyPropertyChanged
    {

        private IDialogService dialogService;

        //display list to datagridview 
        public BindingList<AutomaSysQuestnaire> QuestionLists { get; set; }

        private Dictionary<int, ListState> actualChanges;
        private Dictionary<int, AutomaSysQuestnaire> deletedObj;
        
        private Stack<Tuple<int, AutomaSysQuestnaire, ListState>> undoChanges;
        private Stack<Tuple<int, AutomaSysQuestnaire, ListState>> redoChanges;

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
            actualChanges = new Dictionary<int, ListState>();
            deletedObj = new Dictionary<int, AutomaSysQuestnaire>();
            undoChanges = new Stack<Tuple<int, AutomaSysQuestnaire, ListState>>();
            redoChanges = new Stack<Tuple<int, AutomaSysQuestnaire, ListState>>();

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
                            actualChanges.Add(QuestionLists.Count-1, ListState.ADDED);
                            
                            //push new list
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(QuestionLists.Count - 1, result, ListState.ADDED));
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

                        foreach (var it in actualChanges)
                        {
                            //add new entry
                            if (it.Value == ListState.ADDED || it.Value == ListState.ADDED_CHANGE)
                            {
                                context.db.AutomaQuestnaire.Add(QuestionLists[it.Key]);
                                context.db.SaveChanges();
                            }
                            //update exist entry
                            else if (it.Value == ListState.EXIST_CHANGE)
                            {
                                var find = QuestionLists[it.Key].Id;
                                var autoQuest = context.db.AutomaQuestnaire.Find(find);

                                if (autoQuest != null)
                                {
                                    autoQuest.ListName = QuestionLists[it.Key].ListName;
                                    autoQuest.ObjName = QuestionLists[it.Key].ObjName;
                                    autoQuest.ControlAnalog = QuestionLists[it.Key].ControlAnalog;
                                    autoQuest.ControlStruct = QuestionLists[it.Key].ControlStruct;
                                    autoQuest.Network = QuestionLists[it.Key].Network;
                                    autoQuest.Software = QuestionLists[it.Key].Software;
                                    autoQuest.Document = QuestionLists[it.Key].Document;
                                    autoQuest.Extra = QuestionLists[it.Key].Extra;
                                    autoQuest.Cabinet = QuestionLists[it.Key].Cabinet;
                                    autoQuest.Parameter = QuestionLists[it.Key].Parameter;
                                    context.db.SaveChanges();
                                }
                            }

                            else if (it.Value == ListState.EXIST_DELETE)
                            {
                                var del = context.db.AutomaQuestnaire.Find(deletedObj[it.Key].Id);
                                context.db.AutomaQuestnaire.Remove(del);
                                context.db.SaveChanges();
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

                        //push to redo stack
                        redoChanges.Push(currPop);

                        switch (currPop.Item3)
                        {
                            case ListState.ADDED:
                                QuestionLists.RemoveAt(currPop.Item1);
                                actualChanges.Remove(currPop.Item1);
                                break;
                            case ListState.ADDED_CHANGE:
                            case ListState.EXIST_CHANGE:
                                QuestionLists[currPop.Item1] = currPop.Item2;
                                break;
                            case ListState.ADDED_DELETE:
                                QuestionLists.Add(currPop.Item2);
                                actualChanges.Add(QuestionLists.Count - 1, ListState.ADDED);
                                break;
                            case ListState.EXIST_DELETE:
                                if(QuestionLists.Count-1 < currPop.Item1) //undo last item
                                    QuestionLists.Add(deletedObj[currPop.Item1]);
                                else //undo middle item
                                    QuestionLists.Insert(currPop.Item1, currPop.Item2);
                                deletedObj.Remove(currPop.Item1);
                                break;
                        }
                    },
                    (obj) => undoChanges.Count > 0));
            }
        }

        //redo one step
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
                        undoChanges.Push(currPop);

                        if (currPop.Item3 == ListState.ADDED_DELETE || currPop.Item3 == ListState.EXIST_DELETE)
                        {
                            QuestionLists.Insert(currPop.Item1,currPop.Item2);
                            actualChanges.Add(currPop.Item1, currPop.Item3 == ListState.ADDED_DELETE ? ListState.ADDED : ListState.EXIST_CHANGE); 
                        }
                    },
                    (obj) => redoChanges.Count > 0));
            }
        }

        //Find database entry
        private UICommand findCommand;
        public UICommand FindCommand
        {
            get
            {
                return findCommand ??
                    (findCommand = new UICommand(obj =>
                    {

                    }));
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
                    AutomaSysQuestnaire list = obj as AutomaSysQuestnaire;
                    var index = QuestionLists.IndexOf(SelectedList);

                    if (list != null) 
                    {
                        if(list.Id != 0)
                        {
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(list.Id-1, list, ListState.EXIST_DELETE));
                            deletedObj.Add(list.Id-1,list);
                            actualChanges.Add(list.Id - 1, ListState.EXIST_DELETE);
                            QuestionLists.RemoveAt(index);
                        }

                        else
                        {
                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(0, list, ListState.ADDED_DELETE));
                            QuestionLists.RemoveAt(index);
                            actualChanges.Remove(index);
                        }
                    }
                },
                (obj) => QuestionLists.Count > 0));
            }
        }

        //print command
        private UICommand printCommand;
        public UICommand PrintCommand
        {
            get
            {
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
                                if (actualChanges.Count > 0)
                                {
                                    //Check added/exist
                                    switch (actualChanges[index])
                                    {
                                        case ListState.ADDED:
                                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.ADDED_CHANGE)); 
                                            actualChanges[index] = ListState.ADDED_CHANGE;
                                            break;
                                        case ListState.ADDED_CHANGE:
                                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.ADDED_CHANGE)); 
                                            break;
                                        case ListState.EXIST_CHANGE:
                                            undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.EXIST_CHANGE));
                                            break;
                                    }
                                }

                                else //first change to an exist item
                                {
                                    undoChanges.Push(new Tuple<int, AutomaSysQuestnaire, ListState>(index, list, ListState.EXIST_CHANGE));
                                    actualChanges.Add(index, ListState.EXIST_CHANGE);
                                }

                                //Bind new values
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