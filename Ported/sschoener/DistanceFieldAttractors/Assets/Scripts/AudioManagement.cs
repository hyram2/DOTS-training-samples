using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class AudioManagement : MonoBehaviour
{
    public float[] data = new float[1024];
    public float modifier;

    public static AudioManagement instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(data, 0, FFTWindow.Blackman);
        
    }
}
