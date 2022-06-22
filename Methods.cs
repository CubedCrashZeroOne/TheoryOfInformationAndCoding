using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TICprog
{
    internal static class Methods
    {
        private static readonly Dictionary<char, double> probabilities = new Dictionary<char, double>
        {
            [' '] = 0.122,
            ['О'] = 0.09,
            ['А'] = 0.074,
            ['И'] = 0.059,
            ['І'] = 0.055,
            ['Н'] = 0.053,
            ['В'] = 0.047,
            ['Т'] = 0.044,
            ['Е'] = 0.041,
            ['Р'] = 0.04,
            ['С'] = 0.034,
            ['Л'] = 0.034,
            ['К'] = 0.032,
            ['У'] = 0.032,
            ['Д'] = 0.026,
            ['П'] = 0.026,
            ['М'] = 0.023,
            ['Ь'] = 0.021,
            ['З'] = 0.018,
            ['Й'] = 0.017,
            ['Б'] = 0.016,
            ['Я'] = 0.015,
            ['Г'] = 0.013,
            ['Ч'] = 0.012,
            ['Ш'] = 0.01,
            ['Х'] = 0.008,
            ['Щ'] = 0.008,
            ['Ж'] = 0.007,
            ['Ц'] = 0.006,
            ['Ю'] = 0.006,
            ['Ї'] = 0.006,
            ['Є'] = 0.003,
            ['Ф'] = 0.002
        };

        public static string FixError(this string message, string syndrome)
        {
            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (Convert.ToByte(syndrome, 2) == 0)
            {
                return message;
            }

            int n = message.Length;
            int r = n > 8 ? 4 : n > 5 ? 3 : 2;
            int k = n - r;

            var h = ParityCheckMatrix(n, k);

            int position = 0;

            for (position = 0; position < n; position++)
            {
                bool check = true;
                for(int i = 0; i < r; i++)
                {
                    bool bothOne = h[i, position] && syndrome[i] == '1';
                    bool bothZero = !h[i, position] && syndrome[i] == '0';
                    if (!(bothOne || bothZero))
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    break;
                }
            }


            

            char[] cArr = message.ToCharArray();
            cArr[position] = cArr[position] == '0' ? '1' : '0';
            string result = new string(cArr);

            Console.WriteLine($"Fixed error at position {position+1}");

            return result;
        }


        public static string AddError(this string message, string error)
        {
            var resultBuilder = new StringBuilder();
            if (message.Length != error.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < message.Length; i++)
            {
                resultBuilder.Append(message[i] == error[i] ? '0' : '1');
            }

            return resultBuilder.ToString();
        }

        public static string HuffmanDecode(this List<(TreeNode node, string code)> alphabet, string message)
        {
            var resultBuilder = new StringBuilder();
            string acode = String.Empty;
            for (int i = 0; i < message.Length; i++)
            {
                acode += message[i];
                var encodedChar = alphabet.FirstOrDefault(t => t.code.Equals(acode));
                if (!encodedChar.Equals(default))
                {
                    resultBuilder.Append(encodedChar.node.Value);
                    acode = String.Empty;
                }
            }

            return resultBuilder.ToString();
        }

        public static string HammingDecode(this string message)
        {
            int n = message.Length;
            int r = n > 8 ? 4 : n > 5 ? 3 : 2;
            int k = n - r;

            return message.Substring(0, k);
        }

        public static string HammingSyndrome(this string message)
        {
            int n = message.Length;
            int r = n > 8 ? 4 : n > 5 ? 3 : 2;
            int k = n - r;

            var h = ParityCheckMatrix(n, k);
            h.OutputMatrix();
            var b1 = message.Select(c => c == '1').ToArray();
            var b2 = new bool[b1.Length, 1];
            for (int i = 0; i < b1.Length; i++)
            {
                b2[i, 0] = b1[i];
            }

            var b = MultiplyMatrix(b2, h);
            var chars = new char[b.GetLength(0)];


            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = b[i, 0] ? '1' : '0';
            }

            return new string(chars);

        }

        public static bool[,] ParityCheckMatrix(int n, int k)
        {
            bool[,] result = new bool[n - k, n];

            // Complement.
            for (byte i = 0, row = 0; row < k; i++)
            {
                var bitArr = new BitArray(new byte[] { i });

                bool[] boolArr = new bool[8];
                bitArr.CopyTo(boolArr, 0);

                if (boolArr.Count(b => b) >= 2)
                {
                    // Inverse because BitArray has elements in reverse order.
                    for (int j = n - k - 1; j >= 0; j--)
                    {
                        result[j, row] = boolArr[j];
                    }
                    row++;
                }
            }

            // Identity matrix.
            for (int i = k; i < n; i++)
            {
                result[i - k, i] = true;
            }
            return result;
        }

        public static string HammingEncode(this string message)
        {
            int k = message.Length;
            int r = k > 4 ? 4 : k > 2 ? 3 : 2;
            int n = k + r;

            var g = GeneratingMatrix(n, k);
            g.OutputMatrix();
            var a1 = message.Select(c => c == '1').ToArray();
            var a2 = new bool[a1.Length, 1];
            for (int i = 0; i < a1.Length; i++)
            {
                a2[i, 0] = a1[i];
            }

            var b = MultiplyMatrix(a2, g);
            var chars = new char[b.GetLength(0)];


            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = b[i, 0] ? '1' : '0';
            }

            return new string(chars);

        }

        public static bool[,] MultiplyMatrix(bool[,] A, bool[,] B)
        {
            int cA = A.GetLength(0);
            int rA = A.GetLength(1);
            int cB = B.GetLength(0);
            int rB = B.GetLength(1);
            bool temp = false;
            bool[,] result = new bool[cB, rA];
            if (cA != rB)
            {
                throw new InvalidOperationException();
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = false;
                        for (int k = 0; k < cA; k++)
                        {
                            temp ^= (A[k, i] && B[j, k]);
                        }
                        result[j, i] = temp;
                    }
                }
                return result;
            }
        }

        public static void OutputMatrix(this bool[,] arr)
        {
            if (arr.GetLength(1) < arr.GetLength(0))
            {
                Console.WriteLine($"\nGenerating matrix for Hamming({arr.GetLength(0)}, {arr.GetLength(1)})");
            }
            else
            {
                Console.WriteLine($"\nParity check matrix for Hamming({arr.GetLength(1)}, {arr.GetLength(1) - arr.GetLength(0)})");
            }
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    Console.Write((arr[i, j] ? 1 : 0) + " ");
                }
                Console.WriteLine();
            }
        }

        public static bool[,] GeneratingMatrix(int n, int k)
        {
            bool[,] result = new bool[n, k];

            // Identity matrix.
            for (int i = 0; i < k; i++)
            {
                result[i, i] = true;
            }

            // Complement.
            for (byte i = 0, row = 0; row < k; i++)
            {
                var bitArr = new BitArray(new byte[] { i });

                bool[] boolArr = new bool[8];
                bitArr.CopyTo(boolArr, 0);

                if (boolArr.Count(b => b) >= 2)
                {
                    // Inverse because BitArray has elements in reverse order.
                    for (int j = n - k - 1; j >= 0; j--)
                    {
                        result[k + j, row] = boolArr[j];
                    }
                    row++;
                }
            }
            return result;
        }

        public static string HuffmanEncode(this List<(TreeNode node, string code)> alphabet, string message)
        {
            string encoded = message;
            foreach (var k in alphabet)
            {
                encoded = encoded.Replace(k.node.Value, k.code);
            }
            return encoded;
        }


        // Non-dictionary flag is to not use dictionary even if it's possible.
        // if set to false, this method will use the dictionary, 
        // unless there are characters that aren't in it
        public static List<TreeNode> CreateAlphabet(string input, bool nonDictionary = false)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentOutOfRangeException();
            }

            var result = new List<TreeNode>();

            if (nonDictionary == false && input.All(c => probabilities.ContainsKey(c)))
            {
                foreach (var k in input)
                {
                    result.Add(new TreeNode(probabilities[k], k.ToString()));
                }
            }
            else
            {
                char[] characters = input.ToCharArray();
                Array.Sort(characters);
                int i = 0;
                // I chose this bc pressing enter finalizes the input.
                char? current = null;
                foreach (var k in characters)
                {
                    if (k.Equals(current))
                    {
                        i++;
                    }
                    else
                    {
                        if (current != null)
                        {
                            result.Add(new TreeNode(Convert.ToDouble(i) / input.Length, current.ToString()));
                        }
                        current = k;
                        i = 1;
                    }
                }
                result.Add(new TreeNode(Convert.ToDouble(i) / input.Length, current.ToString()));
            }
            if (result.Count < 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            return result.GroupBy(t => t.Value)
                            .Select(g => g.First())
                            .ToList();
        }

        public static List<(TreeNode node, string code)> Huffman(this List<TreeNode> list)
        {
            var currentNode = list.Normalize().ToTree();
            currentNode.OutputTree();

            var result = new List<(TreeNode node, string code)>();

            string code = "";

            var stack = new Stack<TreeNode>();

            while (true)
            {
                if (currentNode.Value == null)
                {
                    stack.Push(currentNode);
                    currentNode = currentNode.Left;
                    code += "0";
                }
                else
                {
                    result.Add((node: currentNode, code: code));
                    if (stack.Count != 0)
                    {
                        currentNode = stack.Pop().Right;
                    }
                    else
                    {
                        break;
                    }
                    code = code.Remove(code.LastIndexOf('0'));
                    code += "1";
                }
            }

            return result;
        }

        public static TreeNode ToTree(this List<TreeNode> list)
        {

            if (list.Count() == 0)
            {
                throw new InvalidOperationException();
            }

            List<TreeNode> tempList = list.ToList();

            while (tempList.Count > 1)
            {
                var a = tempList.OrderBy(n => n.Probability).Take(2).ToArray();
                tempList.Add(new TreeNode(probability: Math.Round(a[0].Probability + a[1].Probability, 5),
                                        left: a[1], right: a[0]));
                tempList.Remove(a[0]);
                tempList.Remove(a[1]);
            }

            return tempList[0];
        }

        public static void OutputTree(this TreeNode node, int c = 0)
        {
            if (node.Left != null)
            {
                node.Left.OutputTree(c + 1);
            }

            for (int i = 0; i < c; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($"{(node.Value == null ? "( )" : $"({node.Value})")} - {node.Probability}");

            if (node.Right != null)
            {
                node.Right.OutputTree(c + 1);
            }
        }

        public static List<TreeNode> Normalize(this List<TreeNode> list)
        {
            var sum = list.Sum(n => n.Probability);
            foreach (var k in list)
            {
                k.Probability = Math.Round(k.Probability / sum, 5);
            }
            return list;
        }

        public static double NAverage(this List<(TreeNode node, string code)> list)
        {
            double result = 0;
            foreach (var k in list)
            {
                result += k.code.Length * k.node.Probability;
            }
            return Math.Round(result, 5);
        }

        public static double N(this List<TreeNode> list)
        {
            int i = 1;
            while (i < list.Count)
            {
                i *= 2;
            }
            return Math.Round(Math.Log2(i), 5);
        }

        public static double Eta(double n, double nAverage) => Math.Round(n / nAverage, 5);
        public static double Mu(double ha, double nAverage) => Math.Round(ha / nAverage, 5);

        public static double Ha(this List<TreeNode> list)
        {
            double result = 0;
            foreach (var k in list)
            {
                result -= k.Probability * Math.Log2(k.Probability);
            }
            return Math.Round(result, 5);
        }
    }
}
