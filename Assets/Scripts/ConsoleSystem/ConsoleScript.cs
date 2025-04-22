using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConsoleScript : MonoBehaviour
{
    [SerializeField] private GameObject consoleObject;
    [SerializeField] private Transform consoleContent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputField consoleInputField;

    [SerializeField] private GameObject debugMessagePrefab;
    [SerializeField] private int messagesPoolSize = 30;
    [SerializeField] private List<ConsoleMessageBox> messagesPool = new List<ConsoleMessageBox>();
    [SerializeField] private int indexPool = 1;

    [SerializeField] private KeyCode openConsoleKey = KeyCode.Return;
    [SerializeField] private KeyCode SendKey = KeyCode.Return;
    [SerializeField] private KeyCode closeConsoleKey = KeyCode.Escape;

    private static ConsoleScript _mainInstance;

    private void Awake()
    {
        if (_mainInstance == null) _mainInstance = this;
        else { 
            Destroy(this);
            return;
        }
        Transform msgInitInstance = consoleContent.transform.GetChild(0);
        messagesPool.Add(new ConsoleMessageBox(msgInitInstance.gameObject, msgInitInstance.GetChild(0).GetComponent<Text>()));
        messagesPool[0].TextBox.SetActive(true);
        for (int i = 1; i < messagesPoolSize; i++)
        {
            GameObject msgObj = Instantiate(_mainInstance.debugMessagePrefab, _mainInstance.consoleContent);
            ConsoleMessageBox msgBox = new ConsoleMessageBox(msgObj, msgObj.transform.GetChild(0).GetComponent<Text>());
            messagesPool.Add(msgBox);
        }
    }

    void Update()
    {
        if (consoleObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(SendKey)) sendInputMessage();
            if (Input.GetKeyDown(closeConsoleKey)) closeConsole();
        }
        else
        {
            if (Input.GetKeyDown(openConsoleKey)) openConsole();
        }
    }


    private ConsoleMessageBox getMessageBox()
    {
        ConsoleMessageBox messageBox = messagesPool[indexPool];
        indexPool++;
        if (indexPool == messagesPoolSize) indexPool = 0;
        return messageBox;
    }

    public static void writeConsoleMessage(string message, Color txtColor = default)
    {
        ConsoleMessageBox messageBox = _mainInstance.getMessageBox();

        int minutos = Mathf.FloorToInt(Time.time / 60);
        int segundos = Mathf.FloorToInt(Time.time % 60);
        messageBox.messageShow("\n" + $"[{minutos:D2}:{segundos:D2}]: " + message + "\n", txtColor);
        Canvas.ForceUpdateCanvases();
        _mainInstance.StartCoroutine(_mainInstance.RepositionYScroll());
    }

    public void sendInputMessage()
    {
        if (consoleInputField.text != "")
        {
            ConsoleMessageBox messageBox = _mainInstance.getMessageBox();
            int minutos = Mathf.FloorToInt(Time.time / 60);
            int segundos = Mathf.FloorToInt(Time.time % 60);
            messageBox.messageShow("\n" + $"[{minutos:D2}:{segundos:D2}]: " + consoleInputField.text + "\n");
            if(EventManager.instance != null) EventManager.TriggerEvent("MSJ_" + consoleInputField.text);
            consoleInputField.text = "";
            Canvas.ForceUpdateCanvases();
            StartCoroutine(RepositionYScroll());
            consoleInputField.ActivateInputField();
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 0f;
            consoleInputField.ActivateInputField();
        }
    }

    public void clearConsole()
    {
        for(int i = 0; i < messagesPool.Count; i++)
        {
            if(i == 0)
            {
                messagesPool[0].TextBox.transform.SetAsFirstSibling();
                messagesPool[0].TextBox.SetActive(true);
                messagesPool[0].Message.color = Color.black;
                messagesPool[0].Message.text = "\nStart console input\n";
                Canvas.ForceUpdateCanvases();
                StartCoroutine(RepositionYScroll());
            } else messagesPool[i].TextBox.SetActive(false);
        }
    }

    private void openConsole()
    {
        consoleObject.SetActive(true);
        consoleObject.GetComponent<RectTransform>().transform.localPosition = Vector3.zero;
        consoleInputField.ActivateInputField();
    }

    public void closeConsole()
    {
        consoleObject.SetActive(false);
    }

    private IEnumerator RepositionYScroll()
    {
        for (int i = 0; i < 4; i++) yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f;
    }

}





[Serializable]
public class ConsoleMessageBox
{
    [SerializeField] private GameObject textBox;
    [SerializeField] private Text message;

    public ConsoleMessageBox(GameObject textBox, Text message)
    {
        this.textBox = textBox;
        this.message = message;
        textBox.SetActive(false);
    }

    public GameObject TextBox { get => textBox; }
    public Text Message { get => message; }

    public void messageShow(string msg, Color txtColor = default){
        textBox.transform.SetAsLastSibling();
        this.textBox.SetActive(true);
        if (txtColor == default) txtColor = Color.black;
        this.message.color = txtColor;
        this.message.text = msg;
    }
}