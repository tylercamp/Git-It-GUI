﻿using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GitItGUI.Core;
using System.IO;
using Avalonia.Interactivity;
using System.Threading;
using Avalonia.Threading;

namespace GitItGUI
{
	public delegate void MainContentPageNavigateMethod();

	public class MainContent : UserControl, NavigationPage
	{
		public static MainContent singleton;
		public event MainContentPageNavigateMethod MainContentPageNavigatedTo, MainContentPageNavigateFrom;

		private TextBlock repoName;
		private Button closeRepoButton;
		private TabControl tabControl;
		public int tabControlNavigateIndex = -1;

		public MainContent()
		{
			singleton = this;
			AvaloniaXamlLoader.Load(this);

			repoName = this.Find<TextBlock>("repoName");
			closeRepoButton = this.Find<Button>("closeRepoButton");
			tabControl = this.Find<TabControl>("tabControl");

			closeRepoButton.Click += CloseRepoButton_Click;
			RepoManager.RepoRefreshedCallback += RepoManager_RepoRefreshedCallback;
		}

		private void CloseRepoButton_Click(object sender, RoutedEventArgs e)
		{
			RepoManager.Close();
			MainWindow.LoadPage(PageTypes.Start);
		}

		private void RepoManager_RepoRefreshedCallback()
		{
			if (Dispatcher.UIThread.CheckAccess())
			{
				RepoManager_RepoRefreshedCallback_UIThread();
			}
			else
			{
				bool isDone = false;
				Dispatcher.UIThread.InvokeAsync(delegate
				{
					RepoManager_RepoRefreshedCallback_UIThread();
					isDone = true;
				});

				while (!isDone) Thread.Sleep(1);
			}
		}

		private void RepoManager_RepoRefreshedCallback_UIThread()
		{
			string name = RepoManager.repoPath;
			if (!string.IsNullOrEmpty(name)) repoName.Text = name.Substring(Path.GetDirectoryName(name).Length + 1);
			else repoName.Text = "";
		}

		public void NavigatedFrom()
		{
			if (MainContentPageNavigateFrom != null) MainContentPageNavigateFrom();
		}

		public void NavigatedTo()
		{
			if (tabControlNavigateIndex != -1) tabControl.SelectedIndex = tabControlNavigateIndex;
			tabControlNavigateIndex = -1;
			if (MainContentPageNavigatedTo != null) MainContentPageNavigatedTo();
		}
	}
}
