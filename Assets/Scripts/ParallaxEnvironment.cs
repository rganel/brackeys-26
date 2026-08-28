using System;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxEnvironment : MonoBehaviour
{
    [SerializeField] private Layer[] Layers;

    [Serializable]
    private class Layer
    {
        public Image FirstImage;
        public Image SecondImage;
        public float RelativeScrollSpeed;
    }

    private void Awake()
    {
        Debug.Assert(Layers != null);
    }

    public void ApplyMovement(float baseMovement)
    {
        foreach (Layer layer in Layers)
        {
            Vector3 translation = Vector3.right * (baseMovement * layer.RelativeScrollSpeed);
            TranslateAndWrap(layer.FirstImage, translation);
            TranslateAndWrap(layer.SecondImage, translation);
        }
    }

    private void TranslateAndWrap(Image image, Vector3 translation)
    {
        image.transform.Translate(translation, Space.Self);

        float screenWidth = image.rectTransform.rect.width;
        if (image.rectTransform.localPosition.x > screenWidth)
        {
            image.transform.localPosition = new Vector3(-screenWidth, 0.0f, 0.0f);
        }
    }
}
