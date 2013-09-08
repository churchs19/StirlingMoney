using System;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface ITileService<T>
	{
		bool TileExists(Guid id);
	}
}
