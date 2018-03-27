using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileManager : MonoBehaviour {

    public static FileManager Inst = null;

    private string mp3Path = "";
    private string notePath = "";

    private void Awake() {
        Inst = this;
    }

    private void Start() {
        //OnOpenFile(@"D:\audio\start-cut.OGG");
        //OnOpenFile(@"01_Oops.mp3");
        //OnOpenFile(@"D:\audio\Ji le jing tu  - 2'37(bpm131).csv");
    }


    public void OnOpenFile(string _path) {
        string extension = System.IO.Path.GetExtension(_path).ToLower();
        switch (extension) {
            case ".ogg": {
                    LoadAudio(_path);
                }
                break;
            case ".csv": {
                    LoadNote(_path);
                }
                break;
            default: {
                    //UnityEditor.EditorUtility.DisplayDialog("Error", "不支持的文件格式", "OK");
                }
                break;
        }
    }

    private void LoadAudio(string _path) {
        mp3Path = _path;
        StartCoroutine(_LoadAudio(_path));
    }

    IEnumerator _LoadAudio(string _path) {
        WWW www = new WWW("file:///" + _path);
        yield return www;
        AudioClip clip = www.GetAudioClip();
        while(clip.loadState != AudioDataLoadState.Loaded) {
            yield return www;
        }


        UIController.Inst.OnOpenMusic(_path);
        AudioManager.Inst.AudioReady(www.GetAudioClip(false, false));
    }


    private void LoadNote(string _path) {
        notePath = _path;
        StartCoroutine(_LoadNote(_path));
    }

    IEnumerator _LoadNote(string _path) {
        WWW www = new WWW("file:///" + _path);
        yield return www;
        InitNodeData(www.text);
        UIController.Inst.OnOpenNote(_path);
    }

    public void SaveNodeData(List<Node> _nodeList) {
        string dataText = "开始时间,结束时间,时间比例,节点类型\n";
        for(int i = 0; i < _nodeList.Count; ++i) {
            Node node = _nodeList[i];
            string str = string.Format("{0},{1},{2},{3}", node.startTime, node.endTime, node.startPercentage, (int)node.nodeType);
            dataText += str;
            Debug.Log("Str:" + str);
            if(i + 1 != _nodeList.Count) {
                dataText += '\n';
            }
        }

        if (string.IsNullOrEmpty(this.mp3Path)) {
            return;
        }

        string mp3Path = System.IO.Path.GetDirectoryName(this.mp3Path);
        string mp3Name = System.IO.Path.GetFileNameWithoutExtension(this.mp3Path);
        Debug.Log(mp3Path + "  " + mp3Name);
        string notePath = System.IO.Path.Combine(mp3Path, mp3Name + ".csv");

        System.IO.File.WriteAllText(notePath, dataText, System.Text.Encoding.UTF8);
        Debug.Log(notePath);
        this.notePath = notePath;
        UIController.Inst.OnOpenNote(this.notePath + "  <color=red>保存成功</color>");
        StartCoroutine(TextRecover());
    }

    IEnumerator TextRecover() {
        yield return new WaitForSeconds(2.0f);
        UIController.Inst.OnOpenNote(this.notePath);
    }

    private void InitNodeData(string dataText) {
        NodeManager.Inst.ClearAllNode();
        string[] lines = dataText.Split('\n');
       for(int i = 1; i < lines.Length; ++i) {
            string line = lines[i];
            if (string.IsNullOrEmpty(line)) {
                continue;
            }
            string[] field = line.Split(',');
            if (field != null && field.Length > 0) {
                Debug.Log(line);
                float startTime = float.Parse(field[0]);
                float endTime = float.Parse(field[1]);
                float startPercentage = float.Parse(field[2]);
                En_NodeType nodeType = (En_NodeType)int.Parse(field[3]);

                if (nodeType == En_NodeType.Short) {
                    NodeManager.Inst.CreateShortNode(startTime, startPercentage);
                } else {
                    NodeManager.Inst.CreateLongNode(startTime, startPercentage, endTime);
                }
            }
        }
    }
}
