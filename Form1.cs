using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDCardTool
{
    public partial class Form1 : Form
    {
        enum ScanType
        {
            Update,
            Add,
            Remove
        }

        enum PathType
        {
            SDCard,
            Source
        }

        private string _sdCardPath, _sourcePath;
        private List<string> _filesToAdd = new List<string>(), _filesToUpdate = new List<string>(), _filesToRemove = new List<string>();
        public Form1()
        {
            if (ValidatePath(Properties.Settings.Default.SDCardPath)) //We check if the path saved in the application settings is non null or empty, if so we set the SDCardPath to it, so we don't have to retype it each use.
                _sdCardPath = Properties.Settings.Default.SDCardPath;

            if (ValidatePath(Properties.Settings.Default.SourcePath))
                _sourcePath = Properties.Settings.Default.SourcePath;

            MaximizeBox = false;
            InitializeComponent();

            textBoxSDPath.Text = _sdCardPath; //We update the readonly textbox to the SDCardPath
            textBoxSourcePath.Text = _sourcePath;

            if(Properties.Settings.Default.ScanOptions >= 4)
            {
                checkBoxRemove.Checked = true;
                Properties.Settings.Default.ScanOptions -= 4;
            }

            if(Properties.Settings.Default.ScanOptions >= 2)
            {
                checkBoxAdd.Checked = true;
                Properties.Settings.Default.ScanOptions -= 2;
            }

            if(Properties.Settings.Default.ScanOptions == 1)
                checkBoxUpdate.Checked = true;

            treeViewChanges.StateImageList = new ImageList();
            treeViewChanges.StateImageList.Images.Add("folder_96", Properties.Resources.folder_96);
            treeViewChanges.StateImageList.Images.Add("file_96", Properties.Resources.file_96);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonSDCardSelect_Click(object sender, EventArgs e)
        {
            SelectPath(PathType.SDCard);
            
            /*FolderBrowserDialog dialog = new FolderBrowserDialog();

            if(!string.IsNullOrEmpty(_sdCardPath)) //We verify the SD card path is not null or empty.
                dialog.SelectedPath = _sdCardPath; //We set the selected SD card path to the previous one to make change of directories easier.

            if(dialog.ShowDialog() == DialogResult.OK) //We check the dialog result is ok and not cancel otherwise we ignore the dialog.
            {
                if(ValidatePath(dialog.SelectedPath)) //We make sure the user selected a valid existing path.
                {
                    _sdCardPath = dialog.SelectedPath; //If the path is valid we update the stored path.
                    textBoxSDPath.Text = dialog.SelectedPath; //We also need to update the textbox text to the new path.
                }
                else
                    MessageBox.Show("Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //We show an error dialog if the path is invalid or non existant.
            }*/
        }

        private void SelectPath(PathType pathType)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (ValidatePath(pathType == PathType.SDCard ? _sdCardPath : _sourcePath)) //We verify the path is not null or empty.
                dialog.SelectedPath = pathType == PathType.SDCard ? _sdCardPath : _sourcePath; //We set the path to the previous one to make change of directories easier.

            if (dialog.ShowDialog() == DialogResult.OK) //We check the dialog result is ok and not cancel otherwise we ignore the dialog.
            {
                if (ValidatePath(dialog.SelectedPath)) //We make sure the user selected a valid existing path.
                {
                    if(pathType == PathType.SDCard)
                    {
                        _sdCardPath = dialog.SelectedPath; //If the path is valid we update the stored path.
                        textBoxSDPath.Text = dialog.SelectedPath; //We also need to update the textbox text to the new path.
                    }
                    else
                    {
                        _sourcePath = dialog.SelectedPath; //If the path is valid we update the stored path.
                        textBoxSourcePath.Text = dialog.SelectedPath; //We also need to update the textbox text to the new path.
                    }
                }
                else
                    MessageBox.Show("Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //We show an error dialog if the path is invalid or non existant.
            }
        }

        private void buttonFolderSelect_Click(object sender, EventArgs e)
        {
            SelectPath(PathType.Source);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            if (!checkBoxAdd.Checked && !checkBoxRemove.Checked && !checkBoxUpdate.Checked) //Ensure that at least one options is selected, otherwise the scan would do nothing.
            {
                MessageBox.Show("Please select at least one scan options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //We show a error message if no scan options is chosen.
                return;
            }

            if (!ValidatePath(_sdCardPath) || !ValidatePath(_sourcePath))
            {
                MessageBox.Show("One of the path is invalid, please ensure every path is valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //We cancel the scan if there is any errors.
            }

            Properties.Settings.Default.ScanOptions = 0;

            if (checkBoxUpdate.Checked)
                Properties.Settings.Default.ScanOptions += 1;

            if (checkBoxAdd.Checked)
                Properties.Settings.Default.ScanOptions += 2;

            if (checkBoxRemove.Checked)
                Properties.Settings.Default.ScanOptions += 4;

            Properties.Settings.Default.SDCardPath = _sdCardPath; //We update the default SD card path in the applications settings to simplify next usage.
            Properties.Settings.Default.SourcePath = _sourcePath;

            Properties.Settings.Default.Save();

            treeViewChanges.Nodes.Clear();

            if (checkBoxAdd.Checked)
                _filesToAdd = Scan(ScanType.Add);
            if (checkBoxRemove.Checked)
                _filesToRemove = Scan(ScanType.Remove);
            if(checkBoxUpdate.Checked)
                _filesToUpdate = Scan(ScanType.Update);

            labelToUpdate.Text = $"{_filesToUpdate.Count} to update";
            labelToAdd.Text = $"{_filesToAdd.Count} to add";
            labelToRemove.Text = $"{_filesToRemove.Count} to remove";
        }

        private bool ValidatePath(string path) => !string.IsNullOrWhiteSpace(path) && path.Length > 0 && Directory.Exists(path); //Return true if the path is valid, it must be non null, non empty, have more than 0 characters and the path must exists.

        private List<string> Scan(ScanType scanType)
        {
            List<string> files = new List<string>();
            List<string> directories = new List<string>();

            switch (scanType)
            {
                case ScanType.Update:
                    {
                        var sourceFiles = Directory.EnumerateFiles(_sourcePath, "*", SearchOption.AllDirectories).Select(file => (GetReducedPath(file, PathType.Source), File.GetLastWriteTimeUtc(file)));

                        var sdCardFiles = Directory.EnumerateFiles(_sdCardPath, "*", SearchOption.AllDirectories).Select(file => (GetReducedPath(file, PathType.SDCard), File.GetLastWriteTimeUtc(file)));
                        var sdCardDirectories = Directory.EnumerateDirectories(_sdCardPath, "*", SearchOption.AllDirectories).Select(dir => GetReducedPath(dir, PathType.SDCard));

                        foreach (var dir in sdCardDirectories)
                        {
                            if (!NodesExist(dir))
                            {
                                if (!HasParent(dir))
                                    AddNode(dir, imageIndex: 0);
                                else
                                    AddChildrenNode(dir, imageIndex: 0);
                            }
                        }

                        foreach ((string, DateTime) sFileInfo in sourceFiles.Where(sFile => sdCardFiles.Any(tFile => tFile.Item1.Equals(sFile.Item1) && tFile.Item2 != sFile.Item2)))
                        {
                            AddChildrenNode(sFileInfo.Item1, Color.DarkBlue, 1);
                            files.Add(sFileInfo.Item1);
                            //File.SetLastWriteTimeUtc(GetFullPath(sFileInfo.Item1, PathType.SDCard), sFileInfo.Item2);
                        }

                        break;
                    }
                case ScanType.Add:
                    {
                        try
                        {
                            var sourceFiles = Directory.EnumerateFiles(_sourcePath, "*", SearchOption.AllDirectories);
                            var sourceDirectories = Directory.EnumerateDirectories(_sourcePath, "*", SearchOption.AllDirectories);

                            var sdCardFiles = Directory.EnumerateFiles(_sdCardPath, "*", SearchOption.AllDirectories);
                            var sdCardDirectories = Directory.EnumerateDirectories(_sdCardPath, "*", SearchOption.AllDirectories);

                            var missingDirs = sourceDirectories.Where(dirS => !sdCardDirectories.Select(dirT => dirT.Replace(_sdCardPath, "")).Contains(dirS.Replace(_sourcePath, "")));
                            var missingFiles = sourceFiles.Where(filS => !sdCardFiles.Select(filT => filT.Replace(_sdCardPath, "")).Contains(filS.Replace(_sourcePath, "")));

                            foreach (string dir in missingDirs.Select(mDir => GetReducedPath(mDir, PathType.Source)))
                            {
                                if (HasParent(dir))
                                    AddChildrenNode(dir, Color.DarkGreen, 0);
                                else
                                    AddNode(dir, foreColor: Color.DarkGreen, imageIndex: 0);

                                files.Add(dir);
                            }

                            foreach (string dir in sourceDirectories.Except(missingDirs).Select(sDir => GetReducedPath(sDir, PathType.Source)))
                            {
                                if (HasParent(dir))
                                    AddChildrenNode(dir, imageIndex: 0);
                                else
                                    AddNode(dir, imageIndex: 0);
                            }

                            foreach (string file in missingFiles.Select(mFile => GetReducedPath(mFile, PathType.Source)))
                            {
                                if (HasParent(file))
                                    AddChildrenNode(file, Color.DarkGreen, imageIndex: 1);
                                else
                                    AddNode(file, foreColor: Color.DarkGreen, imageIndex: 1);

                                files.Add(file);
                            }

                            foreach (string file in sourceFiles.Except(missingFiles).Select(sFile => GetReducedPath(sFile, PathType.Source)))
                            {
                                if (HasParent(file))
                                    AddChildrenNode(file, imageIndex: 1);
                                else
                                    AddNode(file, imageIndex: 1);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is UnauthorizedAccessException)
                                MessageBox.Show(e.Message + Environment.NewLine + "Scan results may be invalid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Console.WriteLine(e);
                        }

                        break;
                    }
                case ScanType.Remove:
                    {
                        try
                        {
                            var sourceFiles = Directory.EnumerateFiles(_sourcePath, "*", SearchOption.AllDirectories);
                            var sourceDirectories = Directory.EnumerateDirectories(_sourcePath, "*", SearchOption.AllDirectories);

                            var sdCardFiles = Directory.EnumerateFiles(_sdCardPath, "*", SearchOption.AllDirectories);
                            var sdCardDirectories = Directory.EnumerateDirectories(_sdCardPath, "*", SearchOption.AllDirectories);

                            var excessDirs = sdCardDirectories.Where(dirT => !sourceDirectories.Select(dirS => dirS.Replace(_sourcePath, "")).Contains(dirT.Replace(_sdCardPath, "")));
                            var excessFiles = sdCardFiles.Where(filT => !sourceFiles.Select(filS => filS.Replace(_sourcePath, "")).Contains(filT.Replace(_sdCardPath, "")));

                            foreach (string dir in excessDirs.Select(eDir => GetReducedPath(eDir, PathType.SDCard)))
                            {
                                if (HasParent(dir))
                                    AddChildrenNode(dir, Color.DarkRed, 0);
                                else
                                    AddNode(dir, foreColor: Color.DarkRed, imageIndex: 0);

                                files.Add(dir);
                            }

                            foreach (string dir in sdCardDirectories.Except(excessDirs).Select(tDir => GetReducedPath(tDir, PathType.SDCard)))
                            {
                                if (HasParent(dir))
                                    AddChildrenNode(dir, imageIndex: 0);
                                else
                                    AddNode(dir, imageIndex: 0);
                            }

                            foreach (string file in excessFiles.Select(eFile => GetReducedPath(eFile, PathType.SDCard)))
                            {
                                if (HasParent(file))
                                    AddChildrenNode(file, Color.DarkRed, 1);
                                else
                                    AddNode(file, foreColor: Color.DarkRed, imageIndex: 1);

                                files.Add(file);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is UnauthorizedAccessException)
                                MessageBox.Show(e.Message + Environment.NewLine + "Scan results may be invalid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Console.WriteLine(e);
                        }

                        break;
                    }
            }

            return files;
        }

        private void textBoxSDPath_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBoxSDPath.Text, textBoxSDPath.Font);

            textBoxSDPath.Width = size.Width;
        }

        private void textBoxSourcePath_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBoxSourcePath.Text, textBoxSourcePath.Font);

            textBoxSourcePath.Width = size.Width;
        }

        private string[] Update()
        {
            try
            {
                foreach (var item in Directory.EnumerateFiles(textBoxSourcePath.Text))
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception)
            {

                //throw;
            }

            return new string[0];
        }

        private void AddNode(string key, TreeNodeCollection collection = null, Color? foreColor = null, int imageIndex = -1)
        {
            foreColor = foreColor ?? Color.Empty;
            collection = collection ?? treeViewChanges.Nodes;

            if (NodesExist(key, collection))
            {
                TreeNode existingNode = collection.Find(key, true).First();

                if(existingNode.ForeColor == Color.Empty)
                {
                    existingNode.ForeColor = (Color)foreColor;
                    existingNode.StateImageIndex = imageIndex;
                }

                return;
            }

            TreeNode node = collection.Add(key, Path.GetFileName(key));
            node.ForeColor = (Color)foreColor;
            node.StateImageIndex = imageIndex;
        }

        private void AddChildrenNode(string childKey, Color? foreColor = null, int imageIndex = -1)
        {
            foreColor = foreColor ?? Color.Empty;

            if (NodesExist(Path.GetDirectoryName(childKey)) && !NodesExist(childKey))
                AddNode(childKey, treeViewChanges.Nodes.Find(Path.GetDirectoryName(childKey), true).First().Nodes, foreColor, imageIndex);
            else if (NodesExist(childKey) && foreColor != Color.Empty)
            {
                TreeNode node = treeViewChanges.Nodes.Find(childKey, true).First();

                node.ForeColor = (Color)foreColor;
                node.StateImageIndex = imageIndex;
            }
        }

        private bool HasParent(string path) => Path.GetDirectoryName(path).Length > 1;

        private bool NodesExist(string key) => treeViewChanges.Nodes.Find(key, true).Any();

        private bool NodesExist(string key, TreeNodeCollection collection) => collection.Find(key, true).Any();

        private string GetReducedPath(string path, PathType pathType) => path.Replace(pathType == PathType.Source ? _sourcePath : _sdCardPath, "");

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            if((_filesToUpdate.Count + _filesToAdd.Count + _filesToRemove.Count) <= 0)
            {
                MessageBox.Show("You need to run a scan before executing, or have at least one concerned file/folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"{_filesToUpdate.Count} file{(_filesToUpdate.Count > 1 ? "s" : "")} will be updated, {_filesToRemove.Count} file{(_filesToRemove.Count > 1 ? "s" : "")} will be removed, {_filesToAdd.Count} file{(_filesToAdd.Count > 1 ? "s" : "")} will be added, do you want to continue ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private string GetFullPath(string path, PathType pathType) => path.Insert(0, pathType == PathType.Source ? _sourcePath : _sdCardPath);
    }
}
