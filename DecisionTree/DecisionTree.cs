using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DecisionTreeSpace
{
    class PointXD
    {
        public List<object> Attributes { set; get; }
        public string Class { set; get; }
        public int Count { get { return Attributes.Count; } }

        public PointXD(string myclass, params object[] data)
        {
            Attributes = data.ToList();
            Class = myclass;
        }
    }

    class DecisionTree
    {
        public string Answer { set; get; }
        public int Question { set; get; }
        public Dictionary<object, DecisionTree> Branches { set; get; } = Init();

        private static Dictionary<object, DecisionTree> Init() => new Dictionary<object, DecisionTree>();

        public static DecisionTree Build(List<PointXD> data)
        {
            DecisionTree tree = new DecisionTree();
            data.ForEach(point => tree.HandlePoint(point));
            tree.Simplify();
            return tree;
        }

        private void Simplify()
        {
            List<DecisionTree> branches = Branches.Values.ToList();
            branches.ForEach(tree => tree.Simplify());

            List<string> answers = GetAnswers(branches);
            if (answers.Count == 1)
            {                
                Branches = Init();
                Answer = answers[0];
            }
        }

        private List<String> GetAnswers(List<DecisionTree> branches)
        {
            List<string> answers = new List<string>();

            answers.AddRange(branches.FindAll(branch => branch.Answer != null).Select(branch => branch.Answer));
            branches.ForEach(branch => answers.AddRange(GetAnswers(branch.Branches.Values.ToList())));

            return answers.Distinct().ToList();
        }

        private DecisionTree HandlePoint(PointXD point, int paramCount = 0)
        {
            if (paramCount == point.Count)
            {
                Answer = point.Class;
                return this;
            }
        
            Question = paramCount;
            object param = point.Attributes[paramCount];
            DecisionTree tree = Branches.ContainsKey(param) ? Branches[param] : new DecisionTree();
            Branches[param] = tree.HandlePoint(point, ++paramCount);

            return this;
        }

        public string Go(PointXD test)
        {
            foreach (KeyValuePair<object, DecisionTree> pair in Branches)
            {
                if (AboutEqual(pair.Key, (test.Attributes[Question])))
                {
                    return pair.Value.Go(test);
                }
            }
            return Answer ?? Go(test);
        }

        private static bool AboutEqual(double x, double y) => Math.Abs(x - y) <= 0.1;

        private static bool AboutEqual(object x, object y)
        {
            if (ToDouble(x, out double resX) && (ToDouble(y, out double resY)))
            {
                return AboutEqual(resX, resY);
            }
            return x.Equals(y);
        }

        private static bool ToDouble(object x, out double resX)
        {
            return double.TryParse(x.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out resX);
        }

        public void Print(int paramCount = 0)
        {
            foreach (KeyValuePair<object, DecisionTree> pair in Branches)
            {
                Console.WriteLine(new string(' ', paramCount * 4) + pair.Key);
                if (pair.Value.Branches.Count == 0)
                {
                    Console.WriteLine(new string(' ', paramCount * 4 + 4) + pair.Value.Answer);
                }
                pair.Value.Print(++paramCount);
                paramCount--;
            }
        }
    }
}