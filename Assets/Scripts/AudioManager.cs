using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Inst = null;
    public AudioSource audioSource;

    private bool draging = false;

    private bool delMode = false;
    private float startDelPercentage = -1;
    private float currDelPercentage = -1;


    private void Awake() {
        Inst = this;
    }

    public void AudioReady(AudioClip _clip) {
        audioSource.clip = _clip;
        audioSource.timeSamples = 0;
        audioSource.Stop();
        audioSource.time = 0;
        UpdateAudioInfo(0);
        CloseDelMode();

        // 显示信息
        Texture2D tex = AudioUtil.BakeAudioWaveform(_clip);
        UIController.Inst.ShowAudioWave(tex);
        UIController.Inst.ShowAudioClipInfo(_clip);
    }

    public void NoteReady() {

    }

    public float GetTimeWithPercentage(float _percentage) {
        return audioSource.clip.length * _percentage;
    }

    public bool IsPlaying() {
        return audioSource.isPlaying;
    }

    public float GetPlayedTime() {
        return audioSource.time;
    }

    public float GetPlayedPercentage(float _time) {
        return _time / audioSource.clip.length;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(2) && UIController.Inst.IsClickOnWaveformArea() > 0)) {
            PlayOrPause();

            GlobalState.viewMode = false;
        }


        if (Input.GetMouseButtonDown(1)) {
            float percentage = UIController.Inst.IsClickOnWaveformArea();
            if (percentage >= 0) {
                audioSource.Pause();
                GlobalState.viewMode = false;
                float playedTime = audioSource.clip.length * percentage;
                audioSource.time = playedTime;
                UpdateAudioInfo(playedTime);
            }
        }

        if (audioSource.isPlaying) {
            UpdateAudioInfo(audioSource.time);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            GlobalState.viewMode = true;
            PlayOrPause();
            if (audioSource.isPlaying) {
                DanceShowController.Inst.Init(audioSource.clip.length, NodeManager.Inst.nodeList);
            }
        }


        #region 滑动条操作
        if (audioSource.isPlaying) {
            if (Input.GetMouseButtonDown(0)) {
                float percentage = UIController.Inst.IsClickOnWaveformArea();
                if (percentage >= 0) {
                    draging = true;
                    float playedTime = audioSource.clip.length * percentage;
                    audioSource.time = playedTime;
                    UpdateAudioInfo(playedTime);
                }
            }

            if (Input.GetMouseButton(0)) {
                if (draging) {
                    float percentage = UIController.Inst.IsClickOnWaveformArea();
                    if (percentage >= 0) {
                        float playedTime = audioSource.clip.length * percentage;
                        UpdateAudioInfo(playedTime);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                draging = false;
                float percentage = UIController.Inst.IsClickOnWaveformArea();
                if (percentage >= 0) {
                    float playedTime = audioSource.clip.length * percentage;
                    audioSource.time = playedTime;
                    UpdateAudioInfo(playedTime);
                }
            }
        }
        #endregion


        #region 删除选择操作
        if (!audioSource.isPlaying) {
            if (Input.GetMouseButtonDown(0)) {
                float percentage = UIController.Inst.IsClickOnWaveformArea();
                if(percentage >= 0) {
                    delMode = true;
                    startDelPercentage = percentage;
                    UIController.Inst.UpdateDelArea(startDelPercentage, startDelPercentage);
                    Debug.Log("Enter Del Mode");
                }
            }

            if (Input.GetMouseButton(0)) {
                if (delMode) {
                    float percentage = UIController.Inst.IsClickOnWaveformArea();
                    if(percentage >= 0) {
                        currDelPercentage = percentage;
                        // Start Draw Del Area
                        UIController.Inst.UpdateDelArea(startDelPercentage, currDelPercentage);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            float percentage = UIController.Inst.IsClickOnWaveformArea();
            if (percentage < 0) {
                CloseDelMode();
                startDelPercentage = -1;
                currDelPercentage = -1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete)) {
            if (delMode) {
                Debug.LogError("Delete Node");
                DeleteNode();
            }
        }
        #endregion
    }

    private void PlayOrPause() {
        if (audioSource.isPlaying) {
            audioSource.Pause();
        } else {
            audioSource.Play();
            CloseDelMode();
        }
    }

    private void CloseDelMode() {
        if (delMode) {
            delMode = false;
            UIController.Inst.ClearDelArea();
            Debug.Log("Del Mode End");
        }
    }

    private void UpdateAudioInfo(float _playedTime) {
        float percentage = _playedTime / audioSource.clip.length;
        percentage = Mathf.Clamp(percentage, 0.0f, 1.0f);
        UIController.Inst.UpdateAudioPointer(percentage);

        UIController.Inst.UpdatePlayedTime(_playedTime);
    }

    private void DeleteNode() {
        if(startDelPercentage > 0 && currDelPercentage > 0) {
            float min = Mathf.Min(startDelPercentage, currDelPercentage);
            float max = Mathf.Max(startDelPercentage, currDelPercentage);
            float startTime = audioSource.clip.length * min;
            float endTime = audioSource.clip.length * max;
            NodeManager.Inst.DeleteNode(startTime, endTime);
        }
    }
}
