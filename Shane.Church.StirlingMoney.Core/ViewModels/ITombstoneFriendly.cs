
using System.Threading.Tasks;
namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public interface ITombstoneFriendly
	{
		Task DeactivateAsync();
		void Deactivate();
		void Activate();
		Task ActivateAsync();
	}
}
