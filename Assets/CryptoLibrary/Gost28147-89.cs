using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoLibrary;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gost2814789:ICrypto
{
    private List<List<bool>> _bitMessage;
    private List<bool> _key;

    private string _message;

    private List<bool> _encryptedList;
    
    private char[,] _sBlock =
    {
        {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'},
        {'9','6','3','2','8','B','1','7','A','4','E','F','C','0','D','5'},
        {'3','7','E','9','8','A','F','0','5','2','6','C','B','4','D','1'},
        {'E','4','6','2','B','3','D','8','C','F','5','A','0','7','1','9'},
        {'E','7','A','C','D','1','3','9','0','2','B','4','F','8','5','6'},
        {'B','5','1','9','8','D','F','0','E','4','2','3','C','7','A','6'},
        {'3','A','D','C','1','2','0','B','7','5','9','4','8','F','E','6'},
        {'1','D','2','9','7','A','6','0','F','C','4','5','F','3','B','E'},
        {'B','A','F','5','0','C','E','8','6','2','3','9','1','7','D','4'},
    };
    
    public Gost2814789()
    {
        _key = GetKey();       
    }
    
    public string Encrypt(string message)
    {        
        List<string> result = new List<string>();
        
        var bitArray = BitArrayToList(new BitArray(Encoding.UTF8.GetBytes(message)));

        _bitMessage = bitArray.SplitList(64);

        if (_bitMessage.Last().Count < 64)
        {
            for (int i = 0; i < _bitMessage.Last().Count % 64; i++)
            {
                _bitMessage.Last().Add(false);
            }
        }

        foreach (var _bitArray in _bitMessage)
        {

            var A = _bitArray.GetRange(0, 32); //старшие биты
            var B = _bitArray.GetRange(32, 32); //младшие биты

            var keyList = _key.SplitList(32);

            for (int i = 0; i < 24; i++)
            {
                //Debug.Log($"A: {PrintList(A)} B: {PrintList(B)} Key:{PrintList(keyList[i % 8])}");
                var buf = new List<bool>(A);
                A = new List<bool>(B.ExclusiveOr(A.BitPlus(keyList[i % 8])));
                B = new List<bool>(buf);
            }

            for (int i = 7; i >= 0; i--)
            {
                //Debug.Log($"A: {PrintList(A)} B: {PrintList(B)} Key:{PrintList(keyList[i % 8])}");
                var buf = new List<bool>(A);
                A = new List<bool>(B.ExclusiveOr(A.BitPlus(keyList[i])));
                B = new List<bool>(buf);
            }
            
            _encryptedList = Concat(A, B);
            
            result.Add(CodeToString(_encryptedList.Shift(11)));
        }

        return string.Join("", result);
        //Debug.Log($"Encrypted:{PrintList(result)} Key:{PrintList(_key)}");
    }

    public string CodeToString(List<bool> code)
    {
        string result = "";
        for (int i = 0; i < 8; i++)
        {
            result += _sBlock[i, code.GetRange(4 * i, 4).ToInt()];            
        }

        return result;
    }

    private string PrintList(List<bool> owner)
    {
        string result = "";
        foreach (var node in owner)
        {
            result += node ? 1 : 0;
        }

        return result;
    }
    
    private string PrintList(List<string> owner)
    {
        string result = "";
        foreach (var node in owner)
        {
            result += node;
            result += ' ';
        }

        return result;
    }
    
    private List<bool> Concat(List<bool> owner, List<bool> additive)
    {
        foreach (var node in additive)
        {
            owner.Add(node);
        }

        return owner;
    }
    
    private List<bool> BitArrayToList(BitArray bitArray)
    {
        var result = new List<bool>();
        for (int i = 0; i < bitArray.Length; i++)
        {
            result.Add(bitArray[i]);
        }

        return result;
    }

    private List<bool> GetKey()
    {
        var result = new List<bool>();
        for (int i = 0; i < 256; i++)
        {
            result.Add(Random.Range(1,100) <= 50);
        }

        return result;
    }
}
