using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceShowController : MonoBehaviour {

    public static DanceShowController Inst = null;

    public RectTransform rectTrans;
    public Text textMode;
    public InputField iSpeed;
    public GameObject shortSample;
    public GameObject longSample;

    private float moveSpeed = 20;        // 每秒移动多少像素
    private float musicLength = 0.0f;    // 音乐时长(秒)
    private List<Node> nodeList = null;
    private float flowAreaWidth = 0.0f;
    private float ticker = 0.0f;
    private bool started = false;

    private int currIndex = -1;

    private List<GameObject> viewNodeList = new List<GameObject>();

    private void Awake() {
        Inst = this;
    }

    private void ClearViewNodes() {
        for(int i = 0; i < viewNodeList.Count; ++i) {
            Destroy(viewNodeList[i]);
        }

        viewNodeList.Clear();
    }

    public void Init(float _musicLength, List<Node> _nodeList) {
        ClearViewNodes();
        moveSpeed = -1;
        if (!string.IsNullOrEmpty(iSpeed.text)) {
            int speed = 0;
            if(int.TryParse(iSpeed.text, out speed)) {
                moveSpeed = speed;
            }
        } 

        if(moveSpeed < 0) {
            moveSpeed = 100;
        }

        iSpeed.text = moveSpeed.ToString();

        musicLength = _musicLength;
        nodeList = _nodeList;
        flowAreaWidth = moveSpeed * musicLength;
        rectTrans.anchoredPosition = new Vector2(0, 0);
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, flowAreaWidth);

        if(nodeList == null) {
            return;
        }

        for(int i = 0; i < nodeList.Count; ++i) {
            Node node = nodeList[i];
            if(node.nodeType == En_NodeType.Short) {
                GameObject obj = Instantiate(shortSample);
                obj.transform.SetParent(rectTrans);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                RectTransform nodeRectTrans = obj.GetComponent<RectTransform>();
                nodeRectTrans.anchoredPosition = new Vector2(node.startTime * moveSpeed - 10, 0);
                Text numText = nodeRectTrans.Find("Num").GetComponent<Text>();
                numText.text = node.index.ToString();
                numText.gameObject.SetActive(true);
                viewNodeList.Add(obj);
            } else {
                GameObject obj = Instantiate(longSample);
                obj.transform.SetParent(rectTrans);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                RectTransform nodeRectTrans = obj.GetComponent<RectTransform>();
                nodeRectTrans.anchoredPosition = new Vector2(node.startTime * moveSpeed - 10, 0);
                float width = (node.endTime - node.startTime) * moveSpeed;
                nodeRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                Text numText = nodeRectTrans.Find("Num").GetComponent<Text>();
                numText.text = node.index.ToString();
                numText.gameObject.SetActive(true);
                Text dText = nodeRectTrans.Find("Duration").GetComponent<Text>();
                dText.text = System.Math.Round((node.endTime - node.startTime), 3).ToString();
                dText.gameObject.SetActive(true);
                viewNodeList.Add(obj);
            }
        }
    }

    //private void Start() {
    //    Init(50, 60, null);
    //}

    private void Update() {

        if(GlobalState.viewMode == false) {
            textMode.text = "<color=#FB4400FF>打点模式</color>";
            return;
        } else {
            textMode.text = "<color=#17C5FFFF>预览模式</color>";
        }

        if(AudioManager.Inst.IsPlaying() == false) {
            return;
        }

        ticker += Time.deltaTime;

        Vector2 pos = rectTrans.anchoredPosition;
        pos.x = -AudioManager.Inst.GetPlayedTime() * moveSpeed;
        rectTrans.anchoredPosition = pos;
    }
}
