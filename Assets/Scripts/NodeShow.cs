using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeShow : MonoBehaviour {
    public Text textNum;
    public bool longNode;
    private float showTime = 0.0f;

    private Image img;

    private float fillDelte = 0.0f;

    private void Start() {
        img = gameObject.GetComponent<Image>();
    }

    public void Show(float _showTime, int _index) {
        showTime = _showTime;
        img.enabled = true;
        textNum.text = _index.ToString();

        if (longNode) {
            img.fillAmount = 0;
            fillDelte = 1.0f / _showTime;
        }
    }

    private void Update() {
        if (img.enabled == true) {
            if (longNode) {
                img.fillAmount += fillDelte * Time.deltaTime;
            }

            showTime -= Time.deltaTime;
            if (showTime <= 0) {
                img.enabled = false;
            }
        }
    }
}
