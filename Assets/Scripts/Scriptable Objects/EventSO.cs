using UnityEngine;
using Utils;

namespace Scriptable_Objects
{
    [SmartConfig("Assets/Backups/Event")]
    [CreateAssetMenu(fileName = "EventSO", menuName = "Scriptable Objects/EventSO")]
    public class EventSO : ScriptableObject
    {
        public string Description;
        public Sprite DefaultSprite;
        public Sprite HoverSprite;
        public string TrustDescription;
        public string RejectDescription;
        public ActivitySO TrustActivity;
        public ActivitySO RejectActivity;
    }
}
