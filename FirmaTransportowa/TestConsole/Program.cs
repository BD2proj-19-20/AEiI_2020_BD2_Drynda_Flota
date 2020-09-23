using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole {
	class Program {
		static void showCarModels() {
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			var models = db.CarModels;
			foreach (var k in models) {
				Console.WriteLine($"Id: {k.id}, Marka: {k.make}, model: {k.model}");
			}
		}
		static void addCarModels() {
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			var model = new CarModel();
			model.make = "Skoda";
			model.model = "Octavia";
			db.CarModels.Add(model);
			model = new CarModel();
			model.make = "Ford";
			model.model = "Focud";
			db.CarModels.Add(model);
			model = new CarModel();
			model.make = "Audi";
			model.model = "A8";
			db.CarModels.Add(model);
			model = new CarModel();
			model.make = "Volkswagen";
			model.model = "Passat";
			db.CarModels.Add(model);
			model = new CarModel();
			model.make = "Ford";
			model.model = "Mondeo";
			db.CarModels.Add(model);
			db.SaveChanges();
		}
		static void updateCarModels() {
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			var models = db.CarModels;
			foreach (var k in models) {
				k.make = "Modified";
				k.model = "Record";
			}
			db.SaveChanges();
		}
		static void deleteCarModels() {
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			var models = db.CarModels;
			foreach (var k in models) {
				db.CarModels.Remove(k);
			}
			db.SaveChanges();
		}
		static void Main(string[] args) {
			Console.WriteLine("Pierwsze wypisanie:");
			showCarModels();
			Console.WriteLine("Dodawanie:");
			addCarModels();
			Console.WriteLine("Drugie wypisanie:");
			showCarModels();
			Console.WriteLine("Aktualizacja:");
			updateCarModels();
			Console.WriteLine("Trzecie wypisanie:");
			showCarModels();
			Console.WriteLine("Usuwanie:"); //mamy taki rodzaj bazy że prawdopodobnie nigdy nie będziemy niczego usuwać, ale dla zasady tu jest
			deleteCarModels();
			Console.WriteLine("Czwarte wypisanie:");
			showCarModels();
			Console.WriteLine("Koniec");

			Console.ReadKey();
		}
	}
}
