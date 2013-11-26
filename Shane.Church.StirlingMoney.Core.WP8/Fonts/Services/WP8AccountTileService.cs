using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Controls;
using Shane.Church.Utility.Core.WP;
using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class WP7AccountTileService : ITileService<Account, Guid>
	{
		INavigationService _navService;

		public WP7AccountTileService(INavigationService navService)
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
			if (!TileExists(id))
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
#if !AGENT
#if WP8
#else
					RadExtendedTileData tileData = GetWP7TileData(id);
#endif
					if (tileData != null)
					{
						try
						{
							var navUri = _navService.NavigationUri<TransactionListViewModel>(new TransactionListParams() { Id = id, PinnedTile = true });
							if (navUri != null)
							{
								LiveTileHelper.CreateOrUpdateTile(tileData, navUri);
							}
						}
						catch (Exception ex)
						{
							DebugUtility.SaveDiagnosticException(ex);
							throw ex;
						}
					}
#endif
				});
			}
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
#if WP8
					RadFlipTileData tileData = null;
					if (tileContacts.Count > 0)
					{
						displayName = tileContacts.First().DisplayName;
						BirthdayTileBackViewModel backTileModel = new BirthdayTileBackViewModel(tileContacts);
						MediumTileBackUserControl medBackTile = new MediumTileBackUserControl() { DataContext = backTileModel };
						WideTileBackUserControl wideBackTile = new WideTileBackUserControl() { DataContext = backTileModel };
						tileData = new RadFlipTileData()
						{
							Title = Properties.Resources.AppTitle,
							BackTitle = Properties.Resources.AppTitle,
							BackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "m")) ?
													new Uri(string.Format(isoStoreUri, displayName, "m"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.Relative),
							BackVisualElement = medBackTile,
							SmallBackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "s")) ?
													new Uri(string.Format(isoStoreUri, displayName, "s"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileSmall.png", UriKind.RelativeOrAbsolute),
							WideBackgroundImage = appStorage.FileExists(string.Format(isoStorePath, displayName, "w")) ?
													new Uri(string.Format(isoStoreUri, displayName, "w"), UriKind.RelativeOrAbsolute) :
													new Uri("/Assets/Tiles/BirthdayTileWide.png", UriKind.RelativeOrAbsolute),
							WideBackVisualElement = wideBackTile
						};
					}
					else
					{
						tileData = new RadFlipTileData()
						{
							Title = Properties.Resources.AppTitle,
							BackgroundImage = new Uri("/Assets/Tiles/BirthdayTileMedium.png", UriKind.Relative),
							SmallBackgroundImage = new Uri("/Assets/Tiles/BirthdayTileSmall.png", UriKind.RelativeOrAbsolute),
							WideBackgroundImage = new Uri("/Assets/Tiles/BirthdayTileWide.png", UriKind.RelativeOrAbsolute),
						};
					}
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - RadFlipTileData created");
#endif
#else
					RadExtendedTileData tileData = GetWP7TileData(id);
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("UpdateTileSynchronous - RadExtendedTileData created");
#endif
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

		private RadExtendedTileData GetWP7TileData(Guid id)
		{
			var model = KernelService.Kernel.Get<AccountTileViewModel>();
			if (model.LoadData(id).Wait(1000))
			{
				RadExtendedTileData tileData = null;
				MediumTileFrontWP7 front = new MediumTileFrontWP7() { DataContext = model };
				MediumTileBackWP7 back = new MediumTileBackWP7() { DataContext = model };

				tileData = new RadExtendedTileData()
				{
					Title = model.AccountName,
					BackTitle = model.AccountName,
					VisualElement = front,
					BackVisualElement = back,
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
