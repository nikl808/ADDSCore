using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADDSCore.ViewModels;

namespace ADDSCore.Views.Controls
{
    /// <summary>
    /// Interaction logic for QuestionListsPage.xaml
    /// </summary>
    public partial class QuestionListsPage : Page
    {
        public QuestionListsPage()
        {
            InitializeComponent();
            DataContext = new ACSQuestionListViewModel();
        }
    }
}
