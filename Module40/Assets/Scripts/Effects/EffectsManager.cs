using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using Core.Singleton;

public class EffectsManager : Singleton<EffectsManager>
{
    public PostProcessVolume processVolume;
    [SerializeField] private Vignette _vignette;

    public float duration = 1f;

    [NaughtyAttributes.Button]
    public void ChangeVignette()
    {
        StartCoroutine(FlashColorVignette());
    }

    IEnumerator FlashColorVignette()
    {
        Vignette tmp;

        if (processVolume.profile.TryGetSettings<Vignette>(out tmp))
        {
            _vignette = tmp;
        }

        ColorParameter c = new ColorParameter();

        float time = 0;
        while(time < duration)
        {
            c.value = Color.Lerp(Color.black, Color.red, time/duration);

            time += Time.deltaTime;

            _vignette.color.Override(c);

            if (_vignette.intensity.value < .55f)
            {
                _vignette.intensity.value += 0.02f;
            } 

            yield return new WaitForEndOfFrame();
        }

        time = 0;
        while (time < duration)
        {
            c.value = Color.Lerp(Color.red, Color.black, time / duration);

            time += Time.deltaTime;

            _vignette.color.Override(c);

            if (_vignette.intensity.value > .46f)
            {
                _vignette.intensity.value -= 0.02f;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
