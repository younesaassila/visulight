using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using VisulightLibrary;

namespace Visulight
{
    public partial class MainForm : Form
    {
        private Simulation simulation;

        private Thread simulationThread;

        private delegate void UpdatePhotonLocationDelegate(Point location);


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            List<Scene> defaultScenes = Scene.GetDefaultScenes();

            foreach (Scene scene in defaultScenes)
            {
                presetComboBox.Items.Add(scene.Name);
            }

            presetComboBox.SelectedIndex = 0;
		}


        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            if (simulation.State == Simulation.SimState.RUNNING)
            {
                simulation.Pause();
            }
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            if (simulation.State == Simulation.SimState.PAUSED)
            {
                simulation.Resume();
            }
        }

        private void ButtonShow_Click(object sender, EventArgs e)
        {
            buttonShow.Visible = false;
            
            panelSimulation.Location = new Point(panelOptions.Width, 0);
            panelSimulation.Width -= panelOptions.Width;
            panelOptions.Visible = true;

            buttonHide.Visible = true;
        }

        private void ButtonHide_Click(object sender, EventArgs e)
        {
            buttonHide.Visible = false;

            panelOptions.Visible = false;
            panelSimulation.Location = new Point(0, 0);
            panelSimulation.Width += panelOptions.Width;

            buttonShow.Visible = true;
        }


        private void PresetRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (presetRadioButton.Checked)
            {
                presetRadioButton.ForeColor = Color.LightSkyBlue;
                customRadioButton.ForeColor = Color.WhiteSmoke;
                
                Preset_Load();
            }
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetRadioButton.Checked)
            {
                Preset_Load();
            }
        }

        private void Preset_Load()
        {
            List<Scene> defaultScenes = Scene.GetDefaultScenes();
            Scene defaultScene = defaultScenes[presetComboBox.SelectedIndex];

            presetLabelDistance.Text = $"Distance : {defaultScene.Distance:N0} km";

            SetCurrentSimulationWithScene(new Scene
            {
                Name = defaultScene.Name,
                ObjectA = defaultScene.ObjectA,
                ObjectB = defaultScene.ObjectB,
                Distance = defaultScene.Distance,
                Photon = defaultScene.Photon
            });
        }


        private void CustomRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                presetRadioButton.ForeColor = Color.WhiteSmoke;
                customRadioButton.ForeColor = Color.LightSkyBlue;

                Custom_Load();
            }
        }

        private void CustomNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                Custom_Load();
            }
        }

        private void CustomTextBox_TextChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                Custom_Load();
            } 
        }

        private void CustomExample_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            
            switch (button.Name)
            {
                // Terre - Voyager 1
                case "ex_Terre_Voyager1":
                    customNumeric.Value = 21_700_000_000;
                    customTextBoxPointA.Text = "Terre";
                    customTextBoxPointB.Text = "Voyager 1";
                    break;

                // Saturne - Titan
                case "ex_Saturne_Titan":
                    customNumeric.Value = 1_221_870;
                    customTextBoxPointA.Text = "Saturne";
                    customTextBoxPointB.Text = "Titan";
                    break;

                // Jupiter - Europe
                case "ex_Jupiter_Europe":
                    customNumeric.Value = 671_100;
                    customTextBoxPointA.Text = "Jupiter";
                    customTextBoxPointB.Text = "Europe";
                    break;

                // Soleil - Neptune
                case "ex_Soleil_Neptune":
                    customNumeric.Value = 4_503_443_661;
                    customTextBoxPointA.Text = "Soleil";
                    customTextBoxPointB.Text = "Neptune";
                    break;
            }
        }

        private void Custom_Load()
        {
            SetCurrentSimulationWithScene(new Scene
            {
                Name = $"{customTextBoxPointA.Text}-{customTextBoxPointB.Text}",
                ObjectA = new CelestialObject
				{
                    Name = customTextBoxPointA.Text,
                    Image = "WhitePoint",
                    Width = 10,
                    Height = 10
                },
                ObjectB = new CelestialObject
				{
                    Name = customTextBoxPointB.Text,
                    Image = "WhitePoint",
                    Width = 10,
                    Height = 10
                },
                Distance = (double)customNumeric.Value,
                Photon = new Photon
				{
                    Width = 10
				}
            });
        }


        private void SetCurrentSimulationWithScene(Scene scene)
        {
            if (simulation != null)
			{
                if (simulation.State == Simulation.SimState.RUNNING || simulation.State == Simulation.SimState.PAUSED)
                {
                    simulation.Stop();
                }
            }
            simulation = new Simulation(scene);

			simulation.Started += Simulation_Started;
			simulation.Paused += Simulation_Paused;
			simulation.Resumed += Simulation_Resumed;
			simulation.Stopped += Simulation_Stopped;

			scene.Photon.TargetChanged += Photon_TargetChanged;

            pbPointA.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(scene.ObjectA.Image);
            pbPointA.Width = scene.ObjectA.Width;
            pbPointA.Height = scene.ObjectA.Height;
            pbPointA.Location = new Point(40, (panelSimulation.Height - scene.ObjectA.Height) / 2);
            lbPointA.Text = scene.ObjectA.Name;
            int lbPointA_X = pbPointA.Location.X + (pbPointA.Width - lbPointA.Width) / 2;
            int lbPointA_Y = pbPointA.Location.Y + pbPointA.Height + 40;
            lbPointA.Location = new Point(lbPointA_X, lbPointA_Y);

            pbPointB.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(scene.ObjectB.Image);
            pbPointB.Width = scene.ObjectB.Width;
            pbPointB.Height = scene.ObjectB.Height;
            pbPointB.Location = new Point(panelSimulation.Width - (40 + scene.ObjectB.Width), (panelSimulation.Height - scene.ObjectB.Width) / 2);
            lbPointB.Text = scene.ObjectB.Name;
            int lbPointB_X = pbPointB.Location.X + (pbPointB.Width - lbPointB.Width) / 2;
            int lbPointB_Y = pbPointB.Location.Y + pbPointB.Height + 40;
            lbPointB.Location = new Point(lbPointB_X, lbPointB_Y);

            light.Width = scene.Photon.Width;
            int photonX = pbPointA.Location.X + scene.ObjectA.Width - scene.Photon.Width;
            int photonY = pbPointA.Location.Y + ((scene.ObjectA.Height + light.Height) / 2);
            light.Location = new Point(photonX, photonY);

            labelTime.Text = scene.GetTimeString();
        }

		private void Simulation_Started(object sender, EventArgs e)
        {
            Text = "Visulight ‒ Simulation en cours";
            buttonStart.Visible = false;
            buttonStop.Visible = true;
            simulationThread = new Thread(RunSimulation)
            {
                IsBackground = true
            };
            simulationThread.Start();
        }

        private void Simulation_Paused(object sender, EventArgs e)
        {
            simulationThread.Suspend();
            labelInformation.Text = "Simulation en pause lors du déplacement ou redimensionnement de la fenêtre.";
            labelInformation.ForeColor = Color.LightCoral;
        }

        private void Simulation_Resumed(object sender, EventArgs e)
        {
            simulationThread.Resume();
        }

        private void Simulation_Stopped(object sender, EventArgs e)
		{
			simulationThread.Abort();

            Text = "Visulight";
            buttonStop.Visible = false;

            // On réinitialise la direction puis la position de la lumière
            simulation.Scene.Photon.Target = Photon.TargetObject.OBJECT_B;
            light.BackgroundImage = Properties.Resources.LightGoingTowardsPointB;

            int x = pbPointA.Location.X + pbPointA.Width - light.Width;
            int y = pbPointA.Location.Y + ((pbPointA.Height + light.Height) / 2);
            light.Location = new Point(x, y);

            buttonStart.Visible = true;
            labelInformation.Text = "Cliquez sur 'Démarrer la simulation' pour commencer.";
        }

        private void Photon_TargetChanged(object sender, Photon.TargetObject e)
        {
            Photon photonObj = simulation.Scene.Photon;

            light.BackgroundImage = photonObj.Target == Photon.TargetObject.OBJECT_B ?
                Properties.Resources.LightGoingTowardsPointB :
                Properties.Resources.LightGoingTowardsPointA;
            Point newPhotonLocation = photonObj.Target == Photon.TargetObject.OBJECT_B ?
                new Point(pbPointA.Location.X + pbPointA.Width - light.Width, light.Location.Y) :
                new Point(pbPointB.Location.X, light.Location.Y);
            //light.Anchor = AnchorStyles.Left;
            light.Invoke(new UpdatePhotonLocationDelegate(UpdatePhotonLocation), newPhotonLocation);
            simulation.ResetPhotonDistance();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            simulation.Start();
        }

        private void RunSimulation()
        {
            while (true)
            {
                int delay = 1;

                int distanceInPixels = pbPointB.Location.X - pbPointA.Location.X - pbPointA.Width;
                double ratio = simulation.GetDistanceTraveledRatio();
                int distanceTraveledInPixels = (int)(distanceInPixels * ratio);

                Photon photonObj = simulation.Scene.Photon;

                Point newPhotonLocation = photonObj.Target == Photon.TargetObject.OBJECT_B ?
                    new Point(pbPointA.Location.X + pbPointA.Width - light.Width + distanceTraveledInPixels, light.Location.Y) :
                    new Point(pbPointB.Location.X + light.Width - distanceTraveledInPixels, light.Location.Y);

                light.Invoke(new UpdatePhotonLocationDelegate(UpdatePhotonLocation), newPhotonLocation);

                bool rightSideOutOfBound =
                    photonObj.Target == Photon.TargetObject.OBJECT_B && newPhotonLocation.X >= pbPointB.Location.X - light.Width;
                bool leftSideOutOfBound =
                    photonObj.Target == Photon.TargetObject.OBJECT_A && newPhotonLocation.X <= pbPointA.Location.X + pbPointA.Width;

                if (rightSideOutOfBound || leftSideOutOfBound)
                {
                    photonObj.Target = photonObj.Target == Photon.TargetObject.OBJECT_B ?
                        Photon.TargetObject.OBJECT_A :
                        Photon.TargetObject.OBJECT_B;
                    delay = 20;
                }

                Thread.Sleep(delay);
            }
        }

        private void UpdatePhotonLocation(Point location)
        {
            light.Location = location;
            UpdateInformationLabel();
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            simulation.Stop();
        }

        private void UpdateInformationLabel()
        {
            int distanceInPixels = pbPointB.Location.X - pbPointA.Location.X - pbPointA.Width;
            double ratio = simulation.GetDistanceTraveledRatio();
            int distanceTraveledInPixels = (int)(distanceInPixels * ratio);
            labelInformation.Text = $"{distanceTraveledInPixels} pixels parcourus";
            labelInformation.ForeColor = Color.Silver;
        }
    }
}