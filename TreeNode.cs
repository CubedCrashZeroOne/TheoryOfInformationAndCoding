using System;
using System.Collections.Generic;
using System.Linq;

namespace TICprog
{
    internal class TreeNode
    {
        public TreeNode Left {get; set;}
        public TreeNode Right {get; set;}
        public string Value {get; set;}
        public double Probability{get; set;}

        public TreeNode(double probability, string value = null, TreeNode left = null, TreeNode right = null){
            Left = left;
            Right = right;
            Value = value;
            Probability = probability;
        }

    }
}
