using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SmartCopy
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Used to specify if a path is a Source path or a Target path.
        /// </summary>
        enum PathType
        {
            Target,
            Source
        }

        /// <summary>
        /// Used to specify what ScanOptions are enabled.
        /// </summary>
        [Flags] //By using the Flags attribute, ScanOptions can have multiples values associated to it.
        enum ScanOptions
        {
            //Assign binary value to each value of the enum otherwise the Flags attribute wouldn't work.
            Update = 1,
            Add = 2,
            Remove = 4
        }

        /// <summary>
        /// Used to store part of the files and folders returned by a scan.
        /// </summary>
        struct ScanResult
        {
            //The paths of the folders and files.
            public List<string> folders;
            public List<string> files;
            //

            /// <summary>
            /// Return the combined count of the folders and files.
            /// </summary>
            public int Count => folders.Count + files.Count;

            /// <summary>
            /// Clear all the folders and files paths.
            /// </summary>
            public void Clear()
            {
                folders.Clear();
                files.Clear();
            }
        }

        private string _targetPath, _sourcePath; //Used to keep track of the Source and Target path.
        private (ScanResult addResults, ScanResult removeResults, ScanResult updateResults) _scanResults = (new ScanResult() { files = new List<string>(), folders = new List<string>() }, new ScanResult() { files = new List<string>(), folders = new List<string>() }, new ScanResult() { files = new List<string>(), folders = new List<string>() }); //Used to keep track of the files and folders to be updated, added and removed.

        public Form1()
        {
            //Verify that the saved path is valid, empty, null or non existant path are ignored.
            if (IsValidPath(Properties.Settings.Default.TargetPath))
                _targetPath = Properties.Settings.Default.TargetPath;

            if (IsValidPath(Properties.Settings.Default.SourcePath))
                _sourcePath = Properties.Settings.Default.SourcePath;
            //

            MaximizeBox = false; //Disable the posibility of maximizing the form otherwise the ui would become a mess.
            InitializeComponent();

            //Set the textBoxes text to their Path values.
            textBoxTargetPath.Text = _targetPath;
            textBoxSourcePath.Text = _sourcePath;
            //

            ScanOptions options = (ScanOptions)Properties.Settings.Default.ScanOptions; //Convert the stored byte value, ScanOptions, to the enum type ScanOptions.

            //Set the check state of all the ScanOptions CheckBox depending on if they're in the options value or not.
            checkBoxUpdate.Checked = options.HasFlag(ScanOptions.Update);
            checkBoxAdd.Checked = options.HasFlag(ScanOptions.Add);
            checkBoxRemove.Checked = options.HasFlag(ScanOptions.Remove);
            //

            treeViewChanges.StateImageList = new ImageList(); //Create and assign a new ImageList to the treeViewChanges StateImageList.

            //Add the icons stored inside the Resources to the treeViewChanges StateImageList.
            treeViewChanges.StateImageList.Images.Add("folder_96", Properties.Resources.folder_96);
            treeViewChanges.StateImageList.Images.Add("file_96", Properties.Resources.file_96);
            //
        }

        /// <summary>
        /// Determine if a path is a valid Directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if the path is, not null, not empty and is a Directory.</returns>
        private bool IsValidPath(string path) => !string.IsNullOrWhiteSpace(path) && path.Length > 0 && Directory.Exists(path);

        /// <summary>
        /// Determine if a node with a specified key exist inside treeViewChanges nodes.
        /// </summary>
        /// <param name="key">The key of the searched node.</param>
        /// <returns>True if a node with the key exist in treeViewChanges nodes.</returns>
        private bool NodesExist(string key) => treeViewChanges.Nodes.Find(key, true).Any();

        /// <summary>
        /// Determine if a node with a specified key exist inside a TreeNodeCollection.
        /// </summary>
        /// <param name="key">The key of the searched node.</param>
        /// <param name="collection">The collection to search the node in.</param>
        /// <returns>True if a node with the key exist in the TreeNodeCollection.</returns>
        private bool NodesExist(string key, TreeNodeCollection collection) => collection.Find(key, true).Any();

        /// <summary>
        /// Remove the Source or Target path from a full path.
        /// </summary>
        /// <param name="fullPath">The full path to be reduced.</param>
        /// <param name="pathType">The type of path to be removed.</param>
        /// <returns>A reduced path made by removing the pathType path from the full path.</returns>
        private string GetReducedPath(string fullPath, PathType pathType) => fullPath.Replace(pathType == PathType.Source ? _sourcePath : _targetPath, "");

        /// <summary>
        /// Add the Source or Target path to a reduced path.
        /// </summary>
        /// <param name="reducedPath">The reduced path to be transformed.</param>
        /// <param name="pathType">The type of path to be added.</param>
        /// <returns>A full path made by adding the pathType path to the reduced path.</returns>
        private string GetFullPath(string reducedPath, PathType pathType) => reducedPath.Insert(0, pathType == PathType.Source ? _sourcePath : _targetPath);

        /// <summary>
        /// Return the total amount of files and folders returned from the scan.
        /// </summary>
        private int ChangeCount => _scanResults.addResults.Count + _scanResults.removeResults.Count + _scanResults.updateResults.Count;

        /// <summary>
        /// Get all directories inside a directory.
        /// </summary>
        /// <param name="path">The path of the directory to retrieves all directories from.</param>
        /// <returns>All the directories inside the specified directory except System directories.</returns>
        private List<string> GetFolders(string path)
        {
            List<string> dirs = Directory.EnumerateDirectories(path).ToList(); //Get all the top level directories from the path.

            dirs = dirs.Where(d => new DirectoryInfo(d) is var info && !info.Attributes.HasFlag(FileAttributes.System)).ToList(); //Remove all the system directories, e.g. "System Volume Information" that are typically blocked or require administration perms.

            //Try catch to handle any possible exceptions when trying a fast enumeration.
            try
            {
                //Loop through each, non system, top directories.
                foreach (var dir in dirs.ToArray()) //.ToArray() is needed otherwise if the collection get modified inside the loop it will throw an exception.
                {
                    dirs.AddRange(Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories)); //Get all the directories and subdirectories in the folder.
                }
                //
            }
            catch (Exception) //Fast enumeration threw an exception, typically because of the lack of permissions for a folder.
            {
                int lastCount = -1; //LastCount of directories in the collection, default is -1 because the count of a collection cannot be negative.

                while (lastCount != dirs.Count()) //Keep looping if the count is different than before, new dirs were added.
                {
                    lastCount = dirs.Count(); //Update the lastCount before updating the collection.

                    dirs = GetFolders(dirs); //Get folders safely without throwing exceptions.
                }
            }
            //

            return dirs;
        }

        /// <summary>
        /// Add to a list of directories all the top directories inside the directories of the list.
        /// </summary>
        /// <param name="dirs">The list containing the paths of the directories to add all the top directories from.</param>
        /// <returns>A list containing the previous given directories and the top directories inside of those except System directories.</returns>
        private List<string> GetFolders(List<string> dirs)
        {
            List<string> newDirs = new List<string>(); //Use a new list to not modify the dirs param.

            foreach (var dir in dirs.ToArray()) //.ToArray() is needed otherwise if the collection get modified inside the loop it will throw an exception.
            {
                //Try catch to detect any directory the user cannot access.
                try
                {
                    newDirs.AddRange(Directory.EnumerateDirectories(dir)); //Add the top directories inside the current dir to newDirs.
                }
                catch (Exception) //Cannot enumerate top directories, typically because the user lacks of permissions for the folder.
                {
                    dirs.Remove(dir); //Remove the dir inside of dirs because the user most likely lack permissions for this folder.
                    continue;
                }
                //
            }

            newDirs = newDirs.Where(d => new DirectoryInfo(d) is var info && !info.Attributes.HasFlag(FileAttributes.System)).ToList(); //Remove all the system directories, e.g. "System Volume Information" that are typically blocked or require administration perms.

            //Add all newDirs while avoiding duplicates.
            foreach (var newDir in newDirs)
            {
                if (!dirs.Contains(newDir)) //Ensure dirs don't already has the directory.
                    dirs.Add(newDir);
            }
            //

            return dirs;
        }

        /// <summary>
        /// Get all files insides a list of directories.
        /// </summary>
        /// <param name="dirs">The list containing the paths of the directories.</param>
        /// <returns>A list of paths for all the files inside the given directories.</returns>
        public List<string> GetFiles(List<string> dirs)
        {
            List<string> files = new List<string>();

            foreach (var dir in dirs)
                files.AddRange(Directory.EnumerateFiles(dir)); //Add top files only to avoid searching in subdirectories without permission.

            return files;
        }

        /// <summary>
        /// Open a FolderBrowserDialog and set the Source or Target path value to the dialog path.
        /// </summary>
        /// <param name="pathType">Determine which path will be set.</param>
        private void SelectPath(PathType pathType)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            //If the current path is valid set the path of the dialog to it.
            if (IsValidPath(pathType == PathType.Target ? _targetPath : _sourcePath))
                dialog.SelectedPath = pathType == PathType.Target ? _targetPath : _sourcePath;
            //

            if (dialog.ShowDialog() == DialogResult.OK) //If the result is not OK ignore the Dialog.
            {
                if (IsValidPath(dialog.SelectedPath)) //Ensure the path is a valid Directory.
                {
                    if (pathType == PathType.Target)
                    {
                        //Update the TargetPath and the Target TextBox.
                        _targetPath = dialog.SelectedPath;
                        textBoxTargetPath.Text = dialog.SelectedPath;
                        //
                    }
                    else
                    {
                        //Same logic for the Source.
                        _sourcePath = dialog.SelectedPath;
                        textBoxSourcePath.Text = dialog.SelectedPath;
                        //
                    }
                }
                else
                    MessageBox.Show(this, "Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //Show an error dialog if the path is not valid.
            }
        }

        /// <summary>
        /// Scan two folders and return the differents files and folders based on the enabled ScanOptions checkBoxes.
        /// </summary>
        /// <returns>A Tuple containing 3 ScanResults, add, remove and update.</returns>
        private (ScanResult addResults, ScanResult removeResults, ScanResult updateResults) Scan()
        {
            ScanResult addResults = new ScanResult() { folders = new List<string>(), files = new List<string>() }, removeResults = new ScanResult() { folders = new List<string>(), files = new List<string>() }, updateResults = new ScanResult() { folders = new List<string>(), files = new List<string>() }; //Initialize the list of each ScanResults.

            //Get and stores all the directories in the Source and Target path.
            List<string> sourceDirectories = GetFolders(_sourcePath).ToList();
            List<string> targetDirectories = GetFolders(_targetPath).ToList();
            //

            //Add the Source and Target directory to their list otherwise the top files inside them won't be included in the Scan.
            sourceDirectories.Add(_sourcePath);
            targetDirectories.Add(_targetPath);
            //

            //Create a list of tuples containing each files reducedPath, lastWriteTime and info for both the Source and the Target.
            IEnumerable<(string reducedPath, DateTime lastWriteTimeUtc, FileInfo fileInfo)> sourceFiles = GetFiles(sourceDirectories).Select(file => (GetReducedPath(file, PathType.Source), File.GetLastWriteTimeUtc(file), new FileInfo(file)));
            IEnumerable<(string reducedPath, DateTime lastWriteTimeUtc, FileInfo fileInfo)> targetFiles = GetFiles(targetDirectories).Select(file => (GetReducedPath(file, PathType.Target), File.GetLastWriteTimeUtc(file), new FileInfo(file)));
            //

            //Remove the Source and Target directory from the list since the files were scanned and they're no longer needed.
            sourceDirectories.Remove(_sourcePath);
            targetDirectories.Remove(_targetPath);
            //

            //Transform the Source and Target directories path into reducedPath.
            sourceDirectories = sourceDirectories.Select(dir => GetReducedPath(dir, PathType.Source)).ToList();
            targetDirectories = targetDirectories.Select(dir => GetReducedPath(dir, PathType.Target)).ToList();
            //

            copyProgressBar.Value = copyProgressBar.Minimum; //Set the value of the Progress Bar to his minimum (0).
            copyProgressBar.Maximum = sourceFiles.Count() + targetFiles.Count() + sourceDirectories.Count + targetDirectories.Count; //Set the maximum of the Progress Bar to the count of files and folders to scan.

            #region Files

            foreach (var sFile in sourceFiles)
            {
                if (!File.Exists(GetFullPath(sFile.reducedPath, PathType.Target)) && checkBoxAdd.Checked) //If the file doesn't exist in the Target and the add option is enabled add the file to the addResults.
                    addResults.files.Add(sFile.reducedPath);
                else if (File.Exists(GetFullPath(sFile.reducedPath, PathType.Target)) && ((File.GetLastWriteTimeUtc(GetFullPath(sFile.reducedPath, PathType.Target)) != sFile.lastWriteTimeUtc) || (new FileInfo(GetFullPath(sFile.reducedPath, PathType.Target)).Length != sFile.fileInfo.Length)) && checkBoxUpdate.Checked) //If the file exist in the Target, the LastWriteTime or file size isn't equal and the update option is enabled add the file to the updateResults.
                    updateResults.files.Add(sFile.reducedPath);

                copyProgressBar.Value++; //Increment the Progress Bar value each time a file is scanned.
            }

            if (checkBoxRemove.Checked)
            {
                foreach (var tFile in targetFiles.Select(tFile => tFile.reducedPath).Except(sourceFiles.Select(sFile => sFile.reducedPath)))
                    removeResults.files.Add(tFile);
            }

            copyProgressBar.Value += targetFiles.Count(); //Add to the Progress Bar value the count of targetFiles after scanning them.

            #endregion
            #region Folders

            if (checkBoxAdd.Checked) //Scan Source directories if the add option is enabled.
            {
                foreach (var sDir in sourceDirectories)
                {
                    if (!Directory.Exists(GetFullPath(sDir, PathType.Target))) //If the Target doesn't have the same directory add it to the addResults.
                        addResults.folders.Add(sDir);

                    copyProgressBar.Value++; //Increment the Progress Bar value each time a folder is scanned.
                }
            }
            else
                copyProgressBar.Value += sourceDirectories.Count; //If the add option is disabled add the count of Source directories to the Progress Bar value.

            if (checkBoxRemove.Checked) //Scan Target directories if the remove option is enabled.
            {
                foreach (var tDir in targetDirectories.Except(sourceDirectories)) //Loop through every targetDirectories Except the ones that are also in sourceDirectories.
                    removeResults.folders.Add(tDir);
            }

            copyProgressBar.Value += targetDirectories.Count; //Add the number of targetDirectories to the value of the Progress Bar after scanning all of the Target directories.

            #endregion

            return (addResults, removeResults, updateResults);
        }

        /// <summary>
        /// Refresh the text value for the update, add and remove files/folders count labels.
        /// </summary>
        private void UpdateCountLabels()
        {
            //Update the text based on the scanResults.
            labelToUpdate.Text = $"{_scanResults.updateResults.files.Count} to update";
            labelToAdd.Text = $"{_scanResults.addResults.files.Count + _scanResults.addResults.folders.Count} to add";
            labelToRemove.Text = $"{_scanResults.removeResults.files.Count + _scanResults.removeResults.folders.Count} to remove";
            //
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            if (!checkBoxAdd.Checked && !checkBoxRemove.Checked && !checkBoxUpdate.Checked) //Verify at least one scan option is selected, if not show an error message.
            {
                MessageBox.Show("Please select at least one scan options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //Show an error message.
                return; //Cancel the scan.
            }

            if (!IsValidPath(_targetPath) || !IsValidPath(_sourcePath)) //Verify the choosen path still exists.
            {
                MessageBox.Show("One of the path is invalid, please ensure every path is valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //Show an error message.
                return; //Cancel the scan.
            }

            Properties.Settings.Default.ScanOptions = 0; //Set the saved scanOptions to 0 to avoid overflow.

            if (checkBoxUpdate.Checked)
                Properties.Settings.Default.ScanOptions += (int)ScanOptions.Update; //If the checkbox update is checked, add value of ScanOptions.Update to the saved ScanOptions.

            if (checkBoxAdd.Checked)
                Properties.Settings.Default.ScanOptions += (int)ScanOptions.Add; //If the checkbox add is checked, add value of ScanOptions.Add to the saved ScanOptions.

            if (checkBoxRemove.Checked)
                Properties.Settings.Default.ScanOptions += (int)ScanOptions.Remove; //If the checkbox remove is checked, add value of ScanOptions.Remove to the saved ScanOptions.

            //Disable the checkBoxes so the user can't change the options during a scan or an execution.
            checkBoxAdd.Enabled = false;
            checkBoxRemove.Enabled = false;
            checkBoxUpdate.Enabled = false;
            //

            Properties.Settings.Default.TargetPath = _targetPath; //Update the default Target path in the applications settings to simplify next usage.
            Properties.Settings.Default.SourcePath = _sourcePath; //Update the default Source path in the applications settings to simplify next usage.

            Properties.Settings.Default.Save(); //Save the settings.

            treeViewChanges.Nodes.Clear(); //Remove all nodes from previous scan.

            _scanResults = Scan(); //Start the scan and save the results.

            //Add a node with DarkGreen color for each folders to add.
            foreach (var folder in _scanResults.addResults.folders)
                AddNode(folder, true, Color.DarkGreen);
            //

            //Add a node with DarkRed color for each folders to remove.
            foreach (var folder in _scanResults.removeResults.folders)
                AddNode(folder, true, Color.DarkRed);
            //

            //Add a node with DarkGreen color for each files to add.
            foreach (var file in _scanResults.addResults.files)
                AddNode(file, false, Color.DarkGreen);
            //

            //Add a node with DarkRed color for each files to remove.
            foreach (var file in _scanResults.removeResults.files)
                AddNode(file, false, Color.DarkRed);
            //

            //Add a node with DarkBlue color for each files to update.
            foreach (var file in _scanResults.updateResults.files)
                AddNode(file, false, Color.DarkBlue);
            //

            UpdateCountLabels(); //Update the count labels with the new scan results.

            //Enable the checkBoxes so the user can change the options for the next scan.
            checkBoxAdd.Enabled = true;
            checkBoxRemove.Enabled = true;
            checkBoxUpdate.Enabled = true;
            //

            MessageBox.Show(this, "Scan done.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); //Inform the user that the scan is done using a MessageBox.

            copyProgressBar.Value = copyProgressBar.Minimum; //Set the value of the Progress Bar to his minimum (0).
        }

        /// <summary>
        /// Add a node to represent a folder or a file inside a FileSystem.
        /// </summary>
        /// <param name="reducedPath">The reducedPath of the folder or file.</param>
        /// <param name="isFolder">Specify if the reducedPath is a folder or a file.</param>
        /// <param name="foreColor">Specify the ForeColor of the new node, null for default.</param>
        /// <param name="collection">Specify the collection to add the node to, null for default.</param>
        /// <returns>The new created node.</returns>
        private TreeNode AddNode(string reducedPath, bool isFolder, Color? foreColor = null, TreeNodeCollection collection = null)
        {
            if (reducedPath is null || string.IsNullOrWhiteSpace(reducedPath) || reducedPath.Length < 2) //Cancel the operation if the reducedPath is invalid.
                return null;

            if (reducedPath[0] == Path.DirectorySeparatorChar)
                reducedPath = reducedPath.Remove(0, 1); //If there's a '\' at the start of the path remove it.

            foreColor = foreColor ?? Color.Empty; //If the foreColor is null set it to Color.Empty otherwise do not change the value.

            var parentsNames = reducedPath.Split(Path.DirectorySeparatorChar).Reverse().ToArray(); //Get all the parent folders names of the file/folder.
            List<string> parentsPaths = new List<string>();

            if (parentsNames.Length <= 1 || collection != null) //If there's no parent or if a collection is specified.
            {
                if (!(NodesExist(parentsNames[0]) && collection is null)) //Ensure that either the collection is null or that a node with the same key already exist.
                {
                    collection = collection ?? treeViewChanges.Nodes; //If the collection is null set it to the treeViewChanges nodes collection otherwise do not change the value.

                    TreeNode node = collection.Add(reducedPath, parentsNames[0]); //Add the node.
                    node.ForeColor = (Color)foreColor; //Set the ForeColor.
                    node.StateImageIndex = isFolder ? 0 : 1; //Set the image, file or folder.

                    return node;
                }
            }
            else
            {
                //Finds all the parents path in other words the parents directories.
                string currentPath = reducedPath;

                foreach (var parent in parentsNames)
                {
                    parentsPaths.Add(currentPath);

                    if (currentPath.Length != parent.Length)
                        currentPath = currentPath.Remove(currentPath.Length - (parent.Length + 1), parent.Length + 1);
                    else
                        parentsPaths.Reverse();
                }
                //

                TreeNodeCollection currCollection = treeViewChanges.Nodes; //Keep tracks of in which collection the node should be added in.

                foreach (var path in parentsPaths) //Loop through every parents path in a reversed order (see lines above).
                {
                    bool isLast = parentsPaths[parentsPaths.Count - 1] == path;

                    if (NodesExist(path, currCollection))
                        currCollection = currCollection.Find(path, true).First().Nodes; //If the parent directory already exist set the current collection to his.
                    else if (AddNode(path, isLast ? isFolder : true, isLast ? foreColor : Color.Empty, currCollection) is TreeNode node) //Create the new node and ensure it was created (non null).
                        currCollection = node.Nodes; //If the new node was created set the current collection to his.
                }
            }

            return null;
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            if (ChangeCount <= 0) //If there's no file/folder to change show an error and cancel the execution.
            {
                MessageBox.Show(this, "You need to run a scan before executing, or have at least one concerned file/folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show(this, $"{_scanResults.removeResults.Count + _scanResults.updateResults.Count} file(s)/folder(s) will be modified and {_scanResults.addResults.Count} file(s)/folder(s) will be added, do you wish to continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question); //Ask the user for confirmation.

            if (result != DialogResult.Yes) //Cancel the execution if the dialog gets closed or the user choose No.
                return;

            List<string> failed = new List<string>(); //Keeps tracks of the failed files, typically because it was used by another process or lack of permission.

            copyProgressBar.Value = copyProgressBar.Minimum; //Reset the Progress Bar value.
            copyProgressBar.Maximum = ChangeCount; //Set the Progress Bar max value to the amount of changes.

            foreach (var dir in _scanResults.addResults.folders) //Loop through each folders to add (the order is important since new files can be inside those folders).
            {
                string fullPath = GetFullPath(dir, PathType.Target); //Convert the reduced path into a Target path.

                if(!Directory.Exists(fullPath)) //Ensure no duplicate folder are created.
                    Directory.CreateDirectory(fullPath); //Create the new directory.

                copyProgressBar.Value++; //Increment the Progress Bar value.
            }

            foreach (var file in _scanResults.removeResults.files) //Loop through each files to remove (the order is important since old files can take too much spaces for the new ones).
            {
                string fullPath = GetFullPath(file, PathType.Target); //Convert the reduced path into a Target path.

                if (File.Exists(fullPath)) //Ensure the file still exist.
                {
                    FileInfo info = new FileInfo(fullPath); //Get the file information.

                    if (info.IsReadOnly)
                        info.IsReadOnly = false; //Remove the ReadOnly attribute since read only files can't be deleted.

                    File.Delete(fullPath); //Delete the file.
                }

                copyProgressBar.Value++; //Increment the Progress Bar value.
            }

            foreach (var dir in _scanResults.removeResults.folders) //Loop through each folders to remove (the order is important since ReadOnly files can't be deleted recursively).
            {
                string fullPath = GetFullPath(dir, PathType.Target); //Convert the reduced path into a Target path.

                //Try catch for any potential folders the user lack permission for.
                try
                {
                    if (Directory.Exists(fullPath)) //Ensure the folder still exist.
                        Directory.Delete(fullPath, true); //Delete with recursive to avoid exceptions (if recursive is set to false and there are still files/folders inside the directory an exception is thrown).
                }
                catch (UnauthorizedAccessException)
                {
                    failed.Add(fullPath); //Keep track of the path of failed operations.
                }
                //

                copyProgressBar.Value++; //Increment the Progress Bar value.
            }

            foreach (var file in _scanResults.addResults.files) //Loop through each files to add (the order is important since the files can't be added if their parents directories aren't added and if the old files take too much space).
            {
                string tarFullPath = GetFullPath(file, PathType.Target); //Convert the reduced path into a Target path.
                string souFullPath = GetFullPath(file, PathType.Source); //Convert the reduced path into a Source path.

                //Try catch for any potential error.
                try
                {
                    if (!File.Exists(tarFullPath) && File.Exists(souFullPath)) //Ensure no files with the same path already exist and that the Source file exist.
                        File.Copy(souFullPath, tarFullPath);
                    else if (File.Exists(tarFullPath))
                        failed.Add(tarFullPath); //Keep track of the path of failed operations.
                    else
                        failed.Add(souFullPath); //Keep track of the path of failed operations.
                }
                catch (IOException)
                {
                    failed.Add(GetFullPath(file, PathType.Source)); //Keep track of the path of failed operations.
                }
                //

                copyProgressBar.Value++; //Increment the Progress Bar value.
            }

            foreach (var file in _scanResults.updateResults.files) //Loop through each files to update.
            {
                string tarFullPath = GetFullPath(file, PathType.Target); //Convert the reduced path into a Target path.
                string souFullPath = GetFullPath(file, PathType.Source); //Convert the reduced path into a Source path.

                try
                {
                    if (File.Exists(souFullPath)) //Ensure the Source file exist (no need to verify if the Target file exist since in the worst case it will be simply copied).
                        File.Copy(souFullPath, tarFullPath, true); //Overwrite the Target file.
                    else
                        failed.Add(souFullPath); //Keep track of the path of failed operations.
                }
                catch (IOException)
                {
                    failed.Add(souFullPath); //Keep track of the path of failed operations.
                }

                copyProgressBar.Value++; //Increment the Progress Bar value.
            }

            if (failed.Count > 0) //If one or more operations failed warn the user.
                MessageBox.Show(this, $"{failed.Count} file(s)/folder(s) failed the operation, please do a scan again to see whichs file(s) or folder(s) are not updated", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //Clear the scan results and the nodes.
            _scanResults.addResults.Clear();
            _scanResults.removeResults.Clear();
            _scanResults.updateResults.Clear();
            treeViewChanges.Nodes.Clear();
            //

            copyProgressBar.Value = copyProgressBar.Minimum; //Reset the Progress Bar value.

            UpdateCountLabels(); //Reset the count labels.
        }

        private void buttonSourceSelect_Click(object sender, EventArgs e) => SelectPath(PathType.Source); //Open the path selection dialog.

        private void buttonTargetSelect_Click(object sender, EventArgs e) => SelectPath(PathType.Target); //Open the path selection dialog.

        private void textBoxTargetPath_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBoxTargetPath.Text, textBoxTargetPath.Font); //Calculate the new text size based on the textBox font.

            textBoxTargetPath.Width = size.Width; //Apply the new size width to keep the path fully visible.
        }

        private void textBoxSourcePath_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBoxSourcePath.Text, textBoxSourcePath.Font); //Calculate the new text size based on the textBox font.

            textBoxSourcePath.Width = size.Width; //Apply the new size width to keep the path fully visible.
        }
    }
}
