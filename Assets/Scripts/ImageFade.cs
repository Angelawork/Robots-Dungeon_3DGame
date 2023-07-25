using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    public float fadeInDuration = 3.0f;
    public float displayDuration = 2.0f;
    public float fadeOutDuration = 1.0f;

    private Image imageComponent;

    private void Awake() {
        imageComponent = GetComponent<Image>();
    }
    private void Start()
    {
        StartCoroutine(AnimateImage());
    }

    private IEnumerator AnimateImage()
    {
        Color originalColor = imageComponent.color;

        // Fade in
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Make sure the image is fully transparent
        imageComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
