
namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface ILicensingService
	{
		bool IsSyncLicensed();
		bool IsCSVLicensed();
		bool IsAdvancedReportingLicensed();
	}
}
