using ADDSCore.Services;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ADDSCore.Services
{
    public class DialogService:IDialogService
    {
        public T OpenDialog<T>(DialogViewBaseModel<T> viewModel) where T:class
        {
            IDialogWindow window = new DialogWindow
            {
                DataContext = viewModel
            };
            window.ShowDialog();
            return viewModel.DialogResult;
        }
    }

    public class PrintDialogService:IPrintDialogService
    {
        public void OpenDialog(FlowDocument doc)
        {
            PrintDialog printDialog = new PrintDialog();
            
            if(printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource source = doc;
                printDialog.PrintDocument(source.DocumentPaginator,"Test printing");
            }
        }
    }
}
