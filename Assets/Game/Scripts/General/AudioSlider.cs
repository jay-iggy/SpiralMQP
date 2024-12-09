using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{


    [SerializeField] AudioMixer mixer;
    public void SetVolume(float n)
    {
        if (n == 0) mixer.SetFloat("Volume", -80);
        else mixer.SetFloat("Volume", Mathf.Log10(n) * 20);
    }
    void Awake()
    {
        GetComponent<Slider>().onValueChanged.AddListener(SetVolume);
    }
}
