using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour {
    public static NodeManager Inst = null;

    public Node currNode = null;

    public List<Node> nodeList = new List<Node>();

    private bool longNodeMode = false;

    private void Awake() {
        Inst = this;
    }

    private void Update() {

        if (GlobalState.viewMode) {
            return;
        }

        if (!AudioManager.Inst.IsPlaying()) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            CreateShortNode();
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            StartCreateLongNode();
        }

        if (longNodeMode) {
            UpdateLongNode();
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            EndCreateLongNode();
        }

    }

    private void CreateShortNode() {
        float playedTime = AudioManager.Inst.GetPlayedTime();
        float playedPercentage = AudioManager.Inst.GetPlayedPercentage(playedTime);
        GameObject shortNodeObj = UIController.Inst.CreateShortNode(playedPercentage);
        Node node = shortNodeObj.AddComponent<Node>();
        node.InitNode(En_NodeType.Short, playedTime);
        node.startPercentage = playedPercentage;
        node.rectTrans = shortNodeObj.GetComponent<RectTransform>();
        //nodeList.Add(node);
        AddNode(node);
    }

    public void CreateShortNode(float _startTime, float _percentage) {
        float playedTime = _startTime;
        float playedPercentage = _percentage;
        GameObject shortNodeObj = UIController.Inst.CreateShortNode(playedPercentage);
        Node node = shortNodeObj.AddComponent<Node>();
        node.InitNode(En_NodeType.Short, playedTime);
        node.startPercentage = playedPercentage;
        node.rectTrans = shortNodeObj.GetComponent<RectTransform>();
        //nodeList.Add(node);
        AddNode(node);
    }

    public void CreateLongNode(float _palyedTime, float _percentage, float _endTime) {
        float playedTime = _palyedTime;
        float playedPercentage = _percentage;
        float endTime = _endTime;
        float totalTime = playedTime / playedPercentage;
        float endPercentage = _endTime / totalTime;
        GameObject longNodeObj = UIController.Inst.CreateLongNode(playedPercentage);
        Node node = longNodeObj.AddComponent<Node>();
        node.InitNode(En_NodeType.Long, playedTime);
        node.startPercentage = playedPercentage;
        node.endTime = _endTime;
        node.rectTrans = longNodeObj.GetComponent<RectTransform>();
        UIController.Inst.UpdateLongNode(node.rectTrans, node.startPercentage, endPercentage);
        //nodeList.Add(node);
        AddNode(node);
    }

    private void StartCreateLongNode() {
        longNodeMode = true;
        float playedTime = AudioManager.Inst.GetPlayedTime();
        float playedPercentage = AudioManager.Inst.GetPlayedPercentage(playedTime);
        GameObject longNodeObj = UIController.Inst.CreateLongNode(playedPercentage);
        Node node = longNodeObj.AddComponent<Node>();
        node.InitNode(En_NodeType.Long, playedTime);
        node.startPercentage = playedPercentage;
        //nodeList.Add(node);
        AddNode(node);
        currNode = node;
        node.rectTrans = longNodeObj.GetComponent<RectTransform>();
    }

    private void UpdateLongNode() {
        float playedTime = AudioManager.Inst.GetPlayedTime();
        float playedPercentage = AudioManager.Inst.GetPlayedPercentage(playedTime);
        currNode.UpdateEndTime(playedTime);
        UIController.Inst.UpdateLongNode(currNode.rectTrans, currNode.startPercentage, playedPercentage);
        float holdTime = currNode.endTime - currNode.startTime;
        UIController.Inst.UpdateLongNodeHoldTime(holdTime);
    }

    private void AddNode(Node _node) {
        //int insertTo = 0;
        //for(int i = 0; i < nodeList.Count; ++i) {
        //    Node node = nodeList[i];
        //    if(_node.startTime < node.startTime) {
        //        Debug.Log("Insert into : " + i + "  " + _node.startTime + "  " + node.startTime);
        //        insertTo = i;
        //        break;
        //    }
        //}

        //nodeList.Insert(insertTo, _node);
        nodeList.Add(_node);

        nodeList.Sort((Node a, Node b) => {
            if(a.startTime < b.startTime) {
                return -1;
            } else {
                return 1;
            }
        });

        for(int i = 0; i < nodeList.Count; ++i) {
            nodeList[i].index = i;
        }
    }

    private void EndCreateLongNode() {
        longNodeMode = false;
        currNode = null;
        UIController.Inst.UpdateLongNodeHoldTime(-1);
    }

    public void DeleteNode(float _startTime, float _endTime) {
        for(int i = nodeList.Count - 1; i >= 0; --i) {
            Node node = nodeList[i];
            if(node.nodeType == En_NodeType.Short) {
                if(node.startTime >= _startTime && node.startTime <= _endTime) {
                    GameObject.Destroy(node.rectTrans.gameObject);
                    nodeList.RemoveAt(i);
                }
            } else {
                if((node.startTime >= _startTime && node.startTime <= _endTime) ||
                   (node.endTime >= _startTime && node.endTime <= _endTime) ||
                   (_startTime >= node.startTime && _endTime <= node.endTime)) {
                    Destroy(node.rectTrans.gameObject);
                    nodeList.RemoveAt(i);
                }
            }
        }
    }

    public void ClearAllNode() {
        for(int i = 0; i < nodeList.Count; ++i) {
            Destroy(nodeList[i].rectTrans.gameObject);
        }

        nodeList.Clear();
    }

    
}
