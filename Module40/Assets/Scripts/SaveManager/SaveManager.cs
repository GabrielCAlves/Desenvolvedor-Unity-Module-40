using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;
using System.IO;
using Cloth;

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField] private SaveSetup _saveSetup;

    private string _path = Application.dataPath + "/save.txt";

    public int lastLevel;

    public Action<SaveSetup> FileLoaded;

    public SaveSetup Setup
    {
        get { return _saveSetup;  }
    }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    public void CreateNewSave()
    {
        _saveSetup = new SaveSetup();
        _saveSetup.lastLevel = 1;
        _saveSetup.playerName = "Gabriel";

        var i = GameObject.FindAnyObjectByType<PlayLevel>();
        i.OnLoad(_saveSetup);

        Save();
    }

    private void Start()
    {
        Invoke(nameof(Load), .1f);
    }

    #region SAVE
    [NaughtyAttributes.Button]
    private void Save()
    {
        string setupToJson = JsonUtility.ToJson(_saveSetup, true);
        Debug.Log(setupToJson);
        SaveFile(setupToJson);
    }

    public void SaveItems()
    {
        _saveSetup.coins = Items.ItemManager.Instance.GetItemByType(Items.ItemType.COIN).soInt.value;
        _saveSetup.health = Items.ItemManager.Instance.GetItemByType(Items.ItemType.LIFE_PACK).soInt.value;
        _saveSetup.puCloth = Player.Instance.puCloth;
        _saveSetup.healthBar = Player.Instance.healthBase.currentLife;

        Save();
    }

    public void SaveLastLevel(int level)
    {
        _saveSetup.lastLevel = level;
        SaveItems();
        //Save();
    }
    #endregion

    private void SaveFile(string json)
    {
        //string fileLoaded = "";

        //if (File.Exists(path))
        //{
        //    fileLoaded = File.ReadAllText(path);
        //}

        Debug.Log(_path);

        File.WriteAllText(_path, json);
    }

    [NaughtyAttributes.Button]
    private void Load()
    {
        string fileLoaded = "";

        if (File.Exists(_path))
        {
            fileLoaded = File.ReadAllText(_path);

            _saveSetup = JsonUtility.FromJson<SaveSetup>(fileLoaded);

            lastLevel = _saveSetup.lastLevel;
        }
        else
        {
            CreateNewSave();
        }


        FileLoaded.Invoke(_saveSetup);
    }
}

[System.Serializable]
public class SaveSetup
{
    public int lastLevel;
    public float coins;
    public float health;
    public string puCloth;
    public float healthBar;
    //public ClothItemCartoony itemCartoony;
    //public ClothItemHealth itemHealth;
    //public ClothItemDefense itemDefense;
    //public ClothItemSpeed itemSpeed;

    public string playerName;
    public string qualquer;
}
