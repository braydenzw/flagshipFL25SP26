using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPromptMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;

    // Update is called once per frame
    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}
