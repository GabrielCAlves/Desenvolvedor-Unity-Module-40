using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointBase : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public int key = 01;

    public int levelCheckPoint = 0;

    [Header("UI Message")]
    public GameObject checkpointMessage;
    public TextMeshProUGUI textMeshProUGUI;

    private bool checkpointActivated = false;
    //private string checkpointKey = "CheckpointKey";

    private void OnTriggerEnter(Collider other)
    {
        if(!checkpointActivated && other.transform.tag == "Player")
        {
            CheckCheckpoint();
        }
    }

    private void CheckCheckpoint()
    {
        TurnItOn();
        SaveCheckpoint();
    }

    [NaughtyAttributes.Button]
    private void TurnItOn()
    {
        meshRenderer.material.SetColor("_EmissionColor", Color.white);
        textMeshProUGUI.text = "Checkpoint Ativado!";
        StartCoroutine(MessageCoroutine());
    }

    protected virtual IEnumerator MessageCoroutine()
    {
        checkpointMessage.SetActive(true);
        yield return new WaitForSeconds(2f);
        checkpointMessage.SetActive(false);
    }

    private void TurnItOff()
    {
        meshRenderer.material.SetColor("_EmissionColor", Color.grey);
    }

    private void SaveCheckpoint()
    {
        //if(PlayerPrefs.GetInt(checkpointKey, 0) > key)
        //{
        //    PlayerPrefs.SetInt(checkpointKey, key);
        //}

        CheckpointManager.Instance.SaveCheckpoint(key);

        checkpointActivated = true;

        SaveManager.Instance.SaveLastLevel(levelCheckPoint);
    }
}
