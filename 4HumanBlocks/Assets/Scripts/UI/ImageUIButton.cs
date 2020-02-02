using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    public Sprite Idle;
    public Sprite Hover;
    public Sprite Click;

    private float xRatio;
    private float yRatio;
    private Vector2 originalScale;

    public AudioClip beep;
    private AudioSource audioSource;

    void Start () {
        originalScale = transform.localScale;
        xRatio = transform.localScale[0] / Idle.rect.size[0];
        yRatio = transform.localScale[1] / Idle.rect.size[1];

        // try {
        //     audioSource = gameObject.GetComponent<AudioSource> ();
        // } catch {

        // }
        audioSource = gameObject.AddComponent<AudioSource> ();
        if (beep != null) {
            audioSource.clip = beep;
            audioSource.volume = 0.3f;
        }
    }

    Vector3 getScale (Sprite target) {
        // new Scale
        float xSize = xRatio * target.rect.size[0];
        float ySize = yRatio * target.rect.size[1];
        // Normalize new scale
        xSize = xSize / (xSize / originalScale[0]);
        ySize = ySize / (xSize / originalScale[0]);
        return new Vector3 (xSize, ySize, 1.0f);
    }

    public void OnPointerEnter (PointerEventData eventData) {
        if (Hover != null) {
            GetComponent<Image> ().sprite = Hover;
            transform.localScale = getScale (Hover);
        }
    }

    public void OnPointerExit (PointerEventData eventData) {
        GetComponent<Image> ().sprite = Idle;
        transform.localScale = getScale (Idle);
    }

    public void OnPointerDown (PointerEventData eventData) {
        if (Hover != null) {
            GetComponent<Image> ().sprite = Click;
            transform.localScale = getScale (Click);
            if (beep != null) {
                audioSource.Stop ();
                audioSource.Play ();
            }
        }
    }

    public void OnPointerUp (PointerEventData eventData) {
        if (Hover != null) {
            GetComponent<Image> ().sprite = Hover;
            transform.localScale = getScale (Hover);
        } else {
            GetComponent<Image> ().sprite = Idle;
            transform.localScale = getScale (Idle);
        }
    }
}