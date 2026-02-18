using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class captionPanelTurnerOffer : MonoBehaviour
{
    GameObject captions;
    Image myRenderer;
    [SerializeField] TextMeshProUGUI captionText;

    private void Start()
    {
        captions = transform.GetChild(0).gameObject;
        myRenderer = GetComponent<Image>();
    }

    void Update()
    {
        if(captions.activeSelf == false || captionText.text == "" || captionText.text == null)
        {
            myRenderer.enabled = false;
        }
        else
        {
            myRenderer.enabled = true;
        }
    }
}
