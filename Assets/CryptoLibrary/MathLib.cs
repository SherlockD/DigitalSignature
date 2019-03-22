using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Random = System.Random;

namespace CryptoLibrary
{
    class CryptoLibraryExceptions : Exception
    {
        public CryptoLibraryExceptions(string message) : base(message)
        {}
    }
    
    public static class MathLib
    {
        public static List<bool> Shift(this List<bool> owner, int num)
        {
            List<bool> result = new List<bool>();
            for (int i = num; i < owner.Count; i++)
            {
                result.Add(owner[i]);
            }

            for (int i = 0; i < num; i++)
            {
                result.Add(owner[i]);
            }

            return result;
        }
        
        public static int ToInt(this List<bool> owner)
        {
            int result = 0;
            for (int i = owner.Count - 1; i >= 0; i--)
            {
                result += (owner[i]?1:0) * (int)Math.Pow(2,i);
            }

            return result;
        }
        
        public static List<bool> ExclusiveOr(this List<bool> owner, List<bool> additive)
        {
            if (owner.Count >= additive.Count)
            {
                var result = new List<bool>(owner);
                for (int i = 0; i < additive.Count; i++)
                {
                    result[i] = (owner[i] ^ additive[i]);
                }

                return result;
            }

            for (int i = 0; i < owner.Count; i++)
            {
                var result = new List<bool>(additive);
                result[i] = (owner[i] ^ additive[i]);
                return result;
            }

            return null;
        }
        
        public static List<bool> BitPlus(this List<bool> owner, List<bool> additive)
        {            
            if (owner.Count >= additive.Count)
            {
                var result = new List<bool>(owner);
                for (int i = 0; i < additive.Count; i++)
                {
                    result[i] = owner[i] | additive[i];
                }

                return result;
            }

            for (int i = 0; i < owner.Count; i++)
            {
                var result = new List<bool>(additive);
                result[i] = owner[i] | additive[i];
                return result;
            }

            return null;
        }
    }

    public static class CryptoLib
    {
        public static List<List<T>> SplitList<T>(this List<T> owner,int splitNum)
        {
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < owner.Count; i += splitNum)
            {
                List<T> node;
                if (i + splitNum <= owner.Count)
                {
                    node = owner.GetRange(i, splitNum);
                    result.Add(node);
                }
                else
                {
                    node = new List<T>();
                    for (int j = i; j < owner.Count; j++)
                    {
                        node.Add(owner[j]);
                    }
                    result.Add(node);
                    break;
                }
            }

            return result;
        }

        public static BigInteger BigIntRange(this Random owner, BigInteger min, BigInteger max)
        {
            if(max<min){throw new CryptographicException("Max value must be more then min value");}
            
            var random = new Random(DateTime.Now.Year);
            
            BigInteger difference = max - min;

            return min + (difference >> random.Next(1,5));
        }
    }
}