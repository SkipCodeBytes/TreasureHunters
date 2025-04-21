using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1f;
    private Text _text;
    private float _alpha = 0f;
    private bool _isDownward = false;
    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        if (_isDownward)
        {
            _alpha -= Time.deltaTime * fadeSpeed;
            if (_alpha <= 0f)
            {
                _alpha = 0f;
                _isDownward = !_isDownward;
            }
        }
        else
        {
            _alpha += Time.deltaTime * fadeSpeed;
            if (_alpha >= 1f)
            {
                _alpha = 1f;
                _isDownward = !_isDownward;
            }
        }

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _alpha);
    }
}
