using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public abstract class UIBase<T> : MonoBehaviour, IInitializePotentialDragHandler where T : class
    {
        [SerializeField] private float openCloseTime = 0.2f;
        [SerializeField] private bool closeOnLostFocus = false;
        [SerializeField] private bool hasBackDrop = false;
        [SerializeField] private bool isStatic = false;
        [SerializeField] private bool customButtonSetup = false;

        private bool _isOpening = false;
        private bool _isClosing = false;

        private Transform _panel;
        private Image _backDrop;
        
        private float _timer = 0;

        private static AnimationCurve ScaleCurve 
            => UiManager.Instance.ToggleUiScaleCurve;
        private static AnimationCurve AlphaCurve 
            => UiManager.Instance.ToggleUiAlphaCurve;

        protected virtual void Update()
        {
            if(_isOpening) OpenTick();
            if(_isClosing) CloseTick();

            if (!(Input.GetMouseButtonDown(0) && closeOnLostFocus)) return;
            
            Vector2 localMousePosition = _panel.InverseTransformPoint(Input.mousePosition);
            if (_panel.GetComponent<RectTransform>().rect.Contains(localMousePosition)) return;
                
            SetCloseUI();
        }

        private void OpenTick()
        {
            _timer += Time.deltaTime;
            var evaluation = _timer / openCloseTime;
            if (evaluation >= 1)
            {
                _isOpening = false;
                OnUIOpened();
                return;
            }
            
            _panel.localScale = Vector3.one * ScaleCurve.Evaluate(evaluation);
            if (!hasBackDrop) return;

            var color = _backDrop.color;
            color.a = AlphaCurve.Evaluate(evaluation);
            _backDrop.color = color;
        }

        private void CloseTick()
        {
            _timer += Time.deltaTime;
            var evaluation = 1 - _timer / openCloseTime;
            if (evaluation <= 0)
            {
                _isClosing = false;
                OnUIClosed();
                Destroy(gameObject);
                return;
            }
            
            _panel.localScale = Vector3.one * ScaleCurve.Evaluate(evaluation);
            if (!hasBackDrop) return;

            var color = _backDrop.color;
            color.a = AlphaCurve.Evaluate(evaluation);
            _backDrop.color = color;
        }

        public virtual void SetOpenUI(T props = null)
        {
            _panel = hasBackDrop ? transform.GetChild(0) : transform;
            _panel.localScale = Vector3.zero;
            _isOpening = true;
            _isClosing = false;
            _timer = 0;
            gameObject.SetActive(true);
            if (!hasBackDrop) return;
            
            _backDrop = GetComponent<Image>();
            _backDrop.color = new Color()
            {
                r = 0,
                g = 0,
                b = 0,
                a = AlphaCurve.Evaluate(0),
            };
        }

        public virtual void SetCloseUI()
        {
            _panel = hasBackDrop ? transform.GetChild(0) : transform;
            _panel.localScale = Vector3.zero;
            _timer = 0;
            _isClosing = true;
            _isOpening = false;
            if (!hasBackDrop) return;
            
            _backDrop = GetComponent<Image>();
            _backDrop.color = new Color()
            {
                r = 0,
                g = 0,
                b = 0,
                a = AlphaCurve.Evaluate(1),
            };
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnUIDrag(BaseEventData data)
        {
            if (isStatic) return;

            var pointerEventData = (PointerEventData)data;
            var mainWindow = GetComponent<RectTransform>();
            mainWindow.anchoredPosition += pointerEventData.delta / UiManager.Instance.MainCanvas.scaleFactor;
            mainWindow.transform.SetAsLastSibling();
        }

        protected virtual void OnUIOpened()
        {
            
        }

        protected virtual void OnUIClosed()
        {
            
        }
    }
}