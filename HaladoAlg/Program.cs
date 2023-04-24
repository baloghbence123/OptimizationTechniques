using HaladoAlg.Problems;
using HaladoAlg.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaladoAlg
{
    public class Program
    {
        static void Main(string[] args)
        {
            //TravellingSalesmanProblem t1 = new TravellingSalesmanProblem(500,500);
            //t1.CreateInitialTowns(1000);

            //GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(200, 0.1f, t1.towns);
            //geneticAlgorithm.Run(false);

            ;
            //List<Person> p = new List<Person>();
            //p.AddRange(Enumerable.Range(0,Person.PopulationSize).Select(t=>new Person()).ToList());
            //;

            //var tmp = NSGAII.NSGAIIMethod(p,2);
            //;
            //for (int j = 0; j <   10000; j++)
            //{
            //    for (int i = 0; i < tmp.Count; i++)
            //    {
            //        Console.WriteLine(i + " -> " + tmp[i].Properties[0] + " \t" + tmp[i].Properties[1]);
            //    }
            //    Thread.Sleep(100);
            //    tmp = NSGAII.NSGAIIMethod(tmp, 2);
            //    Console.Clear();
            //}

            SmallestBWSimulatedA test1 = new SmallestBWSimulatedA();
            test1.AddPolygon(0, 0);
            test1.AddPolygon(10, 0);
            test1.AddConstant(5, 5);
            var tmp = test1.outerDistanceToBoundary(test1.polygon);
            ;
            Console.ReadLine();


            ;



            Console.ReadLine();
        }
    }
}
