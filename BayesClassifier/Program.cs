using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BayesClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PointXD> data = Read("test.txt");
            PointXD test = new PointXD(null, 6.3, 2.7, 5.5, 2.1);
            // пример из методички
            //List<PointXD> data = Read("test2.txt");           
            //PointXD test = new PointXD(null, "<=30", "средний", "да", "хороший");
            string winnerClass = BayesClassifier(data, test);
            Console.WriteLine("Found class: " + winnerClass);
        }

        private static string BayesClassifier(List<PointXD> data, PointXD test)
        {
            int n;
            Dictionary<string, int> classes = new Dictionary<string, int>(); // названия всех классов и количество объектов в каждом классе
            for (int i = 0; i < data.Count; i++)
            {
                if (!classes.Keys.Contains(data[i].Class))
                    classes.Add(data[i].Class, 1);
                else
                    classes[data[i].Class]++;
            }
            List <PointXD> probabilities = new List<PointXD>(); // вероятности атрибутов для каждого класса (по тесту)
            List<object> probs;
            for (int i = 0; i < classes.Count; i++)
            {
                probs = new List<object>();
                for (int j = 0; j < test.Count; j++)
                {
                    n = 0;
                    string testData = test.Attributes[j].ToString().Replace(',', '.');
                    for (int z = 0; z < data.Count; z++)
                        if (data[z].Attributes[j].Equals(testData) && data[z].Class == classes.Keys.ToList()[i].ToString())
                            n++;
                    probs.Add(n / (double)classes.Values.ToList()[i]);
                }
                probabilities.Add(new PointXD(classes.Keys.ToList()[i].ToString(), probs.ToArray()));
            }
            List<double> probabilities2 = new List<double>(); // вероятности теста для каждого класса
            for (int i = 0; i < classes.Count; i++)
            {
                double multiplication = 1;
                for (int j = 0; j < probabilities[i].Count; j++)
                    multiplication *= Convert.ToDouble(probabilities[i].Attributes[j]);
                probabilities2.Add(multiplication * classes.Values.ToList()[i] / data.Count); // вероятности теста для каждого класса, умноженные на вероятность класса
            }
            int index = probabilities2.IndexOf(probabilities2.Max());
            return classes.Keys.ToList()[index];
        }

        private static List<PointXD> Read(string path, char separator = ',')
        {
            string[] s = File.ReadAllLines(path);
            List<PointXD> rawData = new List<PointXD>();
            for (int i = 0; i < s.Length; i++)
            {
                string[] str = s[i].Split(separator);
                object[] d = new object[str.Length - 1];
                for (int j = 0; j < str.Length - 1; j++)
                    d[j] = str[j];
                rawData.Add(new PointXD(str[str.Length - 1], d));
            }
            return rawData;
        }
    }

    class PointXD
    {
        public List<object> Attributes;
        public string Class;

        public int Count { get { return Attributes.Count; } }

        public PointXD(string myclass, params object[] data)
        {
            Attributes = data.ToList();
            Class = myclass;
        }
    }
}
