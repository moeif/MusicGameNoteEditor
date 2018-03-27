using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtil {

    public static Texture2D BakeAudioWaveform(AudioClip _clip) {
        int resolution = 60;
        int width = 1920;
        int height = 200;

        resolution = _clip.frequency / resolution;

        float[] samples = new float[_clip.samples * _clip.channels];
        _clip.GetData(samples, 0);

        float[] waveForm = new float[(samples.Length / resolution)];

        float min = 0;
        float max = 0;
        bool inited = false;


        for (int i = 0; i < waveForm.Length; i++) {
            waveForm[i] = 0;

            for (int j = 0; j < resolution; j++) {
                waveForm[i] += Mathf.Abs(samples[(i * resolution) + j]);
            }

            if (!inited) {
                min = waveForm[i];
                max = waveForm[i];
                inited = true;
            } else {
                if (waveForm[i] < min) {
                    min = waveForm[i];
                }

                if (waveForm[i] > max) {
                    max = waveForm[i];
                }
            }
            //waveForm[i] /= resolution;
        }


        Color backgroundColor = Color.black;
        Color waveformColor = Color.green;
        Color[] blank = new Color[width * height];
        Texture2D texture = new Texture2D(width, height);

        for (int i = 0; i < blank.Length; ++i) {
            blank[i] = backgroundColor;
        }

        texture.SetPixels(blank, 0);

        float xScale = (float)width / (float)waveForm.Length;

        int tMid = (int)(height / 2.0f);
        float yScale = 1;

        if (max > tMid) {
            yScale = tMid / max;
        }

        for (int i = 0; i < waveForm.Length; ++i) {
            int x = (int)(i * xScale);
            int yOffset = (int)(waveForm[i] * yScale);
            int startY = tMid - yOffset;
            int endY = tMid + yOffset;

            for (int y = startY; y <= endY; ++y) {
                texture.SetPixel(x, y, waveformColor);
            }

        }

        texture.Apply();

        return texture;
    }
}
