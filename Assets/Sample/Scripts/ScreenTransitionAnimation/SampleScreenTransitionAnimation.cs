using UnityEngine;
using UnityEngine.UI;

namespace Sabanishi.ScreenSystemSample
{
    public class SampleScreenTransitionAnimation:MonoBehaviour
    {
        [SerializeField] private Image image;
        private static SampleScreenTransitionAnimation _instance;
        public static SampleScreenTransitionAnimation Instance => _instance;
        
        public SampleOpenAnimation OpenAnimation { get; private set; }
        public SampleCloseAnimation CloseAnimation { get; private set; }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                OpenAnimation = new SampleOpenAnimation(image);
                CloseAnimation = new SampleCloseAnimation(image);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}