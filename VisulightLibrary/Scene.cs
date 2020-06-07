using System.Collections.Generic;

namespace VisulightLibrary
{
	public class Scene
	{
		public string Name { get; set; }

		public CelestialObject ObjectA { get; set; }

		public CelestialObject ObjectB { get; set; }

		private double distance;
		/// <summary>
		/// La distance qui sépare les deux objets célestes en kilomètres.
		/// Valeur minimum : 36 000 km
		/// Valeur maximale : 320 000 000 000 000 km
		/// </summary>
		public double Distance
		{
			get
			{
				return distance;
			}
			set
			{
				if (value < 36_000)
				{
					distance = 36_000;
				}
				else if (value > 320_000_000_000_000)
				{
					distance = 320_000_000_000_000;
				}
				else
				{
					distance = value;
				}
			}
		}

		public Photon Photon { get; set; }


		/// <summary>
		/// Récupère le temps d'un aller pour le photon depuis l'objet
		/// céleste A vers l'objet céleste B.
		/// </summary>
		/// <returns>
		/// Une chaîne de caractère indiquant le temps d'un aller pour
		/// le photon de A à B au format années, jours, heures, minutes,
		/// secondes, millisecondes.
		/// </returns>
		public string GetTimeString()
		{
			ulong millis = (ulong)(Distance / Photon.Speed * 1000);
			ulong seconds = millis / 1000;
			millis -= seconds * 1000;
			ulong minutes = seconds / 60;
			seconds -= minutes * 60;
			ulong hours = minutes / 60;
			minutes -= hours * 60;
			ulong days = hours / 24;
			hours -= days * 24;
			ulong years = days / 365;
			days -= years * 365;

			string yearsString = $"{years:N0} {(years == 1 ? "an" : "ans")}";
			string daysString = $"{days} jours";
			string timeString = $"{hours:00}:{minutes:00}:{seconds:00}.{millis:000}";

			if (years > 0)
			{
				return $"Temps : {yearsString}, {daysString}, {timeString}";
			}
			else if (days > 0)
			{
				return $"Temps : {daysString}, {timeString}";
			}
			else
			{
				return $"Temps : {timeString}";
			}
		}

		/// <summary>
		/// Récupère la liste des scènes par défaut.
		/// </summary>
		/// <returns>
		/// Une liste d'objet Scene représentant la liste des scènes par défaut.
		/// </returns>
		public static List<Scene> GetDefaultScenes()
		{
			List<Scene> scenes = new List<Scene>
			{
				new Scene
				{
					Name = "Terre-Lune",
					ObjectA = new CelestialObject
					{
						Name = "Terre",
						Image = "Earth",
						Width = 45,
						Height = 45
					},
					ObjectB = new CelestialObject
					{
						Name = "Lune",
						Image = "Moon",
						Width = 16,
						Height = 16
					},
					Distance = 384_400,
					Photon = new Photon
					{
						Width = 16
					}
				},

				new Scene
				{
					Name = "Terre-Satellite géostationnaire",
					ObjectA = new CelestialObject
					{
						Name = "Terre",
						Image = "Earth",
						Width = 60,
						Height = 60
					},
					ObjectB = new CelestialObject
					{
						Name = "Satellite",
						Image = "Satellite",
						Width = 25,
						Height = 60
					},
					Distance = 36_000,
					Photon = new Photon
					{
						Width = 25
					}
				},

				new Scene
				{
					Name = "Terre-Mars (proche)",
					ObjectA = new CelestialObject
					{
						Name = "Terre",
						Image = "Earth",
						Width = 17,
						Height = 17
					},
					ObjectB = new CelestialObject
					{
						Name = "Mars",
						Image = "Mars",
						Width = 14,
						Height = 14
					},
					Distance = 55_700_000,
					Photon = new Photon
					{
						Width = 10
					}
				},

				new Scene
				{
					Name = "Terre-Mars (éloigné)",
					ObjectA = new CelestialObject
					{
						Name = "Terre",
						Image = "Earth",
						Width = 13,
						Height = 13
					},
					ObjectB = new CelestialObject
					{
						Name = "Mars",
						Image = "Mars",
						Width = 10,
						Height = 10
					},
					Distance = 401_300_000,
					Photon = new Photon
					{
						Width = 7
					}
				},

				new Scene
				{
					Name = "Soleil-Terre",
					ObjectA = new CelestialObject
					{
						Name = "Soleil",
						Image = "Sun",
						Width = 120,
						Height = 120
					},
					ObjectB = new CelestialObject
					{
						Name = "Terre",
						Image = "Earth",
						Width = 8,
						Height = 8
					},
					Distance = 149_600_000,
					Photon = new Photon
					{
						Width = 5
					}
				}
			};

			return scenes;
		}
	}
}
