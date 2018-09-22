using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClassificationExample
{
    class Program
    {
        static void Main(string[] args)
        {
            int nearestNeighborsNumber = 5;                      
            List<Element> data = Read("iris.csv");
            Element elementToClassify = new Element(null, 6.3, 2.7, 5.5, 2.1);
            Console.WriteLine("Element to classify: " + elementToClassify.ToString());

            // get nearest neighbors
            List<Element> nearestNeighbors = 
                data.OrderBy(p => p.Distance(elementToClassify.Attributes)).Take(nearestNeighborsNumber).ToList();
            Console.WriteLine(nearestNeighborsNumber + " nearest neighbours found");
            nearestNeighbors.ForEach(e => Console.WriteLine(e));

            // voting
            string winnerClass = VoteForTheClass(nearestNeighbors, elementToClassify);
            Console.WriteLine("Found class: " + winnerClass);
            Console.ReadKey();
        }

        private static string VoteForTheClass(List<Element> nearestNeighbors, Element toClassify)
        {
            var votes = new Dictionary<string, double>();
            foreach (Element neighbor in nearestNeighbors)
            {
                if (!votes.Keys.Contains(neighbor.Clazz))
                    votes.Add(neighbor.Clazz, 0);
                votes[neighbor.Clazz] += 1 / Math.Pow(neighbor.Distance(toClassify.Attributes), 2);
            }
            return votes.First(x => x.Value == votes.Values.Max()).Key;
        }

        private static List<Element> Read(string path, char separator = ',')
        {
            string[] s = File.ReadAllLines(path);
            List<Element> rawData = new List<Element>();
            for (int i = 0; i < s.Length; i++)
            {
                string[] str = s[i].Split(separator);
                double[] d = new double[str.Length - 1];
                for (int j = 0; j < str.Length - 1; j++)
                    d[j] = Convert.ToDouble(str[j], CultureInfo.GetCultureInfo("en-US"));
                rawData.Add(new Element(str[str.Length - 1], d));
            }
            return rawData;
        }
    }

    class Element
    {
        public List<double> Attributes { get; private set; }
        public string Clazz { get; private set; }

        public int Count { get { return Attributes.Count; } }

        public Element(string clss, params double[] data)
        {
            Attributes = data.ToList();
            Clazz = clss;
        }

        public double Distance(List<double> other, double r = 2)
        {
            double sum = 0;
            for (int i = 0; i < other.Count; i++)
                sum += Math.Pow(Math.Abs(Attributes[i] - other[i]), r);
            return Math.Pow(sum, 1/r);
        }

        public override string ToString()
        {
            string attString = string.Join(",", Attributes);
            if (Clazz == null)
            {
                return attString;
            }
            else return attString + ":" + Clazz;
        }
    }
}
