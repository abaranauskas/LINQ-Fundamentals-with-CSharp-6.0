using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car3
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateXml();
            QueryXml();

        }

        private static void QueryXml()
        {
            var ns = (XNamespace)"http://plularsight.com/cars/2016";
            var ex = (XNamespace)"http://plularsight.com/cars/2016/ex";

            var document = XDocument.Load("fuel.xml");

                                              //.Descendants("Car")   //galima butu prieit ir tokiu budu                       
            /*var query = from element in document.Element("Cars").Elements("Car")
                        where element.Attribute("Make")?.Value == "BMW"   //? pries Make sugrazins null jei nebus tokio atributo nebus exception
                        select element.Attribute("Name").Value;*/

            //su namespaces
            var query2 = from element in document.Element(ns + "Cars")?.Elements(ex +"Car") 
                                                                      ?? Enumerable.Empty<XElement>()
                        where element.Attribute("Make")?.Value == "BMW"   //? pries Make sugrazins null jei nebus tokio atributo nebus exception
                        select element.Attribute("Name").Value;

            foreach (var model  in query2)
            {
                Console.WriteLine(model);
            } 
        }

        private static void CreateXml()
        {
            var records = ProcessCars("fuel.csv");

            var ns = (XNamespace)"http://plularsight.com/cars/2016";
            var ex = (XNamespace)"http://plularsight.com/cars/2016/ex";


            var document = new XDocument();
            var cars = new XElement(ns+"Cars");

            var elements =
                from record in records
                select new XElement(ex+"Car",
                                new XAttribute("Name", record.Model),
                                new XAttribute("Combined", record.Combined),
                                new XAttribute("Make", record.Manufacturer)
                );

            cars.Add(elements); //galima taip arba iskart i cars
            cars.Add(new XAttribute(XNamespace.Xmlns + "ex", ex));
            /*var cars = new XElement("Cars",
                                from record in records
                                select new XElement("Car",
                                                new XAttribute("Name", record.Model),
                                                new XAttribute("Combined", record.Combined),
                                                new XElement("Make", record.Manufacturer))
                                );*/


            foreach (var record in records)
            {

                var car = new XElement("Car",
                                new XAttribute("Name", record.Model),
                                new XAttribute("Combined", record.Combined),
                                new XAttribute("Make", record.Manufacturer)
                       );

                //cars.Add(car);
            }

            foreach (var record in records)
            {
                //var car = new XElement("Car");
                var name = new XAttribute("Name", record.Model);
                var combined = new XAttribute("Combined", record.Combined);
                //car.Add(name);
                //car.Add(combined);

                var car = new XElement("Car", name, combined);

                //cars.Add(car);
            }

            document.Add(cars);
            document.Save("fuel.xml");
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
