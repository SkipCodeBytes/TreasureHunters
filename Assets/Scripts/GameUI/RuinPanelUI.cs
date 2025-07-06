using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuinPanelUI : MonoBehaviour
{
    [SerializeField] private SantuaryItemSlot itemSlotPrefab;
    [SerializeField] private Button btnSubmitObject;

    [SerializeField] private Transform gemItemsContent;
    [SerializeField] private List<Image> itemNeededImage;

    private GameManager _gm;

    [SerializeField] private List<SantuaryItemSlot> itemSlots = new List<SantuaryItemSlot>();
    [SerializeField] private List<int> selectedSlotsIndex = new List<int>();

    //Cristal / Dimanate / Esmeralda / Estrella
    [SerializeField] private int[] formNeededQuanty = new int[4];

    //Amarillo / Azul / Rojo / Verde
    [SerializeField] private int[] colorNeededQuanty = new int[4];

    public void StartRuinPanel()
    {
        _gm = GameManager.Instance;

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("OpenPanel"));

        btnSubmitObject.interactable = false;

        Array.Clear(formNeededQuanty, 0, formNeededQuanty.Length);
        Array.Clear(colorNeededQuanty, 0, colorNeededQuanty.Length);

        //Muestra las 4 gemas que necesita
        for (int i = 0; i < itemNeededImage.Count; i++)
        {
            itemNeededImage[i].sprite = RuinsTile.GemsNeeded[i].GemItemImage;

            string gemName = RuinsTile.GemsNeeded[i].ItemName;
            string[] splitGemName = gemName.Split(' ');
            string forma = splitGemName[0];
            string color = splitGemName[1];

            switch (forma) 
            {
                case "Cristal":
                    formNeededQuanty[0]++;
                    break;
                case "Diamante":
                    formNeededQuanty[1]++;
                    break;
                case "Esmeralda":
                    formNeededQuanty[2]++;
                    break;
                case "Estrella":
                    formNeededQuanty[3]++;
                    break;
                default:
                    Debug.LogError("Forma no encontrada: " + forma);
                    break;
            }

            switch (color)
            {
                case "Amarillo":
                    colorNeededQuanty[0]++;
                    break;
                case "Azul":
                    colorNeededQuanty[1]++;
                    break;
                case "Rojo":
                    colorNeededQuanty[2]++;
                    break;
                case "Verde":
                    colorNeededQuanty[3]++;
                    break;
                default:
                    Debug.LogError("Color no encontrado: " + color);
                    break;
            }
        }

        //Vaciamos el content
        for (int i = gemItemsContent.childCount - 1; i >= 0; i--)
        {
            Destroy(gemItemsContent.GetChild(i).gameObject);
        }
        itemSlots.Clear();
        selectedSlotsIndex.Clear();

        //Creamos la lista de gemas del jugador
        PlayerInventory player = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory;

        for (int i = 0; i < player.GemItems.Count; i++)
        {
            SantuaryItemSlot item = Instantiate(itemSlotPrefab.gameObject, gemItemsContent).GetComponent<SantuaryItemSlot>();
            itemSlots.Add(item);
            item.SetElement(this, player.GemItems[i]);
        }
    }


    public bool AddToSelectElement(SantuaryItemSlot slotRef)
    {
        int slotId = itemSlots.IndexOf(slotRef);
        if (!selectedSlotsIndex.Contains(slotId))
        {
            if (selectedSlotsIndex.Count < 4) {

                if (selectedSlotsIndex.Count == 3)
                {
                    btnSubmitObject.interactable = true;
                }

                selectedSlotsIndex.Add(slotId);
                slotRef.SetToSelectedElement();
                return true; 
            }
            else
            {
                btnSubmitObject.interactable = true;
                NotificationUI.Instance.SetMessage("Solo puedes elegir 4 gemas", Color.yellow);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void RemoveSelectElement(SantuaryItemSlot slotRef)
    {
        int slotId = itemSlots.IndexOf(slotRef);
        selectedSlotsIndex.Remove(slotId);
        btnSubmitObject.interactable = false;
        slotRef.SetToUnselectedElement();
    }


    public void btnSkip()
    {
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
        _gm.GmView.RPC("CloseWaitPanel", Photon.Pun.RpcTarget.All);
    }


    public void btnSubmit()
    {
        //Cristal / Dimanate / Esmeralda / Estrella
        int[] formQuanty = new int[4];

        //Amarillo / Azul / Rojo / Verde
        int[] colorQuanty = new int[4];

        int[] selectedGemsId = new int[4];

        for (int i = 0; i < selectedSlotsIndex.Count; i++)
        {
            string gemName = itemSlots[selectedSlotsIndex[i]].gemData.ItemName;

            selectedGemsId[i] = ItemManager.Instance.GetItemID(itemSlots[selectedSlotsIndex[i]].gemData);

            string[] splitGemName = gemName.Split(' ');
            string forma = splitGemName[0];
            string color = splitGemName[1];


            switch (forma)
            {
                case "Cristal":
                    formQuanty[0]++;
                    break;
                case "Diamante":
                    formQuanty[1]++;
                    break;
                case "Esmeralda":
                    formQuanty[2]++;
                    break;
                case "Estrella":
                    formQuanty[3]++;
                    break;
                default:
                    Debug.LogError("Forma no encontrada: " + forma);
                    break;
            }

            switch (color)
            {
                case "Amarillo":
                    colorQuanty[0]++;
                    break;
                case "Azul":
                    colorQuanty[1]++;
                    break;
                case "Rojo":
                    colorQuanty[2]++;
                    break;
                case "Verde":
                    colorQuanty[3]++;
                    break;
                default:
                    Debug.LogError("Color no encontrado: " + color);
                    break;
            }
        }

        bool formCheck = true;
        bool colorCheck = true;

        for (int i = 0; i < 4; i++)
        {
            if (formNeededQuanty[i] != formQuanty[i]) formCheck = false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (colorNeededQuanty[i] != colorQuanty[i]) colorCheck = false;
        }

        if(formCheck || colorCheck)
        {
            _gm.GmView.RPC("SyncroSubmitRuins", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex, selectedGemsId[0], selectedGemsId[1], selectedGemsId[2], selectedGemsId[3]);
            gameObject.SetActive(false);
        }
        else
        {
            NotificationUI.Instance.SetMessage("Deben coincidir en FORMA o en COLOR a las gemas mostradas", Color.green);
        }

    }
}
