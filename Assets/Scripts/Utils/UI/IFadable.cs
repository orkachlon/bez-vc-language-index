using System;
using System.Collections;
using UnityEngine;

namespace Utils.UI {
    public interface IFadable {

        float GetOpacity();
        void SetOpacity(float percent);


        public IEnumerator FadeOut(float fadeDuration, Action callback = null) {
            SetOpacity(1);
            var time = 0f;
            while (time < fadeDuration) {
                // lerp node translation
                var t = time / fadeDuration;
                t = t * t * (3f - 2f * t); // ease animation
                SetOpacity(Mathf.Lerp(1f, 0f, t));
                // increment
                time += Time.deltaTime;
                yield return null;
            }
            SetOpacity(0);
            callback?.Invoke();
        }

        public IEnumerator FadeIn(float fadeDuration, Action callback = null) {
            SetOpacity(0);
            var time = 0f;
            while (time < fadeDuration) {
                // lerp node translation
                var t = time / fadeDuration;
                t = t * t * (3f - 2f * t); // ease animation
                SetOpacity(Mathf.Lerp(0f, 1f, t));
                // increment
                time += Time.deltaTime;
                yield return null;
            }
            SetOpacity(1);
            callback?.Invoke();
        }
    }
}