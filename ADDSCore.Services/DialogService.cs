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
}
