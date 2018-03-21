using System;
using System.Timers;

namespace cdcrush.lib.task
{
	
/// <summary>
/// Fake Task that reports progress at time intervals
/// </summary>
class CTestTask:CTask
{
	Timer timer;
	
	// --
	public CTestTask(string Name="TestTask", float time=2000, int steps=10,bool reportsProgress=true):base(null,Name)
	{
		float every = time / steps;
		int progressInc = (int) Math.Ceiling(100.0f/steps);
		int triggers = 0;
		
		timer = new Timer(every);
		timer.Elapsed += (s, a) =>
		{
			triggers++;
			if(triggers == steps) {
				complete();
			}else{
				PROGRESS += progressInc;
			}
		};
		killExtra = ()=> timer.Stop();
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();
		timer.Start();
	}// -----------------------------------------

}// --
}// --
