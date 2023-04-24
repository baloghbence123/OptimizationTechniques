using HaladoAlg.Problems;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaladoAlg.Solvers
{
    public class GeneticAlgorithm
    {

        int populationSize;
        float mutationRate = 0.1f;
        public int generationCounter = 0;
        

        public Gene bestGene=null;
        public int bestGenesGeneration = -1;

        private Town[] TownSample;
        private Gene[] genes;

        public GeneticAlgorithm(int populationSize, float mutationRate, List<Town> townSample)
        {
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            TownSample = townSample.ToArray();
            InitGenes();
        }
        private void InitGenes()
        {
            this.genes = new Gene[populationSize];
            //pop size needed
            Parallel.For(0, populationSize, i =>
            {
                this.genes[i] = new Gene(TownSample.Length, false);

            });
        }
        public void GetAllFitness()
        {
            for (int i = 0; i < genes.Length; i++)
            {
                Gene tmp = genes[i];
                for (int j = 0; j < tmp.dns.Length-1; j++)
                {
                    tmp.fitness += TravellingSalesmanProblem.GetDistance(
                        GetTownById(tmp.dns[j]), GetTownById(tmp.dns[j + 1])
                        );
                }
                genes[i].fitness = tmp.fitness;
            }
            genes = genes.OrderBy(t => t.fitness).ToArray();
            if (bestGene==null)
            {
                bestGene = Gene.DeepCopy(genes[0]);
                bestGenesGeneration = generationCounter;
            }
            else
            {
                if (bestGene.fitness > genes[0].fitness)
                {
                    
                    bestGene = Gene.DeepCopy(genes[0]);
                    bestGenesGeneration = generationCounter;
                }
            }
            ;
        }
        private Town GetTownById(int Id)
        {
            return TownSample.FirstOrDefault(x => x.Id == Id);
        }
        public void PopMutation()
        {
            Parallel.ForEach(genes, gene =>
            {
                while (StaticRandom.RandFloat() < (float)mutationRate)
                {
                    int rnd1 = StaticRandom.Rand(0, gene.dns.Length);
                    int rnd2 = StaticRandom.Rand(0, gene.dns.Length);

                    var tmp = gene.dns[rnd1];
                    gene.dns[rnd1] = gene.dns[rnd2];
                    gene.dns[rnd2] = tmp;
                }
            });

        }
        //its not a crossover. Its a bigger mutation and mainly repopping 
        //not used
        public void PopCrossOverWithFillRnd()
        {
            int popHalf = genes.Length / 2;
            for (int i = 0; i < popHalf; i++)
            {
                genes[popHalf + i] = FillGene(genes[i]);
            }
            FitnessReset();
        }
        private Gene FillGene(Gene inpGene)
        {   
            //getting a part of a dns
            int rndIndexer = StaticRandom.Rand(0, TownSample.Length);
            int dnsGetCountr = StaticRandom.Rand(0, (TownSample.Length - rndIndexer)+1);


            //creating a gene filled with -1
            Gene nGene = new Gene(TownSample.Length,true);
            //filling a random subset into the dns
            for (int i = rndIndexer; i < dnsGetCountr; i++)
            {
                nGene.dns[i] = inpGene.dns[i];
            }

            int j = 0;
            for (int i = 0; i < inpGene.dns.Length; i++)
            {
                if (!nGene.dns.Contains(inpGene.dns[i]))
                {
                    if (j == rndIndexer)
                    {
                        j = dnsGetCountr + 1;
                    }
                    nGene.dns[j] = inpGene.dns[i];
                    j++;
                }
            }
            ;
            return nGene;
        }
        public void FitnessReset()
        {
            Parallel.ForEach(genes, gene =>
            {
                gene.fitness = 0;
            }) ;

        }

        public void PopCrossoverWithOx()
        {
            //List<Gene> nGenes= new List<Gene>();
            int popHalf = genes.Length / 2;
            int ctr = 0;

            

            for (int i = 0; i < popHalf/2; i++)
            {
                genes[popHalf+ctr]=new Gene(OrderedCrossover(Gene.DeepCopy(genes[i * 2]).dns, Gene.DeepCopy(genes[i * 2+1]).dns));
                ctr++;
                genes[popHalf + ctr] = new Gene(OrderedCrossover(Gene.DeepCopy(genes[i * 2+1]).dns, Gene.DeepCopy(genes[i * 2]).dns));
                ctr++;
            }

            FitnessReset();


        }
        public static int[] OrderedCrossover(int[] parent1, int[] parent2)
        {
            int n = parent1.Length;
            int[] child = new int[n];

            // Véletlenszerűen kiválasztunk két pontot a szülő kromoszómok között
            int point1 = StaticRandom.Rand(0, n);
            int point2 = StaticRandom.Rand(0, n);

            // Biztosítjuk, hogy point1 < point2
            if (point1 > point2)
            {
                int temp = point1;
                point1 = point2;
                point2 = temp;
            }

            // Másoljuk a kiválasztott részt az első szülőből az utód kromoszómjába
            for (int i = point1; i <= point2; i++)
            {
                child[i] = parent1[i];
            }

            // Az utód kromoszóm másik részét a második szülőből töltjük fel
            int j = 0;
            for (int i = 0; i < n; i++)
            {
                if (!child.Contains(parent2[i]))
                {
                    if (j == point1)
                    {
                        j = point2 + 1;
                    }
                    child[j] = parent2[i];
                    j++;
                }
            }

            return child;
        }
        public void WriteOut(int topX=10)
        {
            Console.Clear();
            for (int i = 0; i < topX; i++)
            {
                Console.WriteLine(genes[i].fitness);
            }
            Console.WriteLine();
            Console.WriteLine("Current best fitness: "+ bestGene.fitness + "\t in the "+bestGenesGeneration+" th generation.");
            Console.WriteLine("Current generation counter: "+generationCounter);
            
        }
        public void Run(bool write)
        {
            while (true)
            {
                GetAllFitness();
                if (write)
                {
                    WriteOut();
                }

                //PopCrossOverWithFillRnd();

                PopCrossoverWithOx();
                PopMutation();
                generationCounter++;
                
            }
            
        }


    }

    public class Gene
    {
        public int[] dns;
        public float fitness;
        public Gene(int dnsCtr,bool fillWZeros)
        {
            if (fillWZeros)
            {
                dns = Enumerable.Repeat(-1,dnsCtr).ToArray();
                fitness= 0;

            }
            else
            {
                dns = Enumerable.Range(0, dnsCtr).OrderBy(x => Guid.NewGuid()).ToArray();
                fitness = 0;
            }
            
        }
        public Gene(int[] dns)
        {
            this.dns= dns;
        }

        public static Gene DeepCopy(Gene inp)
        {
            Gene nGene = new Gene(inp.dns.Length,false);
            for (int i = 0; i < inp.dns.Length; i++)
            {
                nGene.dns[i]=inp.dns[i];

            }
            nGene.fitness= inp.fitness;
            return nGene;

        }

    }

}
