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

namespace ADDSCore.View.ACSQuestionListPageView
{
    /// <summary>
    /// Логика взаимодействия для ACSQuestionListPage.xaml
    /// </summary>
    public partial class ACSQuestionListPage : Page
    {
        public ACSQuestionListPage()
        {
            InitializeComponent();
            DataContext = new ACSQuestionListViewModel();
        }
    }
}
