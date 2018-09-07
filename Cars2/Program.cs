using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars2
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            //query syntax Join
            var query = from car in cars
                        join manufacturer in manufacturers
                           on new { car.Manufacturer, car.Year }
                           equals
                           new { Manufacturer = manufacturer.Name, manufacturer.Year }
                        orderby car.Combined descending, car.Model  //per kableli rasomi antriniai rusevimo budai.
                        select new
                        {
                            manufacturer.Headquarters,
                            car.Model,
                            car.Combined
                        };

            //Method Syntax Join
            var query2 = cars.Join(manufacturers,
                                    c => new { c.Manufacturer, c.Year },
                                    m => new { Manufacturer = m.Name, m.Year },
                                    (c, m) => new
                                    {
                                        m.Headquarters,
                                        c.Model,
                                        c.Combined
                                    })
                              .OrderByDescending(c => c.Combined)
                              .ThenBy(c => c.Model);

            //query syntax group
            var query3 =
                from car in cars
                group car by car.Manufacturer.ToUpper() into m
                orderby m.Key
                select m;


            //extension method syntax group
            var query4 = cars.GroupBy(c => c.Manufacturer.ToUpper())
                             .OrderBy(g => g.Key);


            //query syntax GroupJoin
            var query5 =
                from manufaturer in manufacturers
                join car in cars on manufaturer.Name equals car.Manufacturer
                    into carGroup
                orderby manufaturer.Name
                select new
                {
                    Manufacturer = manufaturer,
                    Cars = carGroup
                };

            // extension method syntax groupJoin
            var query6 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m,g) =>
                    new
                    {
                        Manufacturer = m,
                        Cars = g
                    })
                .OrderBy(m => m.Manufacturer.Name);

            //excesice top3 fuel effician cars by country

            var query7 = from manufaturer in manufacturers
                         join car in cars on manufaturer.Name equals car.Manufacturer
                         into CountryGroup
                         orderby manufaturer.Headquarters
                         select new
                         {
                             Manufacturer = manufaturer,
                             Cars = CountryGroup
                         } into result
                         group result by result.Manufacturer.Headquarters;


            var query8 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m, g) =>
                    new
                    {
                        Manufacturer = m,
                        Cars = g
                    })
                .OrderBy(c=>c.Manufacturer.Headquarters)
                .GroupBy(m => m.Manufacturer.Headquarters);


            //max min avg

            var query9 = from car in cars
                         group car by car.Manufacturer
                         into carGroup
                         select new
                         {
                             Name = carGroup.Key,
                             Max = carGroup.Max(c=>c.Combined),
                             Min = carGroup.Min(c => c.Combined),
                             Avg = carGroup.Average(c => c.Combined)
                         } into result
                         orderby result.Max descending
                         select result;

            var query10 = cars.GroupBy(c => c.Manufacturer)
                              .Select(g =>
                              {
                                  var result = g.Aggregate( new CarStatistics(), 
                                                            (acc, c) => acc.Accumulate(c),
                                                            acc => acc.Compute());

                                  return new
                                  {
                                      Name = g.Key,
                                      Avg = result.Avg,
                                      Max = result.Max,
                                      Min = result.Min
                                  };
                              })
                              .OrderByDescending(r=> r.Max);

            foreach (var group in query10)
            {
                Console.WriteLine($"{group.Name}");
                Console.WriteLine($"\t Min : {group.Min}");
                Console.WriteLine($"\t Max : {group.Max}");
                Console.WriteLine($"\t Average : {group.Avg}");

            }





            foreach (var group in query8)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var car in group.SelectMany(g => g.Cars)
                                         .OrderByDescending(c => c.Combined)
                                         .Take(3))
                {
                    Console.WriteLine($"\t{car.Model} takes {car.Combined} mpg");
                }
            }



           /* foreach (var group in query4)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var car in group.OrderByDescending(c=> c.Combined)
                                         .Take(2))
                {
                    Console.WriteLine($"\t{car.Model} takes {car.Combined} mpg");
                }
            }*/
        }


        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                                        .Where(line => line.Length > 1)
                                        .Select(l =>
                                        {
                                            var columns = l.Split(',');
                                            return new Manufacturer()
                                            {
                                                Name = columns[0],
                                                Headquarters = columns[1],
                                                Year = int.Parse(columns[2])
                                            };
                                        });


            return query.ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            var carList = File.ReadAllLines(path)
                 .Skip(1)        //praleis header
                 .Where(line => line.Length > 1) //neims eiluciu kurios maziau 1
                 .ToCar();

            return carList.ToList();
        }
    }

    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
            
        }
        

        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);

            return this;
        }

        public CarStatistics Compute()
        {
            Avg = Total / Count;
            return this;
        }

        public int Max { get; set; }
        public int Min { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public double Avg { get; set; }
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
