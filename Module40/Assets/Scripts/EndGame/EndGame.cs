using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndGame : MonoBehaviour
{
    public string tagToCompare = "Player";
    //public GameObject uiEndgame;

    public List<GameObject> endGameObjects;
    public int nextLevel = 1; //0;

    public SFXType sfxType;

    private bool _endGame = false;

    private void Awake()
    {
        endGameObjects.ForEach(i => i.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(tagToCompare) && !_endGame)
        {
            if (sfxType != SFXType.NONE)
            {
                PlaySFX();
            }
            
            CallEndGame();
        }
    }

    public void CallEndGame()
    {
        //uiEndgame.SetActive(true);
        //endGameObjects.ForEach(i => i.SetActive(true));
        _endGame = true;

        //++currentLevel;

        foreach (var i in endGameObjects)
        {
            i.SetActive(true);
            i.transform.DOScale(0, .2f).SetEase(Ease.OutBack).From();
            SaveManager.Instance.SaveLastLevel(SaveManager.Instance.Setup.lastLevel);
        }

        nextLevel = SaveManager.Instance.Setup.lastLevel+1;
        Debug.Log("(CallEndGame) SaveManager.Instance.Setup.lastLevel = " + SaveManager.Instance.Setup.lastLevel);
        Debug.Log("(CallEndGame) nextLevel = " + nextLevel);
        if(nextLevel < 3)
        {
            gameObject.GetComponent<LoadScene>().Load(nextLevel);
        }
        else
        {
            SaveManager.Instance.SaveLastLevel(1);
            gameObject.GetComponent<LoadScene>().Load(0);
        }
    }

    private void PlaySFX()
    {
        SFXPool.Instance.Play(sfxType);
    }
}
