using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = MyLinq.Random().Where(n => n > 0.5).Take(10);
            foreach (var number in numbers)
            {
                Console.WriteLine($"Skaiciu: {number}");
            }


            var movies = new List<Movie>()
            {
                new Movie() {Title = "Inception", Rating = 9.1f, Year = 2009 },
                new Movie() {Title = "The King's Speach", Rating = 8.0f, Year = 2004 },
                new Movie() {Title = "Ekskursante", Rating = 8.5f, Year = 2014 },
                new Movie() {Title = "Casablanka", Rating = 8.5f, Year = 1942 },
            };


            var query = movies.Filter(m => m.Year > 2000)
                              .OrderByDescending(m => m.Rating);

            var query2 = from movie in movies
                         where movie.Year > 2000
                         orderby movie.Rating descending
                         select movie;


            var enumerator = query2.GetEnumerator();

            //Console.WriteLine(query.Count());
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Title);
            }


            //foreach (var movie in query)
            //{
            //    Console.WriteLine(movie.Title);
            //}

        }
    }
}
