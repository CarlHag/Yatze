using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MenuManagerScript : MonoBehaviour
{
    string player;
    [SerializeField] TMP_InputField nameField;
    private void Start()
    {
        nameField.onEndEdit.AddListener(SumbitNane);
        
    }

    private void SumbitNane(string arg0)
    {
        
    }
}
