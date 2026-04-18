using UnityEngine;
using TMPro;
using System.Collections;

public class NPCOrderUI : MonoBehaviour
{
    public GameObject orderCanvas;    
    public TextMeshProUGUI nameText;  

    public float waitSeconds = 5.0f;     
    public float displaySeconds = 5.0f;  

    private RecipeData myPrivateOrder;

    private void Awake()
    {
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }

    public void StartOrderProcess()
    {
        StartCoroutine(IndependentOrderRoutine());
    }

    private IEnumerator IndependentOrderRoutine()
    {
        yield return new WaitForSeconds(waitSeconds);

        if (AlchemyManager.Instance != null)
        {
            myPrivateOrder = AlchemyManager.Instance.GetRandomUnlockedRecipe();

            if (myPrivateOrder != null)
            {
                ShowOrder(myPrivateOrder);
            }
        }

        yield return new WaitForSeconds(displaySeconds);
        HideOrder();
    }

    public void ShowOrder(RecipeData recipe)
    {
        if (orderCanvas == null || nameText == null || recipe == null) return;

        nameText.text = "I need " + recipe.potionName;
        orderCanvas.SetActive(true);
    }

    public void HideOrder()
    {
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }

    public RecipeData GetMyOrder()
    {
        return myPrivateOrder;
    }
}