using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFlie("fuel.csv");

            var query1 = cars.OrderByDescending(car => car.Combined)
                            .ThenBy(car => car.Model); //antrinis rusevimas


            //tas pats in query syntax
            var query2 = from car in cars
                         where car.Manufacturer == "BMW" && car.Year == 2016
                         orderby car.Combined descending, car.Model  //per kamleli rasomi antriniai rusevimo budai.
                         select new
                         {
                            car.Manufacturer,
                            car.Model,
                            car.Year,
                            car.Combined
                         };


            var anonObj = new
            {
                name = "Aidas"
            };

            Console.WriteLine(anonObj.name);

            var top = cars.Where(c => c.Manufacturer == "Subaru" && c.Year == 2016)
                             .OrderByDescending(c => c.Combined)
                             .ThenBy(c => c.Model)
                             .FirstOrDefault(); //first returnins jau ne list o car
                                                // dar yra Last(), FirstOrDefault(), LastOrDefault()
                                                //defaut nemes runtime error jei neras car pagal criterijus ir prilygins null
                                                //jie taip pat priima lamda ir taikim is surusiuoto paims pirma ar paskutini

            var result = cars.Any<Car>(c => c.Manufacturer=="Ford");
            var result1 = cars.All(c => c.Manufacturer == "Ford");
            var result3 = cars.Contains(cars[1]);

            string vardas = "Aidas";
            IEnumerable<Char> characters = "Aidas";


            var result4 = cars.SelectMany(c => c.Model).OrderBy(c => c);
            //SalectMany(c => c.model) paims visu modeliu
            //vardus ir tuos vardus isdalins i seka simboliu 
            //kurie sudaro tuos vardus. nereikes loop in loop

            foreach (var character in result4)
            {
                    Console.WriteLine(character);
            }
            
            


           // Console.WriteLine(result3);

            if (top!=null)
            {
               // Console.WriteLine(top.Model);
            }
            


            //foreach (var car in query2.Take(10))
            //{
            //    Console.WriteLine($"{car.Manufacturer} {car.Model} : {car.Combined} mpg");
            //}
        }



        private static List<Car> ProcessFlie(string path)
        {
            /*var carList = File.ReadAllLines(path)
                .Skip(1)        //praleis header
                .Where(line => line.Length > 1) //neims eiluciu kurios maziau 1
                .Select(Car.ParseFromCsv).ToList();*/

            var carList3 = File.ReadAllLines(path)
                .Skip(1)        //praleis header
                .Where(line => line.Length > 1) //neims eiluciu kurios maziau 1
                .ToCar();

            /*var carList2 = from line in File.ReadAllLines(path).Skip(1)
                           where line.Length > 0
                           select Car.ParseFromCsv(line);*/


            return carList3.ToList();
        }        
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var columns = line.Split(',');

                yield return new Car()
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Model = columns[2],
                    Dispalacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7]),
                };
            }
        }
    }
}
