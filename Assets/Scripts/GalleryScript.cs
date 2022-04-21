using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryScript : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image displayWindow;
    [SerializeField] Button leftButton, rightButton;
    [SerializeField] TMP_Text pageIndicatorText;
    int _currentImage;
    int currentImage
    {
        get { return _currentImage; }
        set
        {
            _currentImage = value;
            displayWindow.sprite = sprites[value];

            leftButton.interactable = value != 0;
            rightButton.interactable = value != 7;

            pageIndicatorText.text = $"page {value+1}/8";
        }
    }
    public void ChangeImage(int i)
    {
        currentImage += i;
    }

}
