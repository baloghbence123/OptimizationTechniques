using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaladoAlg.Problems
{

    public class TravellingSalesmanProblem
    {
        public List<Town> towns;
        public int xSize = 0;
        public int ySize = 0;
        int border = 100;

        public TravellingSalesmanProblem(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;
        }

        public void CreateInitialTowns(int townNumber)
        {
            towns= new List<Town>();

            for (int i = 0; i < townNumber; i++)
            {
                towns.Add(new Town(i, StaticRandom.Rand(border, xSize- border), StaticRandom.Rand(border, ySize- border)));
            }

        }
        public float GetDistanceBetweenAllTowns()
        {
            if (towns.Count>1)
            {
                float distance = 0f;
                for (int i = 0; i < towns.Count-1; i++)
                {
                    distance += GetDistance(towns[i], towns[i + 1]);
                }
                return distance;
            }
            else return 0;
        }
        public static float GetDistance(Town t1,Town t2) 
        {
            return (float)Math.Sqrt(Math.Pow(t2.X - t1.X, 2) + Math.Pow(t2.Y - t1.Y, 2));
        }

    }
    public class Town
    {
        public int Id;
        public float X;
        public float Y;
        public Town()
        {

        }
        public Town(int id, float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
