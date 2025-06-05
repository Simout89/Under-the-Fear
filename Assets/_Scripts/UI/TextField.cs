using System.Collections;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class TextField: MonoBehaviour
    {
        [SerializeField] private TMP_Text textTmp;
        private Coroutine _lastCoroutine;
        
        public void ShowText(string text)
        {
            if(_lastCoroutine != null)
            {
                StopCoroutine(_lastCoroutine);
                textTmp.text = "";
            }
            _lastCoroutine = StartCoroutine(ShowTextCoroutine(text));
        }
        
        private IEnumerator ShowTextCoroutine(string text)
        {
            var textChar = text.ToCharArray();
            textTmp.text = "";
        
            for (int i = 0; i < textChar.Length; i++)
            {
                textTmp.text = textTmp.text + textChar[i];
                yield return new WaitForSeconds(0.08f);
            }

            yield return new WaitForSeconds(1f);

            float fadeDuration = 1f;
            float timeElapsed = 0f;
            Color startColor = textTmp.color;
            while (timeElapsed < fadeDuration)
            {
                timeElapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
                textTmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        
            textTmp.text = "";
            textTmp.color = startColor;
        }
    }
}