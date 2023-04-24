using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaladoAlg.Solvers
{
    public class SmallestBWSimulatedA
    {
        public static List<MyPoint> constantPoints;
        public List<MyPoint> polygon;
        public static List<MyPoint> solutionToDraw;
        public static float distanceToText;
        public static float tempToText;


        public SmallestBWSimulatedA()
        {
            distanceToText = 0;
            tempToText = 0;
            solutionToDraw = new List<MyPoint>();
            constantPoints = new List<MyPoint>();
            this.polygon = new List<MyPoint>();
        }
        public void AddConstant(float x,float y)
        {
            constantPoints.Add(new MyPoint(x,y));
        }
        public void AddPolygon(float x, float y)
        {
            solutionToDraw.Add(new MyPoint(x, y));
            polygon.Add(new MyPoint(x,y));

        }

        //ha a vonaltól jobbra van akkor - előjelű. Tehát ha az óra járásával megegyezően járjuk körbe a pontokat
        //akkor mindig minuszt kell, hogy kapjunk.
        public float distanceFromLine(MyPoint lp1, MyPoint lp2, MyPoint p)
        {
            // https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
            return ((lp2.y - lp1.y) * p.x - (lp2.x - lp1.x) * p.y + lp2.x * lp1.y - lp2.y * lp1.x) / (float)Math.Sqrt((float)Math.Pow(lp2.y - lp1.y, 2) + (float)Math.Pow(lp2.x - lp1.x, 2));
        }

        public float outerDistanceToBoundary(List<MyPoint> solution)
        {
            float sum_min_distances = 0;

            for (int pi = 0; pi < constantPoints.Count; pi++)
            {
                float min_dist = 0;
                for (int li = 0; li < polygon.Count; li++)
                {
                    float act_dist = distanceFromLine(solution[li], solution[(li + 1) % solution.Count], constantPoints[pi]);
                    if (li == 0 || act_dist < min_dist)
                        min_dist = act_dist;
                }
                if (min_dist < 0)
                    sum_min_distances += -min_dist;
            }
            return sum_min_distances;
        }

        float lengthOfBoundary(List<MyPoint> solution)
        {
            float sum_length = 0;

            for (int li = 0; li < solution.Count - 1; li++)
            {
                MyPoint p1 = solution[li];
                MyPoint p2 = solution[(li + 1) % solution.Count];
                sum_length += (float)Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
            }
            return sum_length;
        }

        public float SimulatedAnnealing(List<MyPoint> initialSolution, float InitialTemp, float coolingRate=1.1f)
        {
            List<MyPoint> currentSolution = initialSolution;
            float currentCost = lengthOfBoundary(currentSolution);

            List<MyPoint> bestSolution = initialSolution;
            float bestCost = currentCost;
            int t = 0;
            float temperature = InitialTemp;
            while(temperature>0)
            {

                List<MyPoint> newSolution = GenerateNewSolution(currentSolution,InitialTemp,temperature);
                float newCost = lengthOfBoundary(newSolution);

                float acceptanceProbability = AcceptanceProbability(currentCost, newCost, temperature);
                ;
                if (acceptanceProbability > (float)Utils.rnd.NextDouble())
                {
                    currentSolution = newSolution;
                    currentCost = newCost;
                    distanceToText = currentCost;
                    solutionToDraw = newSolution;


                }

                if (currentCost < bestCost)
                {
                    bestSolution = currentSolution;
                    bestCost = currentCost;


                }

                temperature = Temperature(t, InitialTemp,coolingRate);
                tempToText = temperature;
                t++;
                Thread.Sleep(1);
            }

            return bestCost;
        }

        private float AcceptanceProbability(float currentCost, float newCost, float temperature)
        {
            if (newCost < currentCost)
            {
                return 1.0f;
            }
            

            double delta = newCost - currentCost;
            return (float)Math.Exp((-delta*10) / (temperature*.1f));
        }

        private List<MyPoint> GenerateNewSolution(List<MyPoint> currentSolution,float maxTemp,float currentTemp)
        {
            var tmpSol = currentSolution.Select(t => new MyPoint(t.x, t.y)).ToList();
            for (int indexer = 0; indexer < tmpSol.Count; indexer++)
            {
                float xChange = ((float)Utils.rnd.NextDouble() - 0.5f) * 2;
                float yChange = ((float)Utils.rnd.NextDouble() - 0.5f) * 2;

                if (ChangeabilityCheck(tmpSol, indexer, xChange, yChange))
                {
                    tmpSol[indexer].x += xChange;
                    tmpSol[indexer].y += yChange;
                }

            }


            return tmpSol;
        }
        private bool ChangeabilityCheck(List<MyPoint> solution,int index,float xchange, float ychange)
        {
            MyPoint tmpPoint = new MyPoint(solution[index].x + xchange, solution[index].y + ychange);

            for (int i = 0; i < constantPoints.Count; i++)
            {
               
                ;
                if (0 < distanceFromLine(tmpPoint, solution[(index + 1) % solution.Count], constantPoints[i])
                    || 0 < distanceFromLine(solution[((index - 1) + solution.Count) % solution.Count], tmpPoint, constantPoints[i]))
                {
                    ;
                    return false;
                }
            }
            return true;
        }
        public float Temperature(int t, float TempMax,float alpha)
        {

            return (TempMax*(float)Math.Pow((1-(t/TempMax)),alpha));
        }

    }
    public class MyPoint
    {
        public int Id;
        private static int _id = 0;

        public float x;
        public float y;

        public MyPoint(float x, float y)
        {
            Id=_id++;
            this.x = x;
            this.y=y;

        }
    }
}
