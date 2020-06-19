using System;
using System.Diagnostics;

namespace VisulightLibrary
{
    public class Simulation
    {
        public enum SimState { STOPPED, RUNNING, PAUSED }

        public SimState State { get; private set; } = SimState.STOPPED;

        public event EventHandler<EventArgs> Started;

        public event EventHandler<EventArgs> Stopped;

        public Scene Scene { get; }


        private Stopwatch stopwatch = new Stopwatch();

        
        public Simulation(Scene scene)
        {
            Scene = scene;
        }

        public void Start()
        {
            stopwatch = Stopwatch.StartNew();
            State = SimState.RUNNING;
            Started?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            if (State == SimState.RUNNING || State == SimState.PAUSED)
            {
                stopwatch.Stop();
                State = SimState.STOPPED;
                Stopped?.Invoke(this, new EventArgs());
            }
            else
            {
                throw new Exception("Cannot stop the simulation if it is not running or paused.");
            }
        }

        public void ResetPhotonDistance()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public double GetDistanceTraveledRatio()
        {
            double elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000d;
            double distanceTraveled = elapsedSeconds * Scene.Photon.Speed;
            return distanceTraveled / Scene.Distance;
        }
    }
}
