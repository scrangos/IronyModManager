﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2020
// ***********************************************************************
// <copyright file="FileDialogAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Class FileDialogAction.
    /// Implements the <see cref="IronyModManager.Implementation.Actions.IFileDialogAction" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Actions.IFileDialogAction" />
    public class FileDialogAction : IFileDialogAction
    {
        #region Methods

        /// <summary>
        /// open dialog as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> OpenDialogAsync(string title, params string[] extensions)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = title;
            dialog.Filters = GetFilters(extensions);
            dialog.Directory = GetInitialDirectory();
            dialog.AllowMultiple = false;
            var result = await dialog.ShowAsync(GetMainWindow());
            return result.FirstOrDefault();
        }

        /// <summary>
        /// save dialog as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> SaveDialogAsync(string title, params string[] extensions)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = title;
            dialog.Filters = GetFilters(extensions);
            dialog.Directory = GetInitialDirectory();
            var result = await dialog.ShowAsync(GetMainWindow());
            return result;
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        /// <returns>List&lt;FileDialogFilter&gt;.</returns>
        private List<FileDialogFilter> GetFilters(params string[] extensions)
        {
            var filter = new List<FileDialogFilter>();
            foreach (var item in extensions)
            {
                filter.Add(new FileDialogFilter()
                {
                    Name = item,
                    Extensions = new List<string>() { item }
                });
            }
            return filter;
        }

        /// <summary>
        /// Gets the initial directory.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetInitialDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns>Window.</returns>
        private Window GetMainWindow()
        {
            return ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
        }

        #endregion Methods
    }
}
