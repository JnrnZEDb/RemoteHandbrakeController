using System;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for MediaSelection.xaml
	/// </summary>
	public partial class MediaSelectionPage : Page
	{
		public ObservableCollection<InputDirectory> inputDirectories { get; set; }

		List<FileInfo> lstFilesToBeEncoded = new List<FileInfo>();		
		private XMLConfig xmlConfig;

		public MediaSelectionPage(XMLConfig config)
		{
			InitializeComponent();
			DataContext = this;

			xmlConfig = config;
			inputDirectories = new ObservableCollection<InputDirectory>(xmlConfig.InputDirectories);
		}

		#region BUTTON_CLICKS
		private void BtnLoad_Clicked(object sender, RoutedEventArgs e)
		{
			if (inputDirectories.Count > 0 && lstInputs.SelectedValue != null)
			{
				ListDirectory(treeFiles, (string)lstInputs.SelectedValue);
			}
		}

		private void BtnAddLib_Clicked(object sender, RoutedEventArgs e)
		{
			InputDirectory inputDirectory;
			AddLibrary wndAddLib = new AddLibrary();
			if (wndAddLib.ShowDialog() == true)
			{
				inputDirectory = new InputDirectory(wndAddLib.LibraryName, wndAddLib.Path);
				xmlConfig.InputDirectories.Add(inputDirectory);
				Globals.SaveConfig(Globals.CONFIG_NAME, xmlConfig);
				inputDirectories.Add(inputDirectory);
			}
			wndAddLib = null;
		}

		private void BtnEncode_Click(object sender, RoutedEventArgs e)
		{
			if (treeFiles.Items.Count == 0)
			{
				MessageBox.Show("No input has been loaded.", "ERROR");
				return;
			}
			try
			{
				lstFilesToBeEncoded.Clear();
				FindChecked((TreeViewItem)treeFiles.Items.GetItemAt(0));
				if (lstFilesToBeEncoded.Count == 0)
				{
					MessageBox.Show("No files were selected.", "ERROR");
					return;
				}
				EncodePage pageEncode = new EncodePage(lstFilesToBeEncoded, xmlConfig, this);
				treeFiles.Items.Clear();
				NavigationService.Navigate(pageEncode);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "ERROR");
			}	
		}

		/// <summary>
		/// Right click on library item, select Delete
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LibDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				TextBlock txt = ((ContextMenu)menuItem.Parent).PlacementTarget as TextBlock;
				xmlConfig.InputDirectories.Remove((InputDirectory)txt.DataContext);
				Globals.SaveConfig(Globals.CONFIG_NAME, xmlConfig);
				inputDirectories.Remove((InputDirectory)txt.DataContext);
			}
		}

		#endregion

		#region FUNCTIONS
		/// <summary>
		/// Fills TreeView with files from specified directory
		/// </summary>
		/// <param name="treeView"></param>
		/// <param name="path"></param>
		private void ListDirectory(TreeView treeView, string path)
		{
			treeView.Items.Clear();
			var rootDirectoryInfo = new DirectoryInfo(path);
			treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
		}

		/// <summary>
		/// Recursively builds treeView
		/// </summary>
		/// <param name="directoryInfo"></param>
		/// <returns></returns>
		private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
		{
			var directoryNode = new TreeViewItem
			{
				Header = directoryInfo.Name
			};
			foreach (var directory in directoryInfo.GetDirectories())
			{
				directoryNode.Items.Add(CreateDirectoryNode(directory));
			}
			foreach (var file in directoryInfo.GetFiles("*.mkv"))
			{
				directoryNode.Items.Add(new TreeViewItem
				{
					Header = new CheckBox()
					{
						Content = file,
					},

				});
			}

			return directoryNode;
		}

		/// <summary>
		/// Gets all checked files from TreeViewItem
		/// </summary>
		/// <param name="treeViewFile"></param>
		private void FindChecked(TreeViewItem treeViewFile)
		{
			foreach (TreeViewItem file in treeViewFile.Items)
			{
				if (file.Header is CheckBox fileCheckBox)
				{
					fileCheckBox = (CheckBox)file.Header;
					if (fileCheckBox.IsChecked.Value)
					{
						FileInfo checkedFile = (FileInfo)fileCheckBox.Content;
						lstFilesToBeEncoded.Add(checkedFile);
					}
				}
				FindChecked(file);
			}
		}
		#endregion
	}
}
