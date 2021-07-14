using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingMaterialSOHolder : MonoBehaviour
{
    //originalSO 
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

    //list to hold the effects of this material
    public List<AllMaterialEffects> effectsList = new List<AllMaterialEffects>();

    //Start function for creting in content viewer
    //called as if it's awake, this is to prevent executing the code in Effect Choice UI
    public void ShowMaterialInViewer(CraftingMaterialSO materialSO)
    {
        //instantiate a copy for the instance's values
        //CraftingMaterialSO materialSO = Instantiate(materialSO);

        //assign the image in the SO to the prefab Icon
        materialImage.sprite = Resources.Load<Sprite>($"Materials/{materialSO.materialPrefix}{materialSO.materialType}");

        //assign name
        materialName.text = $"{materialSO.materialPrefix} {materialSO.materialType}";

        Transform effectTextParentTrans = effectTextParentObject.transform;
        //assign texts depending on the CraftingMaterialSO
        //count cap is 3
        for (int i = 0; materialSO.materialEffects.Count - 1 >= i; i++)
        {
            //the TMP is in a child of the child of parentObject
            //child 0 is the TMP
            TextMeshProUGUI effectText = effectTextParentTrans.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = CardTagManager.GetMaterialEffectDescription(materialSO.materialEffects[i]);
            effectTextList.Add(effectText);
            //store the effects from the CraftingMaterialSO in the local list
            effectsList.Add(materialSO.materialEffects[i]);
        }

        // assign the original to the holder script
        craftingMaterialSO = materialSO;
    }



    //function used in the material choose effect script
    //will transfer the identified sprite and text in the content view to the choose UI
    public void TransferToChooseEffectUI(CraftingMaterialSO receivedSO)
    {
        //create a copy for the instance's values
        //CraftingMaterialSO receivedSO = Instantiate(receivedSO);


        //assign the image in the SO to the prefab Icon
        materialImage.sprite = Resources.Load<Sprite>($"Materials/{receivedSO.materialPrefix}{receivedSO.materialType}");
        //assign name
        materialName.text = $"{receivedSO.materialPrefix} {receivedSO.materialType}";

        Transform effectTextParentTrans = effectTextParentObject.transform;
        //assign texts depending on the CraftingMaterialSO
        //count cap is 3
        for (int i = 0; receivedSO.materialEffects.Count - 1 >= i; i++)
        {
            //the TMP is in a child of the child of parentObject
            //child 0 is the TMP
            TextMeshProUGUI effectText = effectTextParentTrans.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = CardTagManager.GetMaterialEffectDescription(receivedSO.materialEffects[i]);
            effectTextList.Add(effectText);
            //store the effects from the CraftingMaterialSO in the local list
            effectsList.Add(receivedSO.materialEffects[i]);
            //makes it so that the only clickable effects are those assigned with text
            Button effectButton = effectTextParentObject.transform.GetChild(i).GetComponent<Button>();
            effectButton.interactable = true;
        }

        //assign the original
        craftingMaterialSO = receivedSO;
    }

    //will be used to clear a material prefab's texts and objects
    public void OnDisable()
    {
        foreach (TextMeshProUGUI text in effectTextList)
        {
            text.text = "";
        }
    }



}
