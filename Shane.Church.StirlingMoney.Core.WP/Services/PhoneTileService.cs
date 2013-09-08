using Microsoft.Phone.Shell;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneAccountTileService : ITileService<Account>
	{
		public bool TileExists(Guid id)
		{
			ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().ToLower().Contains(id.ToString().ToLower()));
			return TileToFind != null;
		}
	}
}
