using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapAccess : MonoBehaviour
{
    [SerializeField] private GameObject tapScrollView;
    private TapExplorer _tapExplorer;
    private Button _button;


    public GameObject TapScrollView { get => tapScrollView; }

    // Start is called before the first frame update
    void Start()
    {
        _tapExplorer = transform.parent.GetComponent<TapExplorer>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OpenTap());
    }

    public void OpenTap()
    {
        if (tapScrollView == null) return;
        //_tapExplorer.GetComponent<Image>().color = GetComponent<Image>().color;
        _tapExplorer.CurrentTapContent.SetActive(false);
        _tapExplorer.CurrentTapContent = tapScrollView;
        //_tapExplorer.CurrentTap.GetComponent<Image>().color = _tapExplorer.SelectedTapColor;

        tapScrollView.SetActive(true);
        _tapExplorer.tapExplorerCheck(_tapExplorer.getTapOrder(tapScrollView));
    }
}
