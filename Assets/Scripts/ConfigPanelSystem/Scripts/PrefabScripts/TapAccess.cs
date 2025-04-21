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
        _tapExplorer.CurrentTap.SetActive(false);
        _tapExplorer.CurrentTap = tapScrollView;
        tapScrollView.SetActive(true);
        _tapExplorer.tapExplorerCheck(_tapExplorer.getTapOrder(tapScrollView));
    }
}
