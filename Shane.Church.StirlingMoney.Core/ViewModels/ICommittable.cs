using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public interface ICommittable
	{
		Task Commit();
	}
}
