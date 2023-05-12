using HaladoAlg.Problems;
using HaladoAlg.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Plot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Town[] towns;
        public int[] bestRoute;
        public int currentGen = 0;

        int dotSize = 8;
        public MainWindow()
        {
            InitializeComponent();


        }

        public void DrawGenetic(object sender, EventArgs e)
        {

            ResetCan();
            if (towns != null)
            {


                foreach (var town in towns)
                {
                    Ellipse dot = new Ellipse();
                    dot.Width = dotSize;
                    dot.Height = dotSize;
                    dot.Fill = Brushes.Red;
                    Canvas.SetLeft(dot, town.X); // set the X coordinate of the dot
                    Canvas.SetTop(dot, town.Y); // set the Y coordinate of the dot
                    can.Children.Add(dot); // add the dot to the canvas
                }
                if (bestRoute != null)
                {


                    for (int i = 0; i < bestRoute.Length - 1; i++)
                    {
                        var tmpTown1 = towns[bestRoute[i]];
                        var tmpTown2 = towns[bestRoute[i + 1]];

                        Line line = new Line();
                        line.X1 = tmpTown1.X + dotSize / 2; // set the X coordinate of the starting point
                        line.Y1 = tmpTown1.Y + dotSize / 2; // set the Y coordinate of the starting point
                        line.X2 = tmpTown2.X + dotSize / 2; // set the X coordinate of the ending point
                        line.Y2 = tmpTown2.Y + dotSize / 2; // set the Y coordinate of the ending point
                        line.Stroke = Brushes.Black;
                        can.Children.Add(line);
                        ;
                    }
                    textLabel.Content = currentGen.ToString();

                }
            }

        }
        public void RemoveChilds()
        {
            grid.Children.Remove(g1);
            grid.Children.Remove(g2);
            grid.Children.Remove(g3);
            grid.Children.Remove(bOne);
            grid.Children.Remove(bTwo);
            grid.Children.Remove(bThree);

        }
        public void ResetCan()
        {
            can.Children.Clear();
        }
        public void StartGenetic(int townNumb,int populationNumber)
        {
            TravellingSalesmanProblem t1 = new TravellingSalesmanProblem((int)ActualWidth, (int)ActualHeight);

            t1.CreateInitialTowns(townNumb);
            towns = t1.towns.ToArray();

            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(populationNumber, 0.5f, t1.towns);
            while (true)
            {
                geneticAlgorithm.GetAllFitness();
                geneticAlgorithm.PopCrossoverWithOx();
                geneticAlgorithm.PopMutation();
                bestRoute = geneticAlgorithm.bestGene.dns;
                currentGen++;
            }
        }

        private void GeneticV(object sender, RoutedEventArgs e)
        {
            can.Width = this.ActualWidth;
            can.Height = this.ActualHeight;

            int townNumb = Convert.ToInt32(townNumber.Text.ToString());
            int populationNumber = Convert.ToInt32(popSize.Text.ToString());

            Task task = new Task(() =>
            {
                StartGenetic(townNumb, populationNumber);
            }, TaskCreationOptions.LongRunning);
            RemoveChilds();
            task.Start();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += DrawGenetic;
            timer.Start();

            ;
        }

        private void StartNsgaII(int generation)
        {
            frontList = new List<Person>();
            
            List<Person> p = new List<Person>();
            p.AddRange(Enumerable.Range(0, Person.PopulationSize).Select(t => new Person()).ToList());
            var tmp = NSGAII.NSGAIIMethod(p, 2);
            
            ;
            while (true)
            {
                tmp = NSGAII.NSGAIIMethod(tmp, generation);
                frontList = tmp.ToList();
                Thread.Sleep(100);
            }

            

            
        }

        private void NSGAIIV(object sender, RoutedEventArgs e)
        {

            can.Width = this.ActualWidth;
            can.Height = this.ActualHeight;
            Thread.Sleep(100);

            Person.PopulationSize = Convert.ToInt32(personPop.Text.ToString());
            int maxgeneration = Convert.ToInt32(maxGen.Text.ToString());
            Task task = new Task(() =>
            {
                StartNsgaII(maxgeneration);


            }, TaskCreationOptions.LongRunning);
            task.Start();
            RemoveChilds();
            ;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += DrawNsga;
            timer.Start();



            //
            //List<List<Person>> pl = new List<List<Person>>();
            //pl.Add(p);
            //DrawNsga(pl);
            ;
        }
        public List<Person> frontList;
        private void DrawNsga(object sender, EventArgs e)
        {
            


            if (frontList != null)
            {

                ResetCan();


                Line line = new Line();
                var tmp1 = PointCalculator(1, 0);
                var tmp2 = PointCalculator(1, 1);

                line.X1 = tmp1[0] + dotSize / 2; // set the X coordinate of the starting point
                line.Y1 = tmp1[1] + dotSize / 2;// set the Y coordinate of the starting point
                line.X2 = tmp2[0] + dotSize / 2; // set the X coordinate of the ending point
                line.Y2 = tmp2[1] + dotSize / 2; // set the Y coordinate of the ending point
                line.Stroke = Brushes.Black;
                can.Children.Add(line);

                line = new Line();
                tmp1 = PointCalculator(0, 1);
                tmp2 = PointCalculator(1, 1);

                line.X1 = tmp1[0] + dotSize / 2; // set the X coordinate of the starting point
                line.Y1 = tmp1[1] + dotSize / 2;// set the Y coordinate of the starting point
                line.X2 = tmp2[0] + dotSize / 2; // set the X coordinate of the ending point
                line.Y2 = tmp2[1] + dotSize / 2; // set the Y coordinate of the ending point
                line.Stroke = Brushes.Black;
                can.Children.Add(line);

                foreach (var p in frontList)
                {
                    Ellipse dot = new Ellipse();
                    dot.Width = dotSize;
                    dot.Height = dotSize;
                    dot.Fill = Brushes.Red;
                    var tmpPts = PointCalculator(p.Properties[0], p.Properties[1]);
                    Canvas.SetLeft(dot, tmpPts[0] - dotSize / 2);
                    Canvas.SetTop(dot, tmpPts[1] - dotSize / 2);
                    can.Children.Add(dot);
                    //for (int i = 0; i < p.Count - 1; i++)
                    //{
                    //    line = new Line();
                    //    tmp1 = PointCalculator(p[i].Properties[0], p[i].Properties[1]);
                    //    tmp2 = PointCalculator(p[i + 1].Properties[0], p[i + 1].Properties[1]);

                    //    line.X1 = tmp1[0]; // set the X coordinate of the starting point
                    //    line.Y1 = tmp1[1];// set the Y coordinate of the starting point
                    //    line.X2 = tmp2[0]; // set the X coordinate of the ending point
                    //    line.Y2 = tmp2[1]; // set the Y coordinate of the ending point
                    //    line.Stroke = Brushes.Blue;
                    //    can.Children.Add(line);
                    //    ;
                    //}
                }
            }



        }
        private float[] PointCalculator(float x, float y)
        {
            float[] retVal = new float[2];
            retVal[0] = (float)((can.Width - 200) * (1-x)) + 100;
            retVal[1] = (float)((can.Height - 200) * y) + 100;

            return retVal;
        }

        private void SimulatedA(object sender, RoutedEventArgs e)
        {

            can.Width = this.ActualWidth;
            can.Height = this.ActualHeight;
            Thread.Sleep(100);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(25);
            timer.Tick += DrawSimul;
            timer.Start();

            SmallestBWSimulatedA sa = new SmallestBWSimulatedA();
            //----- ITT LEHET HOZZÁADNI POLIGONOKAT---------
            sa.AddConstant(200, 200);
            sa.AddConstant(500, 250);
            sa.AddConstant(800, 400);

            sa.AddPolygon(10, 10);
            sa.AddPolygon(1000, 10);
            sa.AddPolygon(1000, 500);
            sa.AddPolygon(10, 500);

            int initialTemperature = Convert.ToInt32(initialTemp.Text.ToString());
            float crate = Convert.ToSingle(coolingRate.ToString());
            Task t = new Task(() =>
            {
                sa.SimulatedAnnealing(sa.polygon, initialTemperature, crate);
            });
            t.Start();
            RemoveChilds();



        }

        private void DrawSimul(object sender, EventArgs e)
        {
            ResetCan();
            if (SmallestBWSimulatedA.constantPoints != null)
            {


                foreach (var town in SmallestBWSimulatedA.constantPoints)
                {
                    Ellipse dot = new Ellipse();
                    dot.Width = dotSize;
                    dot.Height = dotSize;
                    dot.Fill = Brushes.Red;
                    Canvas.SetLeft(dot, town.x); // set the X coordinate of the dot
                    Canvas.SetTop(dot, town.y); // set the Y coordinate of the dot
                    can.Children.Add(dot); // add the dot to the canvas
                }
            }

            if (SmallestBWSimulatedA.solutionToDraw != null)
            {


                for (int i = 0; i < SmallestBWSimulatedA.solutionToDraw.Count; i++)
                {
                    var point1 = SmallestBWSimulatedA.solutionToDraw[i];
                    Ellipse dot = new Ellipse();
                    dot.Width = dotSize;
                    dot.Height = dotSize;
                    dot.Fill = Brushes.Red;
                    Canvas.SetLeft(dot, point1.x); // set the X coordinate of the dot
                    Canvas.SetTop(dot, point1.y); // set the Y coordinate of the dot
                    can.Children.Add(dot); // add the dot to the canvas

                    var point2 = SmallestBWSimulatedA.solutionToDraw[(i + 1)%SmallestBWSimulatedA.solutionToDraw.Count];

                    dot = new Ellipse();
                    dot.Width = dotSize;
                    dot.Height = dotSize;
                    dot.Fill = Brushes.Red;
                    Canvas.SetLeft(dot, point2.x); // set the X coordinate of the dot
                    Canvas.SetTop(dot, point2.y); // set the Y coordinate of the dot
                    can.Children.Add(dot); // add the dot to the canvas

                    Line line = new Line();
                    line.X1 = point1.x + dotSize / 2; // set the X coordinate of the starting point
                    line.Y1 = point1.y + dotSize / 2; // set the Y coordinate of the starting point
                    line.X2 = point2.x + dotSize / 2; // set the X coordinate of the ending point
                    line.Y2 = point2.y + dotSize / 2; // set the Y coordinate of the ending point
                    line.Stroke = Brushes.Black;
                    can.Children.Add(line);
                    ;
                }

            }
            textLabel.Content = "Distance:"+SmallestBWSimulatedA.distanceToText.ToString()+"\tC:"+SmallestBWSimulatedA.tempToText;
        }



    }
}
