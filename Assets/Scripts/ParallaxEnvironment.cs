using System;
using System.Linq;
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
        public Material[] LevelMaterials;

        public bool SetLevel(int level)
        {
            Debug.Assert(level >= 0);
            Debug.Assert(LevelMaterials.Length > level);

            FirstImage.material = LevelMaterials[level];

            if (SecondImage != null)
            {
                SecondImage.material = LevelMaterials[level];
            }

            return true;
        }
    }

    private void Awake()
    {
        Debug.Assert(Layers != null);
    }

    [ContextMenu("SetLevel1")]
    public void SetLevel1()
    {
        SetLevel(1);
    }
    
    [ContextMenu("SetLevel2")]
    public void SetLevel2()
    {
        SetLevel(2);
    }

    public void SetLevel(int level)
    {
        Layers.ToList().ForEach(layer => layer.SetLevel(level));
    }

    public void ApplyMovement(float baseMovement)
    {
        foreach (Layer layer in Layers)
        {
            if (layer.RelativeScrollSpeed == 0)
            {
                continue;
            }
            
            Vector3 translation = Vector3.right * (baseMovement * layer.RelativeScrollSpeed);
            TranslateAndWrap(layer.FirstImage, translation);

            if (layer.SecondImage != null)
            {
                TranslateAndWrap(layer.SecondImage, translation);
            }
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