using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    int roundCounter = 13;
    [SerializeField] TMP_Text rerollText, upperSumText, lowerSumText, grandTotalText, finalScoreText, bonusText;
    [SerializeField] GameObject endScreen;
    [SerializeField] Button rerollButton, finalScoreSubmitButton, menuButton, leaderboardButton;
    [SerializeField] GameObject[] dieObj;
    [SerializeField] Sprite[] dieFaceInput;
    [SerializeField] FirebaseScript firebaseScript;
    string name;
    int[] upperSectionScores = new int[6];
    int _rolls = 3;
    int rolls
    {
        get { return _rolls; }
        set
        {
            _rolls = value;
            rerollText.text = string.Format("Reroll ({0} left)", value);
            rerollButton.interactable = rolls > 0;
        }
    }
    int _upperSum = 0;
    int _lowerSum = 0;
    int _grandTotal = 0;
    int upperSum
    {
        get { return _upperSum; }
        set
        {
            _upperSum = value;
            upperSumText.text = string.Format("Sum: {0}", value);
        }
    }
    int lowerSum
    {
        get { return _lowerSum; }
        set
        {
            _lowerSum = value;
            lowerSumText.text = string.Format("Sum: {0}", value);
        }
    }
    int grandTotal
    {
        get { return _grandTotal; }
        set
        {
            _grandTotal = value;
            grandTotalText.text = string.Format("Total: {0}", value);
        }
    }
    static Sprite[] dieFace = new Sprite[6];
    Die[] dice = new Die[5];
    bool _hasBonus = false;
    bool hasBonus
    {
        get { return _hasBonus; }
        set
        {
            _hasBonus = value;
            if (value)
                bonusText.text = "Bonus: 25";
            else
                bonusText.text = "Bonus: 0";
        }
    }
    public int GetGrandTotal()
    {
        return grandTotal;
    }
    [SerializeField] GameObject aces, twos, threes, fours, fives, sixes, threeOfAKind, fourOfAKind, yahtzee, chance, smallStr, longStr, fullHouse;
    Dictionary<GameObject, Category> categories = new Dictionary<GameObject, Category>();
    void Start()
    {

        foreach (GameObject gameObject in new GameObject[] { aces, twos, threes, fours, fives, sixes })
            categories.Add(gameObject, new Category(true));
        foreach (GameObject gameObject in new GameObject[] { threeOfAKind, fourOfAKind, yahtzee, chance, smallStr, longStr, fullHouse })
            categories.Add(gameObject, new Category(false));
        for (int i = 0; i < 5; i++)
            dice[i] = new Die(dieObj[i]);
        for (int i = 0; i < 6; i++)
            dieFace[i] = dieFaceInput[i];
        Roll(true);
    }

    class Die
    {
        public Die(GameObject gameObject)
        {
            this.gameObject = gameObject;
            image = gameObject.GetComponent<Image>();
        }
        int _value;
        public int value
        {
            get { return _value; }
            set
            {
                _value = value;
                image.sprite = dieFace[value-1];
            }
        }
        public Image image;
        public GameObject gameObject;
        bool _selected;
        public bool selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (value)
                    image.color = Color.green;
                else
                    image.color = Color.white;
            }
        }
    }
    class Category
    {
        public bool isUpper;
        public int score = 0;
        public Category(bool isUpper)
        {
            this.isUpper = isUpper;
        }
    }
    public void SubmitFinalScore()
    {
        firebaseScript.SubmitFinalScore(name, grandTotal);
        finalScoreSubmitButton.interactable = false;
        leaderboardButton.interactable = false;
        menuButton.interactable = false;
    }
    public void SetName(string name)
    {
        this.name = name;
        finalScoreSubmitButton.interactable = name != "";
    }
    void RoundEnd()
    {
        endScreen.SetActive(true);
        rerollButton.interactable = false;
        finalScoreText.text = string.Format("Score: {0}", grandTotal);
    }
    void Roll(bool rollAll = false)
    {
        for (int i = 0; i < 5; i++)
        {
            if (dice[i].selected || rollAll)
                dice[i].value = Random.Range(1, 7);
            dice[i].selected = false;
        }
        for (int i = 0; i < 5; i++)
            Debug.Log(string.Format("Die {0}: {1}", i, dice[i].value));
    }
    void SubmitScore(GameObject gameObject, int score)
    {
        gameObject.SetActive(false);
        GameObject parant = gameObject.transform.parent.gameObject;
        TMP_Text text = parant.GetComponent<TMP_Text>();
        text.text = string.Format("{0} {1}", text.text, score);
        Roll(true);
        rolls = 3;
        categories[gameObject].score = score;
        if (categories[gameObject].isUpper)
        {
            upperSum += score;
            if (score >= 63 && !hasBonus)
            {
                hasBonus = true;
                grandTotal += 35;
            }
        }
        else
            lowerSum += score;
        grandTotal += score;
        roundCounter--;
        if (roundCounter < 1)
            RoundEnd();

    }
    public void Reroll()
    {
        if (rolls > 0)
        {
            Roll();
            rolls--;
        }
    }
    public void ScoreUpper(int i)
    {
        GameObject gameObject = aces;
        switch (i)
        {
            case 1:
                gameObject = aces;
                break;
            case 2:
                gameObject = twos;
                break;
            case 3:
                gameObject = threes;
                break;
            case 4:
                gameObject = fours;
                break;
            case 5:
                gameObject = fives;
                break;
            case 6:
                gameObject = sixes;
                break;
        }
        int score = 0;
        foreach (Die die in dice)
            if (die.value == i)
                score+=i;
        SubmitScore(gameObject, score);
        upperSectionScores[i-1] = score;
    }
    public void ThreeOfKind()
    {
        GameObject gameObject = threeOfAKind;
        int score = 0;
        int[] values = new int[6];
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        for (int i = 0; i < 6; i++)
            if (values[i] > 2)
            {
                score = i * 3;
                break;
            }
        SubmitScore(gameObject, score);
    }
    public void FourOfAKind()
    {
        GameObject gameObject = fourOfAKind;
        int score = 0;
        int[] values = new int[6];
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        for (int i = 0; i < 6; i++)
            if (values[i] > 3)
            {
                score = i * 4;
                break;
            }
        SubmitScore(gameObject, score);
    }
    public void FullHouse()
    {
        GameObject gameObject = fullHouse;
        int score = 0;
        int[] values = new int[6];
        bool isThreeOfOne = false;
        bool isTwoOfOne = false;
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        for (int i = 0; i < 6; i++)
            if (values[i] == 3)
            {
                isThreeOfOne = true;
                break;
            }
        for (int i = 0; i < 6; i++)
            if (values[i] == 2)
            {
                isTwoOfOne = true;
                break;
            }
        if (isThreeOfOne && isTwoOfOne)
            score = 25;
        SubmitScore(gameObject, score);
    }
    public void SmallStr()
    {
        GameObject gameObject = smallStr;
        int score = 0;
        int[] values = new int[6];
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        bool success = false;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (values[i + j] < 1)
                    break;
                else if (j == 3)
                    success = true;
            }
            if (success)
                break;
        }
        if (success)
            score = 30;
        SubmitScore(gameObject, score);
    }
    public void LongStr()
    {
        GameObject gameObject = longStr;
        int score = 0;
        int[] values = new int[6];
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        bool success = false;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (values[i + j] < 1)
                    break;
                else if (j == 4)
                    success = true;
            }
            if (success)
                break;
        }
        if (success)
            score = 40;
        SubmitScore(gameObject, score);
    }
    public void Yahtzee()
    {
        GameObject gameObject = yahtzee;
        int score = 0;
        int[] values = new int[6];
        for (int i = 0; i < 6; i++)
            values[i] = 0;
        foreach (Die die in dice)
            values[die.value-1]++;
        bool success = false;
        for (int i = 0; i < 6; i++)
            if (values[i] == 5)
            {
                success = true;
                break;
            }
        if (success)
            score = 50;
        SubmitScore(gameObject, score);
    }
    public void Chance()
    {
        GameObject gameObject = chance;
        int score = 0;
        foreach (Die die in dice)
            score += die.value;
        SubmitScore(gameObject, score);
    }
    public void SelectDie(GameObject gameObject)
    {
        for (int i = 0; i < 5; i++)
            if (gameObject == dice[i].gameObject)
            {
                dice[i].selected = !dice[i].selected;
                break;
            }
    }
}
