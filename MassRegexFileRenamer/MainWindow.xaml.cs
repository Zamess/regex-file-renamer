﻿using System;
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
        private const string files = "Rename files";
        private const string folders = "Rename folders";
        private const string filesAndFolders = "Rename files & folders";
        

        public MainWindow()
        {
            InitializeComponent();

            cmbRenaming.Items.Add(files);
            cmbRenaming.Items.Add(folders);
            cmbRenaming.Items.Add(filesAndFolders);
            cmbRenaming.SelectedIndex = 0;
            
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            bool renameFiles, renameFolders;
            switch (cmbRenaming.Text)
            {
                case files:
                    renameFiles = true;
                    renameFolders = false;
                    break;
                case folders:
                    renameFiles = false;
                    renameFolders = true;
                    break;
                case filesAndFolders:
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
                    MessageBox.Show(fr.OldName + " -> " + fr.NewName);
                    // TODO: display in datagrid
                }
            }
            else
            {
                throw new System.Exception("Checkbox neither checked nor unchecked.");    // TODO: better exception?
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var browse = new System.Windows.Forms.FolderBrowserDialog();
            browse.ShowNewFolderButton = false;
            browse.SelectedPath = "C:\\";
            var result = browse.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtFileLocation.Text = browse.SelectedPath;
            }
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            //TODO Execute
        }
    }
}
