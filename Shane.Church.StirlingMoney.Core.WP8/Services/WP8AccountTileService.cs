using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Controls;
using Shane.Church.Utility.Core.WP;
using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.Core.WP8.Services
{
	public class WP8AccountTileService : ITileService<Account, Guid>
	{
		INavigationService _navService;

		public WP8AccountTileService(INavigationService navService)
		{
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
		}

		public bool TileExists(Guid id)
		{
			var tileUri = _navService.NavigationUri<TransactionListViewModel>(new TransactionListParams() { Id = id, PinnedTile = true });
			return LiveTileHelper.GetTile(tileUri) != null;
		}

		public void AddTile(Guid id)
		{
#if !AGENT
			if (!TileExists(id))
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					RadFlipTileData tileData = GetWP8TileData(id);
					if (tileData != null)
					{
						try
						{
							var navUri = _navService.NavigationUri<TransactionListViewModel>(new TransactionListParams() { Id = id, PinnedTile = true });
							if (navUri != null)
							{
								LiveTileHelper.CreateOrUpdateTile(tileData, navUri, true);
							}
						}
						catch (Exception ex)
						{
							DebugUtility.SaveDiagnosticException(ex);
							throw ex;
						}
					}
				});
			}
#endif
		}

		public void UpdateTile(Guid id)
		{
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Beginning UpdateTileSynchronous");
#endif
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - Loaded Data");
#endif

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
#if DEBUG
				DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - Medium Tile Control Created");
#endif
				var navUri = _navService.NavigationUri<TransactionListViewModel>(new TransactionListParams() { Id = id, PinnedTile = true });
				ShellTile tileToUpdate = LiveTileHelper.GetTile(navUri);
				if (tileToUpdate != null)
				{
					RadFlipTileData tileData = GetWP8TileData(id);
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - RadFlipTileData created");
#endif
					if (tileData != null)
					{
						try
						{
							LiveTileHelper.UpdateTile(tileToUpdate, tileData);
						}
						catch (Exception ex)
						{
							DebugUtility.SaveDiagnosticException(ex);
							throw ex;
						}
					}

#if DEBUG
					DebugUtility.SaveDiagnosticMessage("Completed tile update");
#endif
				}
			});
		}

		private RadFlipTileData GetWP8TileData(Guid id)
		{
			var model = KernelService.Kernel.Get<AccountTileViewModel>();
			if (model.LoadData(id).Wait(1000))
			{
				RadFlipTileData tileData = null;
				SmallTile small = new SmallTile() { DataContext = model };
				MediumTileFront medFront = new MediumTileFront() { DataContext = model };
				MediumTileBack medBack = new MediumTileBack() { DataContext = model };
				WideTileFront wideFront = new WideTileFront() { DataContext = model };

				tileData = new RadFlipTileData()
				{
					Title = model.AccountName,
					BackTitle = model.AccountName,
					SmallVisualElement = small,
					VisualElement = medFront,
					BackVisualElement = medBack,
					WideVisualElement = wideFront
				};

				return tileData;
			}
			else
				return null;
		}

		public void DeleteTile(Guid Id)
		{
#if !AGENT
			if (TileExists(Id))
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					var tileUri = _navService.NavigationUri<TransactionListViewModel>(new TransactionListParams() { Id = Id, PinnedTile = true });
					var tile = LiveTileHelper.GetTile(tileUri);
					tile.Delete();
				});
			}
#endif
		}
	}
}
