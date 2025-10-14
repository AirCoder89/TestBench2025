using TMPro;
using UnityEngine;

namespace TestBench2025.Core.UI
{
    public class ComboEffectUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        
        public void Initialize()
        {
            comboText.gameObject.SetActive(false);
        }

        public void PlayComboFeedback(int multiplier)
        {
            if (multiplier <= 1)
            {
                comboText.gameObject.SetActive(false);
                return;
            }

            comboText.text = $"x{multiplier}";
            comboText.transform.localScale = Vector3.one * 1.5f;
            comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f);
            comboText.gameObject.SetActive(true);
        }

        
        private void Update()
        {
            if (comboText.gameObject.activeSelf)
            {
                comboText.transform.localScale = Vector3.Lerp(comboText.transform.localScale, Vector3.one, Time.deltaTime * 5f);
                var color = comboText.color;
                color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 2f);
                comboText.color = color;

                if (color.a <= 0.01f)
                {
                    comboText.gameObject.SetActive(false);
                    color.a = 1f;
                    comboText.color = color;
                }
            }
        }
    }
}