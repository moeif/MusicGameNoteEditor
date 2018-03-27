using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController Inst = null;
    public Canvas canvas;
    public Text textMusic;
    public Text textNote;
    public Button btnSave;
    public Button btnClear;
    public RawImage waveImg;
    public Text startTime;
    public Text endTime;
    public RectTransform audioPointer;
    public Text textPlayedTime;
    public RectTransform deletePointer;
    public RectTransform nodeArea;
    public GameObject objShortNodeSample;
    public GameObject objLongNodeSample;
    public Text textLongNodeHold;
    public NodeShow tapNode;
    public NodeShow holdNode;

    private float max = 1910;

    private GUIStyle style = new GUIStyle();

    private void Awake() {
        Inst = this;
    }

    private void Start() {
        btnSave.onClick.AddListener(OnClickSave);
        btnClear.onClick.AddListener(OnClickClear);

        string path = "D:/aaa.OGG";
        Debug.Log(System.IO.Path.GetExtension(path).ToLower());

        style.fontStyle = FontStyle.Bold;
        style.fontSize = 20;
        style.normal.textColor = Color.red;

        textLongNodeHold.gameObject.SetActive(false);
    }

    private void OnClickSave() {
        FileManager.Inst.SaveNodeData(NodeManager.Inst.nodeList);
    }

    private void OnClickClear() {
        NodeManager.Inst.ClearAllNode();
    }

    public void OnOpenMusic(string _path) {
        textMusic.text = "Music: " + _path;
    }

    public void OnOpenNote(string _path) {
        textNote.text = "Note: " + _path;
    }

    public void ShowAudioWave(Texture2D _tex) {
        waveImg.texture = _tex;
        waveImg.color = Color.white;
    }

    public void ShowAudioClipInfo(AudioClip _clip) {
        startTime.text = "0";
        endTime.text = _clip.length.ToString();
    }

    public void UpdateAudioPointer(float _playedPercentage) {
        float currentPosX = max * _playedPercentage;
        currentPosX = Mathf.Clamp(currentPosX, 0, max);
        audioPointer.anchoredPosition = new Vector2(currentPosX, 0);
    }

    public void UpdatePlayedTime(float _playedTime) {
        float mSeconds = _playedTime - (int)_playedTime;
        int playedTime = (int)_playedTime;
        int hours = playedTime / 3600;
        playedTime -= hours * 3600;
        int minutes = playedTime / 60;
        playedTime -= minutes * 60;
        int seconds = playedTime;
        textPlayedTime.text = hours.ToString().PadLeft(2, '0') + ":" + minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0') + "." + System.Math.Round(mSeconds, 3).ToString().Replace("0.","");
    }


    public float IsClickOnWaveformArea() {
        if (RectTransformUtility.RectangleContainsScreenPoint(waveImg.rectTransform, Input.mousePosition)) {
            float x = (Input.mousePosition / canvas.transform.localScale.x).x;
            float percentage = x / max;
            percentage = Mathf.Clamp(percentage,0, 1);
            return percentage;
        }

        return -1;
    }


    public void UpdateDelArea(float _startPercentage, float _currPercentage) {
        if(_currPercentage > _startPercentage) {
            float startX = max * _startPercentage;
            float currX = max * _currPercentage;
            float width = currX - startX;
            deletePointer.pivot = new Vector2(0, 1f);
            deletePointer.anchoredPosition = new Vector2(startX, 0);
            deletePointer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
        else if(_currPercentage < _startPercentage) {
            float startX = max * _startPercentage;
            float currX = max * _currPercentage;
            float width = startX - currX;
            deletePointer.pivot = new Vector2(1, 1f);
            deletePointer.anchoredPosition = new Vector2(startX, 0);
            deletePointer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        } else {
            float startX = max * _startPercentage;
            float width = 0;
            deletePointer.pivot = new Vector2(0, 1f);
            deletePointer.anchoredPosition = new Vector2(startX, 0);
            deletePointer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }

    public void ClearDelArea() {
        deletePointer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
    }

    public GameObject CreateShortNode(float _startPercentage) {
        float startX = max * _startPercentage;
        GameObject shortNode = Instantiate(objShortNodeSample);
        shortNode.transform.SetParent(nodeArea);
        shortNode.transform.localScale = Vector3.one;
        shortNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, 0);
        shortNode.SetActive(true);
        return shortNode;
    }


    public GameObject CreateLongNode(float _startPercentage) {
        float startX = max * _startPercentage;
        GameObject longNode = Instantiate(objLongNodeSample);
        longNode.transform.SetParent(nodeArea);
        longNode.transform.localScale = Vector3.one;
        longNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, 0);
        longNode.SetActive(true);
        return longNode;
    }

    public void UpdateLongNode(RectTransform rectTrans, float _startPercentage, float _endPercentage) {
        float startX = max * _startPercentage;
        float currx = max * _endPercentage;
        float width = currx - startX;
        width = Mathf.Clamp(width, 5, width);
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public void UpdateLongNodeHoldTime(float _holdTime) {
        if(_holdTime < 0) {
            textLongNodeHold.gameObject.SetActive(false);
        } else {
            if (textLongNodeHold.gameObject.activeSelf == false) {
                textLongNodeHold.gameObject.SetActive(true);
            }
            int iField = (int)_holdTime;
            float fField = _holdTime - iField;
            fField = (float)System.Math.Round(fField, 3);
            string sTime = iField.ToString() + "." + fField.ToString().Replace("0.", "").PadRight(3, '0');
            textLongNodeHold.text = sTime;
        }
    }
    

    public void TapNodeShow(float _showTime, int _index) {
        tapNode.Show(_showTime, _index);
    }

    public void HoldNodeShow(float _showTime, int _index) {
        holdNode.Show(_showTime, _index);
    }


    private void OnGUI() {
        float percentage = IsClickOnWaveformArea();
        if(percentage >= 0) {
            float time = AudioManager.Inst.GetTimeWithPercentage(percentage);
            time = (float)System.Math.Round(time, 3);
            GUI.Label(new Rect(Input.mousePosition.x + 20,Screen.height - Input.mousePosition.y, 80, 30), time.ToString(), style);
        }
    }



}
