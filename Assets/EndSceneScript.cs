using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EndSceneScript : MonoBehaviour
{
    string name;
    [SerializeField] FirebaseScript firebaseScript;
    [SerializeField] GameManagerScript gameManagerScript;
    [SerializeField] Button submitFinalScoreButton;
    public void ToggleSubmitScoreButton(string str)
    {
        submitFinalScoreButton.interactable = str != "";
        name = str;
    }
    public void SubmitFinalScore()
    {
        firebaseScript.SubmitFinalScore(name, gameManagerScript.GetGrandTotal());
        submitFinalScoreButton.interactable = false;
    }
    
}
