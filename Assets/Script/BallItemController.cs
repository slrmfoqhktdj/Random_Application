using System.Diagnostics;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public class BallItemController
{
    List<Label> _numberLabels;
    List<int> _numberList;
    List<Color> _matchColors;


    public void SetVisualElement(VisualElement visualElement)
    {
        _numberLabels = new List<Label>()
        {
            visualElement.Q<Label>("number1"),
            visualElement.Q<Label>("number2"),
            visualElement.Q<Label>("number3"),
            visualElement.Q<Label>("number4"),
            visualElement.Q<Label>("number5"),
            visualElement.Q<Label>("number6"),
            visualElement.Q<Label>("number7")
        };

        _numberList = new List<int>();

        _matchColors = new List<Color>()
        {
            new Color(247f / 255f, 234f / 255f, 110f / 255f),   // 1~10 노랑
            new Color(121f / 255f, 170f / 255f, 255f / 255f),   // 11~20 파랑
            new Color(255f / 255f, 113f / 255f, 113f / 255f),   // 21~30 빨강
            new Color(147f / 255f, 147f / 255f, 147f / 255f),   // 31~40 회색
            new Color(152f / 255f, 247f / 255f, 145f / 255f)    // 41~45 연두
        };
    }

    public void SetNumber(int number01, int number02, int number03, int number04, int number05, int number06, int number07)
    {
        _numberLabels[0].text = number01.ToString();
        _numberLabels[1].text = number02.ToString();
        _numberLabels[2].text = number03.ToString();
        _numberLabels[3].text = number04.ToString();
        _numberLabels[4].text = number05.ToString();
        _numberLabels[5].text = number06.ToString();
        _numberLabels[6].text = number07.ToString();
        _numberList = new List<int>() { number01, number02, number03, number04, number05, number06, number07 };
    }

    public void SetColorMatchNumber(List<int> winningNumbers)
    {
        if(winningNumbers == null || winningNumbers.Count == 0 || _numberList == null || _numberList.Count == 0)
            return;

        for(int i = 0; i < _numberList.Count; i++)
        {
            for(int j = 0; j < winningNumbers.Count; j++)
            {
                if(_numberList[i] == winningNumbers[j])
                {
                    var matchcolor = _matchColors[0];

                    if (_numberList[i] <= 20)
                    {
                        matchcolor = _matchColors[1];
                    }
                    else if (_numberList[i] <= 30)
                    {
                        matchcolor = _matchColors[2];
                    }
                    else if (_numberList[i] <= 40)
                    {
                        matchcolor = _matchColors[3];
                    }
                    else if (_numberList[i] <= 45)
                    {
                        matchcolor = _matchColors[4];
                    }

                    _numberLabels[i].style.backgroundColor = matchcolor;
                        
                    break;
                }
            }

        
        }
    }

}
