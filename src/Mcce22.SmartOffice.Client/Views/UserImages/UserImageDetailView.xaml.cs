using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mcce22.SmartOffice.Client.ViewModels;

namespace Mcce22.SmartOffice.Client.Views
{
    /// <summary>
    /// Interaction logic for UserImageDetailView.xaml
    /// </summary>
    public partial class UserImageDetailView : UserControl
    {
        private string[] _allowedFileTypes = new []
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".tif"
        };

        public UserImageDetailView()
        {
            InitializeComponent();
        }

        private string GetFilePathFromDragData(DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            return files.FirstOrDefault();
        }

        private void OnFileDrop(object sender, DragEventArgs e)
        {
            var filePath = GetFilePathFromDragData(e);

            ((UserImageDetailViewModel)DataContext).FilePath = filePath;
            ((UserImageDetailViewModel)DataContext).HasFile = filePath != null;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            var filePath = GetFilePathFromDragData(e);

            if (!_allowedFileTypes.Contains(Path.GetExtension(filePath)))
            {
                DragDrop.DoDragDrop(this, e.Data, DragDropEffects.None);
            }

        }
    }
}
