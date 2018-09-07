using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Features.MyLinq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;
            //paima iki 16 parametru paskutinis reiskia return type
            Func<int, int, int> add2 = (x, y) =>
            {
                int temp = x + y;
                return temp;        //return butinas jei  naudojama {}
            };

            Action<int> writeConsole = x => Console.WriteLine(x);
            
            //Action cisada return void. ir gali paimti iki 16 parametru

            Console.WriteLine(square(3));
            Console.WriteLine(add(3, 8));
            Console.WriteLine(square(add(3, 8)));
            writeConsole(square(5));






            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee() {Name="Scott" },
                new Employee() {Name="Zoran" },
                new Employee() {Name="Aidas" },
                new Employee() {Name="Matt" },
                new Employee() {Name="Linas" },
                new Employee() {Name="IlgasVardas" }
            };


            IEnumerable<Employee> sales = new List<Employee>
            {
                new Employee() {Name="Alex" }
            };

            //_____________Evoliucija Method->Delegate->Lamda_____
            //_____________Method________________________
            foreach (var emp in developers.Where(NameStartsS))
            {               
                    Console.WriteLine(emp.Name);                
            }

            //_____________Delegate________________________
            foreach (var emp in developers.Where(
                delegate(Employee employee)
                {
                    return employee.Name.StartsWith("S");
                }))
            {
                Console.WriteLine(emp.Name);
            }
            //_____________Lambda________________________
            foreach (var emp in developers.Where(e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(emp.Name);
            }
            Console.WriteLine("------------------------------***---------------------");
            foreach (var emp in developers.Where(e => e.Name.Length == 5)
                                            .OrderBy(e => e.Name))
            {
                Console.WriteLine(emp.Name);
            }


            Console.WriteLine("------------------------------***---------------------");
            //method syntax
            var query = developers.Where(e => e.Name.Length == 5)
                                            .OrderBy(e => e.Name)
                                            .Select(e => e);

            //Select(e=>e) nebutinas bet jis reisktu tapati kaip ir Linq
            
            //query sintax
            var query2 = from developer in developers
                        where developer.Name.Length == 5
                        orderby developer.Name
                        select developer;

            foreach (var emp in query)
            {
                Console.WriteLine(emp.Name);
            }


            /*Console.WriteLine(developers.Count());

            IEnumerator<Employee> enumerator = sales.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Name);
            }*/


            //foreach (var person in sales)
            //{
            //    Console.WriteLine(person.Name);
            //}

        }

       private static bool NameStartsS(Employee employee)
        {
            return employee.Name.StartsWith("S");
        }
    }
}
