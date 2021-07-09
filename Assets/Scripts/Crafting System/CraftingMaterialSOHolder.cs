using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingMaterialSOHolder : MonoBehaviour
{
    public CraftingMaterialSO craftingMaterialSO;

    //assiged in editor of prefab
    //sprite icon of material
    public Image materialImage;
    //texts in the prefab
    //holder of the TMPros, 
    public GameObject effectTextParentObject;
    //this this for easily transferring the texts from the content prefab to the chooseUI prefab
    public List<TextMeshProUGUI> effectTextList;
    //text for the name of material
    public TextMeshProUGUI materialName;

    //buttons to be made interactable in Transfer Function
    public Button effectButton1;
    public Button effectButton2;
    public Button effectButton3;

    //contains the chosen effect after choosing the material
    public AllMaterialEffects chosenEffect;


    private void Awake()
    {
        //assign the image in the SO to the prefab Icon
        materialImage.sprite = Resources.Load<Sprite>($"Materials/{craftingMaterialSO.materialPrefix}{craftingMaterialSO.materialType}");

        //assign name
        materialName.text = $"{craftingMaterialSO.materialPrefix} {craftingMaterialSO.materialType}";

        Transform effectTextParentTrans = effectTextParentObject.transform;
        //assign texts depending on the CraftingMaterialSO
        //count cap is 3
        for (int i = 0; craftingMaterialSO.materialEffects.Count-1 >= i; i++)
        {
            //the TMP is in a child of the child of parentObject
            //child 0 is the TMP
            TextMeshProUGUI effectText = effectTextParentTrans.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = CardTagManager.GetMaterialEffectDescription(craftingMaterialSO.materialEffects[i]);
            effectTextList.Add(effectText);
        }

    }



    //function used in the material choose effect script
    //will transfer the identified sprite and text in the content view to the choose UI
    public void TransferToChooseEffectUI(CraftingMaterialSOHolder receivedHolder)
    {

        materialImage.sprite = receivedHolder.materialImage.sprite;
        materialName.text = receivedHolder.materialName.text;
        for (int i = 0; receivedHolder.effectTextList.Count - 1 >= i; i++)
        {
            effectTextParentObject.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = receivedHolder.effectTextList[i].text;
            //makes it so that the only clickable effects are those assigned with text
            effectTextParentObject.GetComponent<Button>().interactable = true;
        }

    }

}
