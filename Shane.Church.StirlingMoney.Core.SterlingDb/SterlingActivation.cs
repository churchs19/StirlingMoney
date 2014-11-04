using Grace;
using Shane.Church.StirlingMoney.Core.Services;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb
{
	public static class SterlingActivation
	{
		public static void ActivateDatabase()
		{
			var engine = ContainerService.Container.Locate<SterlingEngine>();

			SterlingDefaultLogger logger = new SterlingDefaultLogger(engine.SterlingDatabase, SterlingLogLevel.Verbose);

//			engine.Reset();

			engine.Activate();

			engine.SterlingDatabase.RegisterDatabase<StirlingMoneyDatabaseInstance>("Money", ContainerService.Container.Locate<ISterlingDriver>());

			engine.SterlingDatabase.GetDatabase("Money").RefreshAsync().Wait(1000);
		}

	}
}
