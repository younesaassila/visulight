using System;
using System.Diagnostics;
using System.Threading;

namespace VisulightLibrary
{
	public class Simulation
	{
		public enum SimState { STOPPED, RUNNING, PAUSED }

		public SimState State { get; private set; } = SimState.STOPPED;

		private Stopwatch Stopwatch { get; set; } = new Stopwatch();

		private Scene Scene { get; set; }

		private Thread Thread { get; set; }

		public Simulation(Scene scene, Thread thread)
		{
			Scene = scene;
			Thread = thread;
		}

		public void Start()
		{
			Stopwatch = Stopwatch.StartNew();
			State = SimState.RUNNING;
		}

		public void Pause()
		{
			if (State == SimState.RUNNING)
			{
				Stopwatch.Stop();
				State = SimState.PAUSED;
			}
			else
			{
				throw new Exception("Cannot pause the simulation if it is not running.");
			}
		}

		public void Resume()
		{
			if (State == SimState.PAUSED)
			{
				Stopwatch.Start();
				State = SimState.RUNNING;
			}
			else
			{
				throw new Exception("Cannot resume the simulation if it is not paused.");
			}
		}

		public void Stop()
		{
			if (State == SimState.RUNNING || State == SimState.PAUSED)
			{
				Stopwatch.Stop();
				State = SimState.STOPPED;
			}
			else
			{
				throw new Exception("Cannot stop the simulation if it is not running or paused.");
			}
		}

		public void ResetPhotonDistance()
		{
			Stopwatch.Reset();
			Stopwatch.Start();
		}

		public double GetDistanceTraveledRatio()
		{
			double elapsedSeconds = Stopwatch.ElapsedMilliseconds / 1000d;
			double distanceTraveled = elapsedSeconds * Scene.Photon.Speed;
			return distanceTraveled / Scene.Distance;
		}
	}
}
