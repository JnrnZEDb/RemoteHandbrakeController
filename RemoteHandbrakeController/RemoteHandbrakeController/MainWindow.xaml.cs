﻿using System;
using System.Configuration;
using System.Collections.Generic;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<FileInfo> lstFilesToBeEncoded = new List<FileInfo>();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void BtnMovies_Clicked(object sender, RoutedEventArgs e)
		{
			ListDirectory(treeFiles, String.Format("{0}Movies", Properties.Settings.Default.INPUT_DIRECTORY));
		}

		private void BtnTV_Clicked(object sender, RoutedEventArgs e)
		{
			ListDirectory(treeFiles, String.Format("{0}TV Shows", Properties.Settings.Default.INPUT_DIRECTORY));
		}

		private void BtnAnime_Clicked(object sender, RoutedEventArgs e)
		{
			ListDirectory(treeFiles, String.Format("{0}Anime", Properties.Settings.Default.INPUT_DIRECTORY));
		}

		private void BtnEncode_Click(object sender, RoutedEventArgs e)
		{
			lstFilesToBeEncoded.Clear();
			FindChecked((TreeViewItem)treeFiles.Items.GetItemAt(0));
			EncodeWindow wndEncode = new EncodeWindow(lstFilesToBeEncoded);
			wndEncode.Show();
		}

		private void mnuConfig_Click(object sender, RoutedEventArgs e)
		{
			Config wndConfig = new Config();
			wndConfig.Show();
		}

		private void mnuExit_Click(object sender, RoutedEventArgs e)
		{
			lstFilesToBeEncoded = null;
			Application.Current.Shutdown();
			return;
		}

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

	}
}
