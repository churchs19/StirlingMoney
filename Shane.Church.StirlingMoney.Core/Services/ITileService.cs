
namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface ITileService<T, TKey>
	{
		bool TileExists(TKey id);
		void AddTile(TKey id);
		void UpdateTile(TKey id);
		void DeleteTile(TKey Id);
	}
}
