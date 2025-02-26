using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "PersistentInt", menuName = "ScriptableObjects/PersistentIntVariable", order = 2)]
public class PersistentIntVariable : ScriptableObject
{
    public int value;
    public int highestValue;
    private string filePath;

    private void OnEnable()
    {
        filePath = Path.Combine(Application.persistentDataPath, $"{name}.json");
        Load();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        if (value > highestValue)
        {
            highestValue = value;
            Save();
        }
    }

    public void ApplyChange(int amount)
    {
        SetValue(value + amount);
    }

    public void Save()
    {
        File.WriteAllText(filePath, JsonUtility.ToJson(this));
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
        }
    }

    public void ResetHighestValue()
    {
        highestValue = 0;
        Save();
    }
}
