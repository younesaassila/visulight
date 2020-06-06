using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Visulight.Core
{
	public class Preset
	{
		public string name; // C'est le nom qui apparaîtra dans la liste déroulante

		public string pointAName;
		public Image pointAImage;
		public int pointAWidth; // En pixels
		public int pointAHeight; // En pixels

		public string pointBName;
		public System.Drawing.Image pointBImage;
		public int pointBWidth; // En pixels
		public int pointBHeight; // En pixels

		public double distance; // En kilomètres
		public int lightWidth; // En pixels

		public Preset(string _name, string _pointAName, Image _pointAImage, int _pointAWidth, int _pointAHeight,
									string _pointBName, Image _pointBImage, int _pointBWidth, int _pointBHeight,
									double _distance, int _lightWidth)
		{
			name = _name;

			pointAName = _pointAName;
			pointAImage = _pointAImage;
			pointAWidth = _pointAWidth;
			pointAHeight = _pointAHeight;

			pointBName = _pointBName;
			pointBImage = _pointBImage;
			pointBWidth = _pointBWidth;
			pointBHeight = _pointBHeight;

			distance = _distance;
			lightWidth = _lightWidth;
		}
	}
}
