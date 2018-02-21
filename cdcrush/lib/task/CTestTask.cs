using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	public CTestTask(string Name="TestTask", float time=2000, int steps=10)
	{
		float every = time / steps;
		int progressInc = (int) Math.Ceiling(100.0f/steps);
		int triggers = 0;
		
		name = Name;

		timer = new Timer(every);
		timer.Elapsed += (s, a) =>
		{
			triggers++;
			if(triggers == steps)
			{
				complete();
			}else{
				PROGRESS += progressInc;
			}
		};
	}// -----------------------------------------

	// --
	public override void kill()
	{
		timer.Dispose();
		base.kill();
	}// -----------------------------------------

	// --
	public override void start()
	{
		base.start();
		timer.Start();
	}// -----------------------------------------


}// --
}// --
