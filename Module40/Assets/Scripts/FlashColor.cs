using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlashColor : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    //public Material material;

    [Header("Setup")]
    public Color color = Color.red;
    public float duration = .2f;

    public string colorParameter = "_EmissionColor";

    private Color _defaultColor;

    private Tween _currentTween;

    private Material _targetMaterial;

    public void OnValidate()
    {
        if(meshRenderer == null && !gameObject.transform.CompareTag("Player"))
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if(skinnedMeshRenderer == null && !gameObject.transform.CompareTag("Enemy"))
        {
            Debug.Log("Passou em OnValidate() com skinnedMeshRenderer == null");
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
    }

    public void Start()
    {
        // Determina qual material usar
        if (meshRenderer != null)
        {
            _targetMaterial = meshRenderer.material;
        }
        else if (skinnedMeshRenderer != null)
        {
            _targetMaterial = skinnedMeshRenderer.material;
        }
        else
        {
            Debug.LogError("No renderer found on " + gameObject.name);
            return;
        }

        // Habilita emission e salva a cor padr�o
        _targetMaterial.EnableKeyword("_EMISSION");
        _defaultColor = _targetMaterial.GetColor(colorParameter);
    }

    //private void Update()
    //{
    //    if (meshRenderer != null && meshRenderer.material.GetColor("_EmissionColor") != _defaultColor)
    //    {
    //        new WaitForSeconds(1f);
    //        meshRenderer.material.SetColor("_EmissionColor", _defaultColor);
    //    }
    //    else if (skinnedMeshRenderer != null && skinnedMeshRenderer.material.GetColor("_EmissionColor") != _defaultColor)
    //    {
    //        skinnedMeshRenderer.material.SetColor("_EmissionColor", _defaultColor);
    //    }
    //}

    [NaughtyAttributes.Button]
    public void Flash()
    {
        if (_targetMaterial == null) 
        {
            Debug.Log("_targetMaterial = null. Nome: "+gameObject.name);
            return;
        }
        else
        {
            Debug.Log("_targetMaterial = "+_targetMaterial);
            Debug.Log("_targetMaterial.name = " + _targetMaterial.name);
        }

        // Se j� existe um tween ativo, mata ele antes de criar um novo
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        // Cria a sequ�ncia de flash
        _currentTween = _targetMaterial
            .DOColor(color, colorParameter, duration)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => {
                // Garante que volta para a cor padr�o ao finalizar
                Debug.Log("Passou por mudar de cor");
                new WaitForSeconds(.5f);
                _targetMaterial.SetColor(colorParameter, _defaultColor);
            });
    }

    //// M�todo alternativo usando corrotina para mais controle
    //public void FlashWithCoroutine()
    //{
    //    StartCoroutine(FlashRoutine());
    //}

    //private IEnumerator FlashRoutine()
    //{
    //    if (_targetMaterial == null) yield break;

    //    // Primeira fase: mudar para a cor de flash
    //    _targetMaterial.SetColor("_EmissionColor", color);
    //    yield return new WaitForSeconds(duration);

    //    // Segunda fase: voltar para a cor padr�o
    //    _targetMaterial.SetColor("_EmissionColor", _defaultColor);
    //}

    //// Limpa o tween quando o objeto � destru�do
    //private void OnDestroy()
    //{
    //    if (_currentTween != null && _currentTween.IsActive())
    //    {
    //        _currentTween.Kill();
    //    }
    //}
}
