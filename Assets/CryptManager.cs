using UnityEngine;

public class CryptManager : MonoBehaviour
{
    private void Awake()
    {
        Gost341194 gost341194 = new Gost341194();
        var result = gost341194.GenerateSignature("Hello");
        Debug.Log(gost341194.CheckSignature(result.Item1, result.Item2, result.Item3));
    }
}
