using System.Diagnostics;

namespace VisulightLibrary
{
	public class Simulation
	{
		// Speed of light in km/s.
		public const double c = 299_792.458;

		public enum State { STOPPED, RUNNING, PAUSED }

		public State state = State.STOPPED;

		private Stopwatch stopwatch;
		// Distance in kms separating the two objects.
		private double distance;
		// Time taken by light to travel from object A to object B in milliseconds.
		private ulong totalMillis;

		public Simulation(double _distance)
		{
			distance = _distance;
			totalMillis = (ulong)(distance * c) * 1000;
		}

		public void Start()
		{
			stopwatch = Stopwatch.StartNew();
			state = State.RUNNING;
		}

		public void Pause()
		{
			if (stopwatch != null)
			{
				stopwatch.Stop();
				state = State.PAUSED;
			}
		}

		public void Resume()
		{
			if (stopwatch != null)
			{
				stopwatch.Start();
				state = State.RUNNING;
			}
		}

		public void Stop()
		{
			if (stopwatch != null)
			{
				stopwatch.Stop();
				state = State.STOPPED;
			}
		}

		public void ResetClock()
		{
			if (stopwatch != null)
			{
				stopwatch.Reset();
				stopwatch.Start();
			}
		}

		public double GetDistanceTraveledRatio()
		{
			double elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000d;
			double distanceTraveled = elapsedSeconds * c;
			return distanceTraveled / distance;
		}
	}
}
