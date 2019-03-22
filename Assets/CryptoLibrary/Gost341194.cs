using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoLibrary;
using UnityEngine;
using Random = System.Random;


public class Gost341194
{
    internal Gost2814789 gost2814789;
    
    internal BigInteger P;
    internal BigInteger Q;

    internal BigInteger A;
    internal BigInteger K;

    internal BigInteger PublicKey;
    internal BigInteger PrivateKey;
    
    public Gost341194()
    {
        gost2814789 = new Gost2814789();
        GenerateStartNumbersAsync();
        Task.WaitAll();
    }

    public (string,BigInteger,BigInteger) GenerateSignature(string message)
    {
        Random random = new Random();
        
        var hash = new BigInteger(Encoding.ASCII.GetBytes(gost2814789.Encrypt(message)));
        BigInteger r = BigInteger.ModPow(A,K,P) % Q;
        /*while (true)
        {            
            r = BigInteger.ModPow(A, K, P);
            if (r>=Q)
            {
                K++;
                continue;
            }

            break;
        }*/
        
        
        
        var s = (PrivateKey * r + K * hash) % Q;

        return (message, r, s);
    }

    public bool CheckSignature(string message, BigInteger r,BigInteger s)
    {
        var hash = new BigInteger(Encoding.ASCII.GetBytes(gost2814789.Encrypt(message)));
       // Debug.Log($"P: {P}\n Q: {Q}\n A: {A}\n K: {K} PublicKey: {PublicKey} PrivateKey: {PrivateKey}");
        
        var v = BigInteger.ModPow(hash, Q - 2, Q);        
        
        var z1 = (s * v) % Q;
        var z2 = ((Q - r) * v) % Q;
        
        Debug.Log($"v:{v} z1:{z1} z2:{z2}");
        
        var u = BigInteger.ModPow(A,z1,P) * BigInteger.ModPow(PublicKey,z2,P) % Q;

        return r == u;
    }
    
    private async void GenerateStartNumbersAsync()
    {
        var random = new Random();
        
        List<Task<BigInteger>> tasks = new List<Task<BigInteger>>();
        tasks.Add((Task.Run(() =>GetPrime(BigInteger.Pow(2,509), BigInteger.Pow(2,512)))));
        tasks.Add((Task.Run(() =>GetPrime(BigInteger.Pow(2,254), BigInteger.Pow(2,256)))));
        Task.WaitAll(tasks.ToArray());
        
        P = await tasks[0];
        Q = TakeQ();

        tasks.Clear();
        
        tasks.Add(Task.Run(TakeA));
        //tasks.Add(Task.Run(()=>random.BigIntRange(0, Q)));
        
        Task.WaitAll(tasks.ToArray());

        A = await tasks[0];
        K = 1;

        PrivateKey = random.BigIntRange(0, Q);
        PublicKey = BigInteger.ModPow(A, PrivateKey, P);
                
    }

    private BigInteger TakeQ()
    {
        for (BigInteger i = BigInteger.Pow(2, 254); i < BigInteger.Pow(2, 256); i++)
        {
            if ((P-1) % i == 0)
            {
                return i;
            }
        }

        return 0;
    }
    
    private async Task<BigInteger> TakeA()
    {
        Random random = new Random();
        
        BigInteger d = 1;
        
        while(true)
        {            
            var f = BigInteger.ModPow(d, (P - 1) / Q, P);
            if (f != 1)
            {
                return f;
            }

            d++;
        }
    }
    
    #region PrimeNumberGenerator
        
    private async Task<BigInteger> GetPrime(BigInteger min, BigInteger max)
    {
        for (BigInteger i = min; i <= max; i++)
        {
            if (MillerRabinTest(i, 3))
            {
                return i;
            }
        }

        return 0;
    }
    
    private bool MillerRabinTest(BigInteger n, int k)
    {
        if (n == 2 || n == 3)
            return true;
 
        if (n < 2 || n % 2 == 0)
            return false;
 
        BigInteger t = n - 1;
 
        int s = 0;
 
        while (t % 2 == 0)
        {
            t /= 2;
            s += 1;
        }

        for (int i = 0; i < k; i++)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
 
            byte[] _a = new byte[n.ToByteArray().LongLength];
 
            BigInteger a;
 
            do
            {
                rng.GetBytes(_a);
                a = new BigInteger(_a);
            }
            while (a < 2 || a >= n - 2);
            
            BigInteger x = BigInteger.ModPow(a, t, n);
 
            if (x == 1 || x == n - 1)
                continue;
 
            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, n);
 
                if (x == 1)
                    return false;
 
                if (x == n - 1)
                    break;
            }
 
            if (x != n - 1)
                return false;
        }
 
        return true;
    }
    
    
    #endregion  
}
