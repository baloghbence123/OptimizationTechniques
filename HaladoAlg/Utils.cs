﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaladoAlg
{
    public static class Utils
    {
        public static Random rnd= new Random();
        

    }
    public static class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Rand()
        {
            return random.Value.Next();
        }
        public static int Rand(int max)
        {
            return random.Value.Next(max);

        }
        public static int Rand(int min,int max)
        {
            return random.Value.Next(min,max);

        }
        public static float RandFloat()
        {
            return (float)random.Value.NextDouble();
        }

    }
}
