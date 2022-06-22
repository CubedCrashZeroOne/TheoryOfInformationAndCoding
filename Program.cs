using System;
using System.Collections.Generic;
using System.Linq;

namespace TICprog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose an input string:");
            var input = Console.ReadLine();
            Console.WriteLine("\n------------------------------------------\nHuffman Encoding:");
            var list1 = Methods.CreateAlphabet(input);
            foreach (var k in list1)
            {
                Console.WriteLine(k.Value + " " + k.Probability);
            }



            var alphabet = list1.Huffman();

            Console.WriteLine(Environment.NewLine);
            foreach (var k in alphabet)
            {
                Console.WriteLine($"{k.node.Value} - {k.code}");
            }

            Console.WriteLine($"\nn = {list1.N()}");
            Console.WriteLine($"n average = {alphabet.NAverage()}");
            Console.WriteLine($"H(A) = {list1.Ha()}");
            Console.WriteLine($"eta = {Methods.Eta(list1.N(), alphabet.NAverage())}");
            Console.WriteLine($"mu = {Methods.Mu(list1.Ha(), alphabet.NAverage())}");

            string encoded = alphabet.HuffmanEncode(input);
            Console.WriteLine("\nEncoded message: " + encoded);

            Console.WriteLine("\n------------------------------------------\nHamming Encoding:");

            for (int i = 8; i < encoded.Length; i += 9)
            {
                encoded = encoded.Insert(i, " ");
            }

            var messages = encoded.Split(' ');

            for (int i = 0; i < messages.Length; i++)
            {
                Console.WriteLine("\n\nInitial message: " + messages[i]);
                messages[i] = messages[i].HammingEncode();
                Console.WriteLine("\nEncoded message: " + messages[i]);
            }

            Console.WriteLine("\n------------------------------------------\nErrors:");

            string error = string.Empty;
            for (int i = 0; i < messages.Length; i++)
            {
                error = string.Empty;
                Console.WriteLine("\n\nSent message: " + messages[i]);
                while (true)
                {
                    Console.Write("Input Error: ");
                    error = Console.ReadLine();
                    if (error.Length == messages[i].Length && error.All(c => c == '0' || c == '1'))
                    {
                        break;
                    }
                    else if (string.IsNullOrEmpty(error))
                    {
                        error = error.PadLeft(messages[i].Length, '0');
                        break;
                    }
                    
                }

                messages[i] = messages[i].AddError(error);
                Console.WriteLine("\nReceived message: " + messages[i]);
            }

            Console.WriteLine("\n------------------------------------------\nHamming Decoding:");

            for (int i = 0; i < messages.Length; i++)
            {
                Console.WriteLine("\n\nReceived message: " + messages[i]);
                string syndrome = messages[i].HammingSyndrome();
                Console.WriteLine("\nMessage syndrome: " + syndrome);
                messages[i] = messages[i].FixError(syndrome).HammingDecode();
                Console.WriteLine("\nDecoded message: " + messages[i]);
            }

            Console.WriteLine("\n------------------------------------------\nHuffman Decoding:");

            encoded = string.Concat(messages);
            string decoded = alphabet.HuffmanDecode(encoded);
            Console.WriteLine("\nDecoded message: " + decoded);

            Console.ReadKey();
        }
    }
}