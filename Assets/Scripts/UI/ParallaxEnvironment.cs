using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EventManager = Managers.EventManager;

namespace UI
{
    public class ParallaxEnvironment : MonoBehaviour
    {
        [SerializeField] private Layer[] Layers;

        private int m_level;

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

        public bool NextLevel()
        {
            if (m_level == Layers[0].LevelMaterials.Length - 1)
            {
                // Max level already
                return false;
            }

            m_level++;
            Layers.ToList().ForEach(layer => layer.SetLevel(m_level));
            return true;
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
                TranslateAndWrap(layer.FirstImage, translation, out bool wrapped);

                if (layer.SecondImage != null)
                {
                    TranslateAndWrap(layer.SecondImage, translation, out wrapped);

                    if (wrapped)
                    {
                        EventManager.Instance.SpawnEvents();
                    }
                }
            }
        }

        private void TranslateAndWrap(Image image, Vector3 translation, out bool wrapped)
        {
            wrapped = false;
            image.transform.Translate(translation, Space.Self);

            float screenWidth = image.rectTransform.rect.width;
            if (image.rectTransform.localPosition.x > screenWidth)
            {
                image.transform.localPosition = new Vector3(-screenWidth, 0.0f, 0.0f);
                wrapped = true;
            }
        }
    }
}