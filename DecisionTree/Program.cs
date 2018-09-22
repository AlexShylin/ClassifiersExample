using System;
using System.Collections.Generic;
using System.IO;

namespace DecisionTreeSpace
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PointXD> data = Read("test.txt");
            PointXD test = new PointXD(null, 6.3, 2.7, 5.5, 2.1);

            DecisionTree tree = DecisionTree.Build(data);
            tree.Print();
            string winnerClass = MakeDecision(tree, test);
            Console.WriteLine("Found class: " + winnerClass);
            Console.WriteLine();

            // пример из методички
            data = Read("test2.txt");
            test = new PointXD(null, "<=30", "средний", "да", "хороший");
            tree = DecisionTree.Build(data);
            tree.Print();
            winnerClass = MakeDecision(tree, test);
            Console.WriteLine("Found class: " + winnerClass);
        }

        private static string MakeDecision(DecisionTree tree, PointXD test) => tree.Go(test);

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
}

