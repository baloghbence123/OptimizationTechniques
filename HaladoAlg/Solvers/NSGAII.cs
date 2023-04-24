using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HaladoAlg.Solvers
{
    public class NSGAII
    {

        public static List<Solvers.Person> NSGAIIMethod(List<Solvers.Person> population, int maxGeneration = 50)
        {
            List<Solvers.Person> people = population.ToList();
            List<Solvers.Person> children = new List<Solvers.Person>();
            List<Solvers.Person> bestRet = new List<Solvers.Person>();

            int generationCtr = 0;
            while (generationCtr < maxGeneration)
            {
                ;
                people = Selection(people, children);
                ;
                bestRet = people.Where(t => t.rank == 0).ToList();
                ;
                children = MakeNewPopulation(people);
                ;

                generationCtr++;
            }
            ;
            return people;
        }

        private static List<Solvers.Person> MakeNewPopulation(List<Solvers.Person> parents)
        {
            var tmpList = parents.OrderBy(t => Guid.NewGuid()).ToList();

            List<Solvers.Person> children = new List<Solvers.Person>();

            while (children.Count<Person.PopulationSize)
            {
                children.Add(new Solvers.Person(Crossover(tmpList[Utils.rnd.Next(tmpList.Count)], tmpList[Utils.rnd.Next(tmpList.Count)] ).Properties));
            }
            ;
            return children;



        }
        //mutáció= a szülő meg b szülő átlagos értékeit kapja
        private static Solvers.Person Crossover(Solvers.Person a, Solvers.Person b)
        {
            Solvers.Person childOne = new Solvers.Person();
            for (int i = 0; i < Solvers.Person.PropCount; i++)
            {
                childOne.Properties[i] = (a.Properties[i] + b.Properties[i]) / 2;
            }
            return childOne;
        }


        private static bool Dominate(Solvers.Person a, Solvers.Person b)
        {
            bool atLeastOneBetter = false;
            for (int i = 0; i < Solvers.Person.PropCount; i++)
            {
                if (a.Properties[i] < b.Properties[i])
                    return false;
                else if (a.Properties[i] > b.Properties[i])
                    atLeastOneBetter = true;
            }
            return atLeastOneBetter;
        }
        public static void NonDominatedAlfa(List<Solvers.Person> population)
        {
            List<Solvers.Person> F = new List<Solvers.Person>();

            List<int> n = new List<int>();
            List<List<int>> S = new List<List<int>>();
            foreach (var item in population)
            {
                n.Add(0);
                S.Add(new List<int>());
            }
            for (int p = 0; p < population.Count; p++)
            {
                for (int q = 0; q < population.Count; q++)
                {
                    if (Dominate(population[p], population[q]))
                    {
                        n[p] += 1;
                        S[q].Add(p);
                    }

                }
                if (n[p]==0)
                {
                    F.Add(population[p]);
                }
            }

            int pfi = 0;
            while (F.Count != 0)
            {
                List<Solvers.Person> Fb = new List<Solvers.Person>();
                for (int p = 0; p < F.Count; p++)
                {
                    F[p].rank = pfi;
                    
                    for (int q = 0; q < S[p].Count; q++)
                    {
                        n[q] -= 1;
                        if (n[q] == 0)
                        {
                            Fb.Add(population[S[p][q]]);
                        }
                    }
                }
                pfi += 1;
                F= Fb;
            }
            ;
            
        }


        //nem jó a nondominated
        public static List<List<Person>> NonDominatedSort(List<Person> population)
        {

            var frontList = new List<List<Person>>();

            int[] cntDominated = new int[population.Count];
            List<List<int>> listDominate = new List<List<int>>();
            for (int i = 0; i < population.Count; ++i) listDominate.Add(new List<int>());

            for (int i = 0; i < population.Count; ++i)
            {
                for (int j = 0; j < population.Count; ++j)
                {
                    if (Dominate(population[i], population[j]))
                    {
                        cntDominated[j] += 1;
                        listDominate[i].Add(j);
                    }
                }
            }
            int frontCtr = 0;
            bool running = true;
            while (running)
            {
                running = false;
                List<int> currentFront = new List<int>();

                for (int i = 0; i < population.Count; ++i)
                {
                    if (cntDominated[i] == 0)
                    {
                        running = true;

                        cntDominated[i] = -1;
                        currentFront.Add(i);

                    }
                }

                List<Person> Add = new List<Person>();
                foreach (int i in currentFront)
                {
                    Add.Add(population[i]);
                    population[i].rank = frontCtr;

                    foreach (var j in listDominate[i])
                    {
                        cntDominated[j] -= 1;
                    }
                }
                frontCtr++;

                frontList.Add(Add);

            }

            return frontList;
        }




        private static void CalculateCrowdingDistance(List<Solvers.Person> population)
        {
            int n = population.Count;
            for (int i = 0; i < n; i++)
            {
                population[i].CrowdingDistance = 0.0f;
            }

            for (int m = 0; m < Solvers.Person.PropCount; m++)
            {
                population = population.OrderBy(t => t.Properties[m]).ToList();

                population[0].CrowdingDistance = float.PositiveInfinity;
                population[n - 1].CrowdingDistance = float.PositiveInfinity;

                double minObjective = population[0].Properties[m];
                double maxObjective = population[n - 1].Properties[m];
                ;

                for (int i = 1; i < n - 1; i++)
                {
                    double distance = population[i + 1].Properties[m] - population[i - 1].Properties[m];
                    distance /= maxObjective - minObjective;
                    population[i].CrowdingDistance += (float)distance;
                }
            }
        }


        private static List<Person> Selection(List<Person> population,List<Person> children)
        {
            List<Solvers.Person> newPop = new List<Solvers.Person>();

            List<Solvers.Person> fullPop = new List<Solvers.Person>();
            fullPop.AddRange(population.ToList());
            fullPop.AddRange(children.ToList());

            var asd = NonDominatedSort(fullPop);
            ;

            int rankWanted = 0;
            while (newPop.Count < Solvers.Person.PopulationSize)
            {
                List<Solvers.Person> tmpAddList = fullPop.Where(t => t.rank == rankWanted).ToList();
                if ((newPop.Count + tmpAddList.Count) <= Solvers.Person.PopulationSize)
                {
                    newPop.AddRange(tmpAddList);
                    ;
                }
                else
                {
                    CalculateCrowdingDistance(tmpAddList);
                    ;
                    tmpAddList = tmpAddList.OrderByDescending(t => t.CrowdingDistance).ToList();
                    int numberNeeded = (Solvers.Person.PopulationSize - newPop.Count);
                    for (int i = 0; i < numberNeeded; i++)
                    {
                        newPop.Add(tmpAddList[i]);
                        ;
                    }
                    ;
                }
                rankWanted++;
                
            
            }
            ;

            return newPop;
        }

    }
    public class Person
    {
        //the data is normalized by default
        //a grafikus ábrázolás miatt, könnyebb, ha a Quality 1-nél a legjobb
        //quality best at 1

        //prop[0] = Salary; prop[1]=Quality
        public float[] Properties { get; set; }

        public float CrowdingDistance = 0;
        public int rank = 0;




        public static int PropCount = 2;
        public static int PopulationSize = 100;



        public Person()
        {
            GenerateValues();

        }
        public Person(float[] props)
        {
            this.Properties = props;
        }

        private void GenerateValues()
        {
            // this is the balance between the 2 props
            this.Properties = new float[PropCount];
            for (int i = 0; i < PropCount; i++)
            {
                this.Properties[i] = (float)Utils.rnd.NextDouble();

            }


        }
    }

}
