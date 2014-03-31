using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MassRegexFileRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cmbRenaming.Items.Add("Rename files");
            cmbRenaming.Items.Add("Rename folders");
            cmbRenaming.Items.Add("Rename files & folders");
            cmbRenaming.SelectedIndex = 0;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            bool renameFiles, renameFolders;
            switch (cmbRenaming.Text)
            {
                case "Rename files":
                    renameFiles = true;
                    renameFolders = false;
                    break;
                case "Rename folders":
                    renameFiles = false;
                    renameFolders = true;
                    break;
                case "Rename files & folders":
                    renameFiles = true;
                    renameFolders = true;
                    break;
                default:
                    throw new System.Exception("Invalid value in combobox");    // TODO: better exception?
            }
            if (chbRecursively.IsChecked.HasValue)
            {
                var fileRenames = RegexRenamer.Scan(txtFileLocation.Text, txtPattern.Text, txtRename.Text, (bool)chbRecursively.IsChecked, renameFiles, renameFolders);
                foreach (var fr in fileRenames)
                {
                    // TODO: display in datagrid
                }
            }
            else
            {
                throw new System.Exception("Checkbox neither checked nor unchecked.");    // TODO: better exception?
            }
        }

    }
}
