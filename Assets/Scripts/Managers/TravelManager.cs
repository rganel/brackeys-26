using System.Collections;
using Scriptable_Objects;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    [DefaultExecutionOrder(-1000)]
    public class TravelManager : MonoBehaviour
    {
        [SerializeField] private ActivitySO TravelActivity;
        [SerializeField] private float TravelTickInterval;

        [SerializeField] private int TravelDaysPerMap;

        [SerializeField] private float BaseMovementSpeed;
        [SerializeField] private ParallaxEnvironment ParallaxEnvironment;

        [SerializeField] private AnimationCurve DayCycleCurve;
        [SerializeField] private AnimationCurve LightIntensityCurve;
        [SerializeField] private Transform BeginDayLightTransform;
        [SerializeField] private Transform MiddayLightTransform;
        [SerializeField] private Transform EndDayLightTransform;
        [SerializeField] private Light DirectionalLight;
        [SerializeField] private Material[] EnvironmentMaterials;
        [SerializeField] private Material ShadowCatcherMaterial;

        public static TravelManager Instance { get; private set; }

        public UnityEvent NextLevelEvent;
        public UnityEvent ReachedTowerEvent;

        private float m_timeOfDay;
        private float m_timeOfDaySign;
        private float m_thisLevelTravelDaysRemaining;
        private int m_interpolateParamId;
        private int m_shadowFactorParamId;
        private Coroutine m_tickCoroutine;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Debug.Assert(TravelActivity != null);
            Debug.Assert(TravelTickInterval > 0.0f);
            Debug.Assert(BaseMovementSpeed > 0.0f);
            Debug.Assert(ParallaxEnvironment != null);
            Debug.Assert(DayCycleCurve != null);
            Debug.Assert(LightIntensityCurve != null);
            Debug.Assert(BeginDayLightTransform != null);
            Debug.Assert(MiddayLightTransform != null);
            Debug.Assert(EndDayLightTransform != null);
            Debug.Assert(DirectionalLight != null);
            Debug.Assert(EnvironmentMaterials != null);
            Debug.Assert(ShadowCatcherMaterial != null);

            m_interpolateParamId = Shader.PropertyToID("_Interpolation");
            Debug.Assert(m_interpolateParamId != 0);

            m_shadowFactorParamId = Shader.PropertyToID("_ShadowAlphaFactor");
            Debug.Assert(m_shadowFactorParamId != 0);

            m_thisLevelTravelDaysRemaining = TravelDaysPerMap;
        }

        private void OnDestroy()
        {
            foreach (Material material in EnvironmentMaterials)
            {
                material.SetFloat(m_interpolateParamId, 0.0f);
            }

            ShadowCatcherMaterial.SetFloat(m_shadowFactorParamId, 0.0f);
        }

        private void Update()
        {
            if (m_tickCoroutine != null)
            {
                m_timeOfDay = Mathf.Clamp(m_timeOfDay + (2.0f / TravelTickInterval) * Time.deltaTime * m_timeOfDaySign, 0.0f, 1.0f);
                float scaledTimeOfDay = DayCycleCurve.Evaluate(m_timeOfDay);

                if (m_timeOfDaySign > 0.0f)
                {
                    DirectionalLight.transform.rotation = Quaternion.Slerp(BeginDayLightTransform.rotation, MiddayLightTransform.rotation, scaledTimeOfDay);
                }
                else
                {
                    DirectionalLight.transform.rotation = Quaternion.Slerp(EndDayLightTransform.rotation, MiddayLightTransform.rotation, scaledTimeOfDay);
                }

                DirectionalLight.colorTemperature = Mathf.Lerp(1500, 5500, scaledTimeOfDay);
                DirectionalLight.intensity = LightIntensityCurve.Evaluate(scaledTimeOfDay);

                foreach (Material material in EnvironmentMaterials)
                {
                    material.SetFloat(m_interpolateParamId, scaledTimeOfDay);
                }

                ShadowCatcherMaterial.SetFloat(m_shadowFactorParamId, scaledTimeOfDay);

                ParallaxEnvironment.ApplyMovement(BaseMovementSpeed * Time.deltaTime);

                TryNextLevel();
            }
        }

        private void TryNextLevel()
        {
            if (m_thisLevelTravelDaysRemaining > 0)
            {
                return;
            }

            if (!ParallaxEnvironment.NextLevel())
            {
                ReachedTowerEvent?.Invoke();
                return;
            }
            
            m_thisLevelTravelDaysRemaining = TravelDaysPerMap;
            NextLevelEvent?.Invoke();
        }

        public void BeginTravel()
        {
            if (m_tickCoroutine != null)
            {
                StopCoroutine(m_tickCoroutine);
            }

            m_tickCoroutine = StartCoroutine(TickHandler());
        }

        public void PauseTravel()
        {
            if (m_tickCoroutine != null)
            {
                StopCoroutine(m_tickCoroutine);
                m_tickCoroutine = null;
            }
        }

        private IEnumerator TickHandler()
        {
            while (true)
            {
                m_timeOfDaySign = 1.0f;
                yield return new WaitForSeconds(TravelTickInterval / 2);

                m_timeOfDaySign = -1.0f;
                yield return new WaitForSeconds(TravelTickInterval / 2);

                ActivityManager.Instance.ApplyResourceChanges(TravelActivity);
                m_thisLevelTravelDaysRemaining -= ActivityManager.Instance.GetChangeAmount(TravelActivity, EResourceType.Day, out bool isRequiredCost);
            }
        }
    }
}