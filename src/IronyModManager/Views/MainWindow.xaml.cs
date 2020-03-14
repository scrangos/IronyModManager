﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.DI;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class MainWindow.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.ViewModels.MainWindowViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.ViewModels.MainWindowViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainWindow : BaseWindow<MainWindowViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Handles the closing.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool HandleClosing()
        {
            var service = DIResolver.Get<IWindowStateService>();
            var state = service.Get();
            if (WindowState == WindowState.Maximized)
            {
                state.IsMaximized = true;
            }
            else
            {
                var totalScreenX = Screens.All.Sum(p => p.WorkingArea.Width);
                var totalScreenY = Screens.All.Max(p => p.WorkingArea.Height);
                var locX = Position.X + ClientSize.Width > totalScreenX ? totalScreenX - ClientSize.Width : Position.X;
                var locY = Position.Y + ClientSize.Height > totalScreenY ? totalScreenY - ClientSize.Height : Position.Y;
                if (locX < 0.0)
                {
                    locX = 0;
                }
                if (locY < 0.0)
                {
                    locY = 0;
                }
                state.Height = Convert.ToInt32(ClientSize.Height);
                state.Width = Convert.ToInt32(ClientSize.Width);
                state.IsMaximized = false;
                state.LocationX = Convert.ToInt32(locX);
                state.LocationY = Convert.ToInt32(locY);
            }
            service.Save(state);
            return base.HandleClosing();
        }

        /// <summary>
        /// Initializes the size of the window.
        /// </summary>
        protected virtual void InitWindowSize()
        {
            var service = DIResolver.Get<IWindowStateService>();
            if (service.IsDefined())
            {
                var state = service.Get();
                Height = state.Height.GetValueOrDefault();
                Width = state.Width.GetValueOrDefault();
                WindowState = state.IsMaximized.GetValueOrDefault() ? WindowState.Maximized : WindowState.Normal;
                // Silly setup code isn't it?
                var pos = Position.WithX(state.LocationX.GetValueOrDefault());
                pos = pos.WithY(state.LocationY.GetValueOrDefault());
                Position = pos;
            }
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            if (!Design.IsDesignMode)
            {
                InitWindowSize();
            }
        }

        #endregion Methods
    }
}
