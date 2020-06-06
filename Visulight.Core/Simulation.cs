using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Visulight.Core
{
	public class Simulation
	{
		// Speed of light in km/s.
		private const double c = 299_792.458;

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
		}

		public void Start()
		{
			stopwatch = Stopwatch.StartNew();
		}

		public void Pause()
		{

		}

		public void Stop()
		{

		}

		public double GetLightDistanceRatio()
		{
			totalMillis = (ulong)stopwatch.ElapsedMilliseconds;
			ulong totalSeconds = totalMillis / 1000;
			double distanceTraveled = totalSeconds * c % distance;
			return distanceTraveled / distance;
		}
	}
}
