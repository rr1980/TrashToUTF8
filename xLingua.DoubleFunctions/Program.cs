using System;
using System.Linq;
using xLingua.Entities;

namespace xLingua.DoubleFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var _context = new DataDbContext())
            {
                var connections = _context.Connections.GroupBy(x => new { x.BaseWordId, x.FunctionId }).Where(x => x.Count() > 1).ToList();

                Console.WriteLine("count: " + connections.Count());
                Console.WriteLine("---------------------------" + Environment.NewLine);

                foreach (var c in connections)
                {
                    var lowestId = c.Min(x => x.Id);

                    Console.WriteLine("count: " + c.Count());
                    Console.WriteLine("min: " + lowestId);
                    Console.WriteLine("=: " + c.First().Id + " - " + c.Last().Id );

                    _context.Connections.Remove(c.FirstOrDefault(x => x.Id == lowestId));

                    Console.WriteLine("---------------------------" + Environment.NewLine);
                }

                _context.SaveChanges();

                Console.WriteLine("Finished");
            }

            Console.WriteLine("EXIT");
            Console.ReadLine();

        }
    }
}
