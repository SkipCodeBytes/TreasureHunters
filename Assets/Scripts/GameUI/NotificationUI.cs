
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance;

    [SerializeField] private GameObject msgBoxContent;
    [SerializeField] private Text messageInfo;
    [SerializeField] private float duration = 1.5f;

    private float timer = 0;
    private bool isMsgActive = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isMsgActive) return;
        if(timer < Time.time)
        {
            msgBoxContent.SetActive(false);
            isMsgActive = false;
        }
    }

    public void SetMessage(string msj, Color color)
    {
        SoundController.Instance.PlaySound(GameManager.Instance.SoundLibrary.GetClip("Blip"));
        messageInfo.text = msj;
        messageInfo.color = color;
        timer = Time.time + duration;
        msgBoxContent.SetActive(true);
        isMsgActive = true;
    }
}
