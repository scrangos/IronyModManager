﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-09-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="ModifyCollectionControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;
using SmartFormat;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ModifyCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModifyCollectionControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod definition analyze handler
        /// </summary>
        private readonly ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler;

        /// <summary>
        /// The mod definition load handler
        /// </summary>
        private readonly ModDefinitionLoadHandler modDefinitionLoadHandler;

        /// <summary>
        /// The mod merge progress handler
        /// </summary>
        private readonly ModMergeProgressHandler modMergeProgressHandler;

        /// <summary>
        /// The mod merge service
        /// </summary>
        private readonly IModMergeService modMergeService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The shut down state
        /// </summary>
        private readonly IShutDownState shutDownState;

        /// <summary>
        /// The definition analyze load handler
        /// </summary>
        private IDisposable definitionAnalyzeLoadHandler = null;

        /// <summary>
        /// The definition load handler
        /// </summary>
        private IDisposable definitionLoadHandler = null;

        /// <summary>
        /// The definition progress handler
        /// </summary>
        private IDisposable definitionProgressHandler = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="modDefinitionAnalyzeHandler">The mod definition analyze handler.</param>
        /// <param name="modDefinitionLoadHandler">The mod definition load handler.</param>
        /// <param name="modMergeProgressHandler">The mod merge progress handler.</param>
        /// <param name="shutDownState">State of the shut down.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modMergeService">The mod merge service.</param>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        public ModifyCollectionControlViewModel(ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler,
            ModDefinitionLoadHandler modDefinitionLoadHandler, ModMergeProgressHandler modMergeProgressHandler,
            IShutDownState shutDownState, IGameService gameService, IModMergeService modMergeService,
            IModCollectionService modCollectionService, IModPatchCollectionService modPatchCollectionService, ILocalizationManager localizationManager)
        {
            this.modCollectionService = modCollectionService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.localizationManager = localizationManager;
            this.gameService = gameService;
            this.modMergeService = modMergeService;
            this.modDefinitionLoadHandler = modDefinitionLoadHandler;
            this.modDefinitionAnalyzeHandler = modDefinitionAnalyzeHandler;
            this.modMergeProgressHandler = modMergeProgressHandler;
            this.shutDownState = shutDownState;
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Enum ModifyAction
        /// </summary>
        public enum ModifyAction
        {
            /// <summary>
            /// The rename
            /// </summary>
            Rename,

            /// <summary>
            /// The merge
            /// </summary>
            Merge,

            /// <summary>
            /// The duplicate
            /// </summary>
            Duplicate
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets or sets the active collection.
        /// </summary>
        /// <value>The active collection.</value>
        public virtual IModCollection ActiveCollection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; set; }

        /// <summary>
        /// Gets or sets the duplicate.
        /// </summary>
        /// <value>The duplicate.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Duplicate)]
        public virtual string Duplicate { get; protected set; }

        /// <summary>
        /// Gets or sets the duplicate command.
        /// </summary>
        /// <value>The duplicate command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> DuplicateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge.
        /// </summary>
        /// <value>The merge.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Merge)]
        public virtual string Merge { get; protected set; }

        /// <summary>
        /// Gets or sets the merge command.
        /// </summary>
        /// <value>The merge command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> MergeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the rename.
        /// </summary>
        /// <value>The rename.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Rename)]
        public virtual string Rename { get; protected set; }

        /// <summary>
        /// Gets or sets the rename command.
        /// </summary>
        /// <value>The rename command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> RenameCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IEnumerable<IMod> SelectedMods { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show advanced features].
        /// </summary>
        /// <value><c>true</c> if [show advanced features]; otherwise, <c>false</c>.</value>
        public virtual bool ShowAdvancedFeatures { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            ShowAdvancedFeatures = (gameService.GetSelected()?.AdvancedFeaturesSupported).GetValueOrDefault();

            IModCollection copyCollection(string requestedName)
            {
                var collections = modCollectionService.GetAll();
                var count = collections.Where(p => p.Name.Equals(requestedName, StringComparison.OrdinalIgnoreCase)).Count();
                string name = string.Empty;
                if (count == 0)
                {
                    name = requestedName;
                }
                else
                {
                    name = $"{requestedName} ({count})";
                }
                while (collections.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    count++;
                    name = $"{requestedName} ({count})";
                }
                var copied = modCollectionService.Create();
                copied.IsSelected = true;
                copied.Mods = ActiveCollection.Mods;
                copied.Name = name;
                return copied;
            }

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);

            RenameCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<ModifyAction>(ModifyAction.Rename, CommandState.Success);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            DuplicateCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null)
                {
                    var copy = copyCollection(ActiveCollection.Name);
                    if (modCollectionService.Save(copy))
                    {
                        await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Rename_Message));
                        await modPatchCollectionService.CopyPatchCollectionAsync(ActiveCollection.Name, copy.Name);
                        await TriggerOverlayAsync(false);
                        return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            MergeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null)
                {
                    await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFree();

                    SubscribeToProgressReports(disposables);

                    var suffix = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.MergedCollectionSuffix);
                    var copy = copyCollection($"{ActiveCollection.Name} {suffix}");

                    var mode = await modPatchCollectionService.GetPatchStateModeAsync(ActiveCollection.Name);
                    if (mode == PatchStateMode.None)
                    {
                        // fallback to default mod if no patch collection specified
                        mode = PatchStateMode.Default;
                    }

                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = 0,
                        Count = 1,
                        TotalCount = 3
                    });
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Loading_Definitions);
                    await TriggerOverlayAsync(true, message, overlayProgress);

                    modPatchCollectionService.ResetPatchStateCache();
                    var definitions = await Task.Run(() =>
                    {
                        return modPatchCollectionService.GetModObjects(gameService.GetSelected(), SelectedMods);
                    }).ConfigureAwait(false);

                    var conflicts = await Task.Run(() =>
                    {
                        if (definitions != null)
                        {
                            return modPatchCollectionService.FindConflicts(definitions, ActiveCollection.Mods.ToList(), mode);
                        }
                        return null;
                    }).ConfigureAwait(false);

                    var mergeMod = await Task.Run(async () =>
                    {
                        return await modMergeService.MergeCollectionAsync(conflicts, SelectedMods.Select(p => p.Name).ToList(), copy.Name).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                    copy.Mods = new List<string>() { mergeMod.DescriptorFile };

                    await TriggerOverlayAsync(false);

                    definitionAnalyzeLoadHandler?.Dispose();
                    definitionLoadHandler?.Dispose();
                    definitionProgressHandler?.Dispose();

                    if (modCollectionService.Save(copy))
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            ShowAdvancedFeatures = (game?.AdvancedFeaturesSupported).GetValueOrDefault();
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// Subscribes to progress reports.
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void SubscribeToProgressReports(CompositeDisposable disposables)
        {
            definitionLoadHandler?.Dispose();
            definitionLoadHandler = modDefinitionLoadHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Loading_Definitions);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                {
                    PercentDone = s.Percentage,
                    Count = 1,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionAnalyzeLoadHandler = modDefinitionAnalyzeHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Analyzing_Definitions);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                {
                    PercentDone = s.Percentage,
                    Count = 2,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionProgressHandler?.Dispose();
            definitionProgressHandler = modMergeProgressHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Merging_Collection);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                {
                    PercentDone = s.Percentage,
                    Count = 3,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            }).DisposeWith(disposables);
        }

        #endregion Methods
    }
}
