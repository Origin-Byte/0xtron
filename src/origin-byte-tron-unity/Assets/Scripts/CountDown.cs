using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CountDown : MonoBehaviour
{
    public int countDownFrom = 10;
    private int _counter;
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _counter = countDownFrom;
        _text.text = _counter.ToString();
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (_counter > 0)
        {
            _text.text = _counter.ToString();
            _counter--;
            yield return new WaitForSeconds(1);
        }
        
        gameObject.SetActive(false);
    }
}
