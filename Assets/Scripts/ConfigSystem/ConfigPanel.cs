using UnityEngine;

public class ConfigPanel : MonoBehaviour
{
    public void btnSelect()
    {
        Debug.Log("Option Select");
    }

    public void btnSave()
    {
        ConfigManager.Instance.SaveData();
    }

    public void btnBack()
    {
        Debug.Log("Close");
    }

}
