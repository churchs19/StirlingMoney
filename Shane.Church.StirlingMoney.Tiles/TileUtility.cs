using System;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows;

namespace Shane.Church.StirlingMoney.Tiles
{
	public static class TileUtility
	{
		public static bool TileExists(Guid Id)
		{
			ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().ToLower().Contains(Id.ToString().ToLower()));
			return TileToFind != null;
		}

		public static void AddOrUpdateAccountTile(Guid AccountId, string AccountName, double Balance)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					ShellTile TileToFind = ShellTile.ActiveTiles.Where(t => t.NavigationUri.ToString().ToLower().Contains(AccountId.ToString().ToLower())).FirstOrDefault();
					StandardTileData NewTileData = new StandardTileData
					{
						BackgroundImage = new Uri(@"/Images/AccountTileBack.png", UriKind.Relative),
						Title = AccountName,
						Count = 0,
						BackTitle = AccountName,
						BackContent = string.Format(Resources.TileResources.StartTileBalance, Balance.ToString("c"))
					};
					if (TileToFind != null)
					{
						TileToFind.Update(NewTileData);
					}
					else
					{
						ShellTile.Create(new Uri(@"/Transactions.xaml?PinnedTile=True&AccountId=" + AccountId.ToString(), UriKind.Relative), NewTileData);
					}
				}
				catch (Exception ex)
				{
					//					_logger.ErrorException("Error updating tile", ex);
				}
			});
		}

		public static void DeleteTile(Guid Id)
		{

		}
	}
}
