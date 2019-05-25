using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Visulight
{
    public partial class Form : System.Windows.Forms.Form
    {
        Preset[] presets = new Preset[5];
        
        // Temps en millisecondes du point A au point B
        ulong totalTime; // Remarque : On utilise un ulong, car le temps peut être très long si l'on s'intéresse à des sondes très lointaines
        
        int distance; // En pixels
        int distanceTraveled; // En pixels
        decimal speed;

        enum Direction { PointA, PointB };
        Direction lightDirection = Direction.PointB;


        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // On définit les préréglages
            presets[0] = new Preset("Terre-Lune",
                "Terre", Properties.Resources.Earth, 45, 45,
                "Lune", Properties.Resources.Moon, 16, 16,
                384_400, 16);

            presets[1] = new Preset("Terre-Satellite géostationnaire",
                "Terre", Properties.Resources.Earth, 60, 60,
                "Satellite", Properties.Resources.Satellite, 25, 60,
                36_000, 25);

            presets[2] = new Preset("Terre-Mars (proche)",
                "Terre", Properties.Resources.Earth, 17, 17,
                "Mars", Properties.Resources.Mars, 14, 14,
                55_700_000, 10);

            presets[3] = new Preset("Terre-Mars (éloigné)",
                "Terre", Properties.Resources.Earth, 13, 13,
                "Mars", Properties.Resources.Mars, 10, 10,
                401_300_000, 7);

            presets[4] = new Preset("Soleil-Terre",
                "Soleil", Properties.Resources.Sun, 120, 120,
                "Terre", Properties.Resources.Earth, 8, 8,
                149_600_000, 5);

            // Attention : L'ajout d'un préréglage oblige l'incrémentation de la longueur de presets là où il est déclaré
            
            // On complète la liste des préréglages de la ComboBox
            for (int i = 0; i < presets.Length; i++)
            {
                presetComboBox.Items.Add(presets[i].name);
            }

            // On sélectionne le premier préréglage par défaut
            presetComboBox.SelectedIndex = 0;
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            Light_Location_Speed_Update();
        }

        private void ButtonShow_Click(object sender, EventArgs e)
        {
            buttonShow.Visible = false;
            
            panelSimulation.Location = new Point(panelOptions.Width, 0);
            panelSimulation.Width = panelSimulation.Width - panelOptions.Width;
            panelOptions.Visible = true;

            Light_Location_Speed_Update();
            buttonHide.Visible = true;
        }

        private void ButtonHide_Click(object sender, EventArgs e)
        {
            buttonHide.Visible = false;

            panelOptions.Visible = false;
            panelSimulation.Location = new Point(0, 0);
            panelSimulation.Width = panelOptions.Width + panelSimulation.Width;

            Light_Location_Speed_Update();
            buttonShow.Visible = true;
        }
        
        private void Light_Location_Speed_Update()
        {
            if (Timer.Enabled)
            {
                // On calcule le rapport distance parcourue sur distance totale
                decimal ratio = decimal.Divide(distanceTraveled, distance);

                distance = pbPointB.Location.X - (pbPointA.Location.X + pbPointA.Width);
                distanceTraveled = (int)(distance * ratio);

                // On met à jour la position de la lumière
                if (lightDirection == Direction.PointB)
                {
                    Point newLightPanelLocation = new Point(pbPointA.Location.X + pbPointA.Width - light.Width + distanceTraveled, light.Location.Y);
                    light.Location = newLightPanelLocation;
                }

                else
                {
                    Point newLightPanelLocation = new Point(pbPointB.Location.X - distanceTraveled, light.Location.Y);
                    light.Location = newLightPanelLocation;
                }

                // On met à jour la vitesse
                Calculate_Speed();

                LabelInformation_Update();
            }
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
            
            Setup(presets[i].pointAName, presets[i].pointAImage, presets[i].pointAWidth, presets[i].pointAHeight,
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
                LabelsPoints_Setup(customTextBoxPointA.Text, customTextBoxPointB.Text);
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
            Setup(customTextBoxPointA.Text, Properties.Resources.WhitePoint, 10, 10,
                  customTextBoxPointB.Text, Properties.Resources.WhitePoint, 10, 10,
                  (double)customNumeric.Value, 10);
        }


        private void Setup(string _pointAName, Image _pointAImage, int _pointAWidth, int _pointAHeight,
                           string _pointBName, Image _pointBImage, int _pointBWidth, int _pointBHeight,
                           double _distance, int _lightWidth)
        {
            // On arrête la simulation s'il y en a une en cours d'exécution
            Simulation_Stop();

            // On prépare la scène avec les images et la position de la lumière au départ
            Scene_Setup(_pointAImage, _pointAWidth, _pointAHeight,
                        _pointBImage, _pointBWidth, _pointBHeight,
                        _lightWidth);

            // On change le texte des labels des points
            LabelsPoints_Setup(_pointAName, _pointBName);

            // Vitesse de la lumière dans le vide en km/s
            const double speedOfLight = 299_792.458;

            // On calcule le temps mis par la lumière du point A au point B
            totalTime = (ulong)(_distance / (speedOfLight / 1000)); // En millisecondes
            
            // On affiche le temps de déplacement de la lumière entre les deux points
            LabelTime_Setup(totalTime);
        }
        
        private void Scene_Setup(Image aImage, int aWidth, int aHeight,
                                 Image bImage,  int bWidth, int bHeight,
                                 int lightWidth)
        {
            // ----- PictureBoxPointA -----
            // On change l'image de PictureBoxPointA.
            pbPointA.Image = aImage;

            // On change les dimensions de PictureBoxPointA.
            pbPointA.Width = aWidth;
            pbPointA.Height = aHeight;

            // On centre la PictureBoxPointA.
            pbPointA.Location = new Point(40, (panelSimulation.Height - aHeight) / 2);


            // ----- PictureBoxPointB -----
            // On change l'image de PictureBoxPointB.
            pbPointB.Image = bImage;

            // On change les dimensions de PictureBoxPointB.
            pbPointB.Width = bWidth;
            pbPointB.Height = bHeight;
            
            // On centre la PictureBoxPointB.
            pbPointB.Location = new Point(panelSimulation.Width - (40 + bWidth), (panelSimulation.Height - bHeight) / 2);
            

            // Light Panel
            light.Width = lightWidth;
            int x = pbPointA.Location.X + aWidth - lightWidth;
            int y = pbPointA.Location.Y + ((aHeight + light.Height) / 2);
            light.Location = new Point(x, y);
        }

        private void LabelsPoints_Setup(string _pointAname, string _pointBname)
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

        private void LabelTime_Setup(ulong milliseconds)
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
                labelTime.Text = $"Temps : {years.ToString()} années, {string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:#,##0}", days)} jours, {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}.{milliseconds.ToString("000")}";
            }

            // Sinon, on test s'il y a au moins 1 jour
            else if (days > 0)
            {
                labelTime.Text = $"Temps : {string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:#,##0}", days)} jours, {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}.{milliseconds.ToString("000")}";
                
            }

            // Si ce n'est pas le cas, alors on affiche le temps qu'à partir des heures
            else
            {
                labelTime.Text = $"Temps : {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}.{milliseconds.ToString("000")}";
            }
        }
        
        private void Calculate_Speed()
        {
            // On calcule la distance à parcourir, car cette fonction peut être appelé en cours de simulation
            int distanceToTravel = distance - distanceTraveled;

            // Le temps mis pour parcourir la distance depuis la position actuelle jusqu'à la fin
            ulong time = (ulong)distanceToTravel * totalTime / (ulong)distance;

            // Intervalle par défaut du Timer, en millisecondes
            int defaultTimerInterval = 100;

            // On calcule la vitesse ( vitesse = distance / temps )
            if (time > 0)
                speed = decimal.Divide(defaultTimerInterval * distanceToTravel, time);

            else
                return;

            // Si la vitesse de déplacement est de moins d'un pixel toutes les 100 millisecondes
            if (speed > 0 && speed < 1)
            {
                // On calcule l'intervalle à laquelle le Timer doit tourner pour que la vitesse soit de 1 pixel
                decimal interval = 1 * defaultTimerInterval / speed; // En millisecondes
                Timer.Interval = (int)interval;

                speed = 1;
            }

            // Si ce n'est pas le cas, on définit l'intervalle du Timer à l'intervalle par défaut
            else if (speed >= 1)
            {
                Timer.Interval = defaultTimerInterval;
                speed = (int)speed;
            }
        }

        
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Visible = false;

            // On calcule la distance en pixels entre les deux points
            distance = pbPointB.Location.X - (pbPointA.Location.X + pbPointA.Width);

            // On calcule la vitesse de déplacement horizontal
            Calculate_Speed();

            // On démarre le Timer
            Timer.Start();
            Text = "Visulight ‒ Simulation en cours";
            LabelInformation_Update();

            buttonStop.Visible = true;
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            // On appelle la fonction qui arrête la simulation et réinitialise la position de la lumière
            Simulation_Stop();
        }

        private void Simulation_Stop()
        {
            if (Timer.Enabled)
            {
                // On arrête le Timer
                Timer.Stop();

                // On réinitialise le texte affiché dans la barre de titre
                Text = "Visulight";

                buttonStop.Visible = false;

                // On réinitialise la direction puis la position de la lumière
                lightDirection = Direction.PointB;
                light.Anchor = AnchorStyles.Left;
                light.BackgroundImage = Properties.Resources.LightGoingTowardsPointB;

                int x = pbPointA.Location.X + pbPointA.Width - light.Width;
                int y = pbPointA.Location.Y + ((pbPointA.Height + light.Height) / 2);
                light.Location = new Point(x, y);

                // On set la distance parcourue en pixels à 0
                distanceTraveled = 0;
                
                buttonStart.Visible = true;
                labelInformation.Text = "Cliquez sur 'Démarrer la simulation' pour commencer.";
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            // Si la lumière se dirige vers le point B
            if (lightDirection == Direction.PointB)
            {
                int nextLightPanelLocationInX = light.Location.X + (int)speed + light.Width ;

                if (nextLightPanelLocationInX <= pbPointB.Location.X)
                {
                    Point lightPanelLocation = new Point(light.Location.X + (int)speed, light.Location.Y);
                    light.Location = lightPanelLocation;

                    distanceTraveled += (int)speed;
                }

                else if (nextLightPanelLocationInX > pbPointB.Location.X)
                {
                    lightDirection = Direction.PointA;
                    light.BackgroundImage = Properties.Resources.LightGoingTowardsPointA;
                    light.Anchor = AnchorStyles.Right;

                    Point lightPanelLocation = new Point(pbPointB.Location.X, pbPointB.Location.Y + ((pbPointB.Height + light.Height) / 2));
                    light.Location = lightPanelLocation;

                    distanceTraveled = 0;
                    Calculate_Speed();
                }
            }

            // Sinon, si la lumière se dirige vers le point A
            else
            {
                int nextLightPanelLocationInX = light.Location.X - (int)speed;

                if (nextLightPanelLocationInX >= (pbPointA.Location.X + pbPointA.Width))
                {
                    Point lightPanelLocation = new Point(light.Location.X - (int)speed, light.Location.Y);
                    light.Location = lightPanelLocation;

                    distanceTraveled += (int)speed;
                }

                else if (nextLightPanelLocationInX < (pbPointA.Location.X + pbPointA.Width))
                {
                    lightDirection = Direction.PointB;
                    light.BackgroundImage = Properties.Resources.LightGoingTowardsPointB;
                    light.Anchor = AnchorStyles.Left;

                    Point lightPanelLocation = new Point(pbPointA.Location.X + pbPointA.Width - light.Width, pbPointA.Location.Y + ((pbPointA.Height + light.Height) / 2));
                    light.Location = lightPanelLocation;

                    distanceTraveled = 0;
                    Calculate_Speed();
                }
            }
            
            // On met à jour les coordonnées affichées dans labelInformation
            LabelInformation_Update();
        }

        private void LabelInformation_Update()
        {
            labelInformation.Text = $"Vitesse de {speed} px/{string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:#,##0}", Timer.Interval)} ms ‒ {distanceTraveled}/{distance} pixels parcourus";
        }
    }
}