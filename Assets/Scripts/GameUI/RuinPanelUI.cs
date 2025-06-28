using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuinPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button btnSubmitObject;

    [SerializeField] private Transform gemItemsContent;
    [SerializeField] private List<Image> itemNeededImage;

    private GameManager _gm;

    public void StartRuinPanel()
    {
        _gm = GameManager.Instance;

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("OpenPanel"));

        for (int i = 0; i < itemNeededImage.Count; i++)
        {
            itemNeededImage[i].sprite = RuinsTile.GemsNeeded[i].GemItemImage;
        }

        PlayerInventory player = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory;

        for (int i = gemItemsContent.childCount - 1; i >= 0; i--)
        {
            Destroy(gemItemsContent.GetChild(i).gameObject);
        }


        bool[] _gemCheck = new bool[4];

        btnSubmitObject.interactable = false;

        for (int i = 0; i < player.GemItems.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gemItemsContent);
            RectTransform rectSlot = slot.GetComponent<RectTransform>();
            rectSlot.anchoredPosition = new Vector2(0f, -15f - (i * 85));

            rectSlot.GetChild(0).GetComponent<Image>().sprite = player.GemItems[i].Icon;
            rectSlot.GetChild(1).GetComponent<Text>().text = player.GemItems[i].ItemName;

            int itemId = ItemManager.Instance.GetItemID(player.GemItems[i]);
            
            for(int j = 0; j < RuinsTile.GemsNeededID.Length; j++)
            {
                if (_gemCheck[j]) continue;
                if (RuinsTile.GemsNeededID[j] == itemId)
                {
                    _gemCheck[j] = true;
                    break;
                }
            }

        }

        for (int i = 0; i < _gemCheck.Length; i++)
        {
            if (_gemCheck[i] == false) return;
        }

        btnSubmitObject.interactable = true;
    }

    public void btnSkip()
    {
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
        _gm.GmView.RPC("CloseWaitPanel", Photon.Pun.RpcTarget.All);
    }

    public void btnSubmit()
    {
        //RPC para perder objetos del inventario, brindar una estrella y cambiar GemsNeeded
    }
}
