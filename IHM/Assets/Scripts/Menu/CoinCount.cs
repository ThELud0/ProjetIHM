using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCount : MonoBehaviour
{
    public Text CountCoin;
    static public int count = 0;

    void Start()
    {
        count = 0;
    }

    void Update()
    {
        CountCoin.text = " x " + count;
    }
}