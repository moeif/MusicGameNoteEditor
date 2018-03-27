using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum En_NodeType {
    Short,
    Long,
}

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public float startTime = -1;
    public float endTime = -1;
    public float startPercentage = -1;
    public En_NodeType nodeType = En_NodeType.Short;
    public RectTransform rectTrans;
    public int index = 0;

    private GUIStyle style = new GUIStyle();

    private void Start() {
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.gray;
    }

    private bool showInfo = false;

    public void InitNode(En_NodeType _nodeType, float _startTime) {
        nodeType = _nodeType;
        startTime = (float)System.Math.Round(_startTime, 3);
    }

    public void UpdateEndTime(float _endTime) {
        endTime = (float)System.Math.Round(_endTime, 3);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        showInfo = true;
        //Vector2 pos = rectTrans.anchoredPosition;
        //pos.y = -2;
        //rectTrans.anchoredPosition = pos;
        rectTrans.localScale = new Vector3(1f, 1.2f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        showInfo = false;
        //Vector2 pos = rectTrans.anchoredPosition;
        //pos.y = 0;
        //rectTrans.anchoredPosition = pos;
        rectTrans.localScale = new Vector3(1f, 1f, 1f);
    }

    void OnGUI() {
        if (!showInfo) {
            return;
        }
        Rect rect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y + 30, 300, 30);
        if (nodeType == En_NodeType.Short) {
            GUI.Label(rect, "Index: " + index + "  Start: " + startTime, style);
        } else {
            GUI.Label(rect, "Index: " + index + "  Start: " + startTime + "  End: " + endTime + "  Duration: " + (endTime - startTime), style); 
        }
    }
}
