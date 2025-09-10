using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Core.Singleton;

public class UiInGameManager : Singleton<UiInGameManager>
{
    public TextMeshProUGUI uiTextCoins;

    public void UpdateTextCoins(string s)
    {
        uiTextCoins.text = s;
    }

    //public static void UpdateTextCoins(string s)
    //{
    //    Instance.uiTextCoins.text = s;
    //}
}
