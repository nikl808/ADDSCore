using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ADDSCore.Command;
using ADDSCore.Model;
using ADDSCore.Service;
using ADDSCore.Dialog.ACSEditDialog;
using ADDSCore.Utils;
using System.Windows.Documents;

namespace ADDSCore.View.ACSQuestionListPageView
{
    enum ListState { ADDED_FIRST, ADDED_CHANGE, ADDED_DELETE, EXIST_FIRST_CHANGE, EXIST_CHANGE, EXIST_DELETE }
    class ACSQuestionListViewModel:INotifyPropertyChanged
    {

        private IDialogService dialogService;

        //printDialog
        private IPrintDialogService printDialog;

        //display list to datagridview 
        public BindingList<AutomaSysQuestnaire> QuestionLists { get; set; }

        private Dictionary<int, ListState> actualChanges;
        private Dictionary<int, ListState> StateBeforeDelete;
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
            printDialog = new PrintDialogService();
            actualChanges = new Dictionary<int, ListState>();
            StateBeforeDelete = new Dictionary<int, ListState>();
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
                            actualChanges.Add(QuestionLists.Count-1, ListState.ADDED_FIRST);
                            
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
                       //open connection to database 
                       var context = new DbConnection();

                        foreach (var it in actualChanges)
                        {
                            //add new entry
                            if (it.Value == ListState.ADDED_FIRST || it.Value == ListState.ADDED_CHANGE)
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
                                var del = context.db.AutomaQuestnaire.Find(it.Key);
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
                                
                                if (QuestionLists.Count-1 < currPop.Item1) //undo last item
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
                                actualChanges.Add(currPop.Item1,currPop.Item3);
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

        //Find database entry
        private UICommand findCommand;
        public UICommand FindCommand
        {
            get
            {
                return findCommand ??
                    (findCommand = new UICommand(obj =>
                    {
                       
                    },
                    (obj) => QuestionLists.Count > 0));
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
                        if(list.Id != 0)
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

        //print command
        private UICommand printCommand;
        public UICommand PrintCommand
        {
            get
            {
                return printCommand ??
                   (printCommand = new UICommand(obj =>
                   {
                       FlowDocument doc = new FlowDocument();
                       //create oxml temp doc
                       //...
                       doc.LoadWordML("myDoc");
                       doc.Name = "testDoc";
                       printDialog.OpenDialog(doc);
                       //delete tmp doc
                       //...
                   },
                   (obj) => QuestionLists.Count > 0));
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
                   }, 
                   (obj) => QuestionLists.Count > 0));
            }
        }

        //Edit current selected item
        private UICommand editCommand;
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
                            var dialog = new ACSQuestEditMainViewModel("Редактирование: " + list.ListName,new AutomaSysQuestnaire(list));
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