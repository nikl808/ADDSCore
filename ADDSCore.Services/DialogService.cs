using Microsoft.Win32;
using ADDSCore.Services;
using System.Windows;
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

    public class DefaultDialogService : IDefaultDialogService
    {
        public string FilePath { get; set; }

        public bool ExportFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Документ Word (*.docx)|*.docx";
            if(saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        public bool ImportFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Документ Word (*.docx)|*.docx";
            if(openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message) => MessageBox.Show(message);

    }
}
