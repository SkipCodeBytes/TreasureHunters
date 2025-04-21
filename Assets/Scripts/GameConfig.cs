using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(this);
        } else{
            Destroy(Instance);
        }
        
        recoverGameConfig();
        setGameConfig();
    }

    private void recoverGameConfig()
    {

    }

    private void setGameConfig()
    {

    }


    private void saveGraphicConfig(){

    }
}
