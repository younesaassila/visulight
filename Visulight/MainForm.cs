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
        private enum LightDirection { PointA, PointB };

        private Simulation simulation;

        private LightDirection lightDirection = LightDirection.PointB;

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
                labelInformation.Text = "Simulation en pause lors du déplacement ou redimensionnement de la fenêtre.";
                labelInformation.ForeColor = Color.LightCoral;
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
            int i = presetComboBox.SelectedIndex;
            
            presetLabelDistance.Text = $"Distance : { string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:#,##0}", presets[i].distance) } km";

            SetSimulation(presets[i].pointAName, presets[i].pointAImage, presets[i].pointAWidth, presets[i].pointAHeight,
                  presets[i].pointBName, presets[i].pointBImage, presets[i].pointBWidth, presets[i].pointBHeight,
                  presets[i].distance, presets[i].lightWidth);
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
                SetPointLabels(customTextBoxPointA.Text, customTextBoxPointB.Text);
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
            SetSimulation(customTextBoxPointA.Text, Properties.Resources.WhitePoint, 10, 10,
                  customTextBoxPointB.Text, Properties.Resources.WhitePoint, 10, 10,
                  (double)customNumeric.Value, 10);
        }


        private void SetSimulation(string pointAName, Image pointAImage, int pointAWidth, int pointAHeight,
                           string pointBName, Image pointBImage, int pointBWidth, int pointBHeight,
                           double distance, int lightWidth)
        {
            if (simulation != null)
            {
                simulation.Stop();
            }
            simulation = new Simulation(distance);

            // On prépare la scène avec les images et la position de la lumière au départ
            SetUI(pointAImage, pointAWidth, pointAHeight,
                        pointBImage, pointBWidth, pointBHeight,
                        lightWidth);

            // On change le texte des labels des points
            SetPointLabels(pointAName, pointBName);


            // On calcule le temps mis par la lumière du point A au point B
            ulong timeMillis = (ulong)(distance / Simulation.speedOfLight * 1000);
            
            // On affiche le temps de déplacement de la lumière entre les deux points
            SetTimeLabel(timeMillis);
        }
        
        private void SetUI(Image aImage, int aWidth, int aHeight,
                                 Image bImage,  int bWidth, int bHeight,
                                 int lightWidth)
        {
            pbPointA.Image = aImage;
            pbPointA.Width = aWidth;
            pbPointA.Height = aHeight;
            pbPointA.Location = new Point(40, (panelSimulation.Height - aHeight) / 2);

            pbPointB.Image = bImage;
            pbPointB.Width = bWidth;
            pbPointB.Height = bHeight;
            pbPointB.Location = new Point(panelSimulation.Width - (40 + bWidth), (panelSimulation.Height - bHeight) / 2);

            light.Width = lightWidth;
            int x = pbPointA.Location.X + aWidth - lightWidth;
            int y = pbPointA.Location.Y + ((aHeight + light.Height) / 2);
            light.Location = new Point(x, y);
        }

        private void SetPointLabels(string _pointAname, string _pointBname)
        {
            // Point A
            lbPointA.Text = _pointAname;

            int xa = pbPointA.Location.X + (pbPointA.Width - lbPointA.Width) / 2;
            int ya = pbPointA.Location.Y + pbPointA.Height + 40;
            lbPointA.Location = new Point(xa, ya);

            // Point B
            lbPointB.Text = _pointBname;

            int xb = pbPointB.Location.X + (pbPointB.Width - lbPointB.Width) / 2;
            int yb = pbPointB.Location.Y + pbPointB.Height + 40;
            lbPointB.Location = new Point(xb, yb);
        }

        private void SetTimeLabel(ulong milliseconds)
        {
            ulong seconds = milliseconds / 1000;
            milliseconds -= seconds * 1000;

            ulong minutes = seconds / 60;
            seconds -= minutes * 60;

            ulong hours = minutes / 60;
            minutes -= hours * 60;

            ulong days = hours / 24;
            hours -= days * 24;

            ulong years = days / 365;
            days -= years * 365;
            
            // Si il y a au moins 1 année
            if (years > 0)
            {
                labelTime.Text = $"Temps : {years} années, {days} jours, {hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}";
            }

            // Sinon, on test s'il y a au moins 1 jour
            else if (days > 0)
            {
                labelTime.Text = $"Temps : {days} jours, {hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}";
            }

            // Si ce n'est pas le cas, alors on affiche le temps qu'à partir des heures
            else
            {
                labelTime.Text = $"Temps : {hours:00}:{minutes:00}:{seconds:00}.{milliseconds:000}";
            }
        }

        
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Visible = false;

            simulation.Start();
            Text = "Visulight ‒ Simulation en cours";

            buttonStop.Visible = true;

            Thread thread = new Thread(RunSimulation);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }

        private void RunSimulation()
        {
            while (simulation.State == Simulation.SimState.RUNNING || simulation.State == Simulation.SimState.PAUSED)
            {
                if (simulation.State == Simulation.SimState.PAUSED)
                {
                    Thread.Sleep(100);
                    continue;
                }

                int delay = 1;

                int distanceInPixels = pbPointB.Location.X - pbPointA.Location.X - pbPointA.Width;
                double ratio = simulation.GetDistanceTraveledRatio();
                int distanceTraveledInPixels = (int)(distanceInPixels * ratio);

                Point newPhotonLocation = lightDirection == LightDirection.PointB ?
                    new Point(pbPointA.Location.X + pbPointA.Width - light.Width + distanceTraveledInPixels, light.Location.Y) :
                    new Point(pbPointB.Location.X + light.Width - distanceTraveledInPixels, light.Location.Y);

                bool rightSideOutOfBound =
                    lightDirection == LightDirection.PointB && newPhotonLocation.X >= pbPointB.Location.X - light.Width;
                bool leftSideOutOfBound =
                    lightDirection == LightDirection.PointA && newPhotonLocation.X <= pbPointA.Location.X + pbPointA.Width;

                if (rightSideOutOfBound || leftSideOutOfBound)
                {
                    lightDirection = lightDirection == LightDirection.PointB ? LightDirection.PointA : LightDirection.PointB;
                    light.BackgroundImage = lightDirection == LightDirection.PointB ?
                        Properties.Resources.LightGoingTowardsPointB :
                        Properties.Resources.LightGoingTowardsPointA;
                    newPhotonLocation = lightDirection == LightDirection.PointB ?
                        new Point(pbPointA.Location.X + pbPointA.Width - light.Width, light.Location.Y) :
                        new Point(pbPointB.Location.X, light.Location.Y);
                    simulation.ResetPhotonDistance();
                    delay = 20;
                }

                light.Invoke(new UpdatePhotonLocationDelegate(UpdatePhotonLocation), newPhotonLocation);
                Thread.Sleep(delay);
            }

            Text = "Visulight";

            buttonStop.Visible = false;

            // On réinitialise la direction puis la position de la lumière
            lightDirection = LightDirection.PointB;
            light.Anchor = AnchorStyles.Left;
            light.BackgroundImage = Properties.Resources.LightGoingTowardsPointB;

            int x = pbPointA.Location.X + pbPointA.Width - light.Width;
            int y = pbPointA.Location.Y + ((pbPointA.Height + light.Height) / 2);
            light.Location = new Point(x, y);

            buttonStart.Visible = true;
            labelInformation.Text = "Cliquez sur 'Démarrer la simulation' pour commencer.";
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
            labelInformation.Text = $"{distanceTraveledInPixels} pixels parcourus ‒ Ratio : {simulation.GetDistanceTraveledRatio()}";
            labelInformation.ForeColor = Color.Silver;
        }
    }
}