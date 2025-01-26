using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class robotFaces : MonoBehaviour
{
    [SerializeField] private Sprite normalFace, happyFace;

    private Image faceImage;

    private void Awake()
    {
        this.faceImage = GetComponent<Image>();
    }

    public void OnExplosion()
    {
        this.faceImage.sprite = this.happyFace;
        Invoke(nameof(this.ResetFace), 10);
    }

    private void ResetFace()
    {
        this.faceImage.sprite = this.normalFace;
    }
}
