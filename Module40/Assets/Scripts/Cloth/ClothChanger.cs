using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cloth;

namespace Cloth
{
    public class ClothChanger : MonoBehaviour
    {
        public SkinnedMeshRenderer mesh;

        public Texture2D texture;

        public string shaderIdName = "_EmissionMap";

        private Texture2D _defaultTexture;
        private Texture2D _actualTexture;


        public void Awake()
        {
            _defaultTexture = (Texture2D)mesh.sharedMaterials[0].GetTexture(shaderIdName);
            _actualTexture = (Texture2D)mesh.sharedMaterials[0].GetTexture(shaderIdName);

            if (_defaultTexture == null)
            {
                Debug.Log("_defaultTexture = null");
            }
            else
            {
                Debug.Log("_defaultTexture não é null");
                Debug.Log("_defaultTexture.name = "+ _defaultTexture.name);
                Debug.Log("_actualTexture.name = " + _actualTexture.name);
            }
        }

        private void Update()
        {
            _actualTexture = (Texture2D)mesh.sharedMaterials[0].GetTexture(shaderIdName);
            Debug.Log("(UpdateTexture) _actualTexture.name = " + _actualTexture.name);

            if (_defaultTexture == null)
            {
                Debug.Log("_defaultTexture = null");
            }
            else
            {
                Debug.Log("_defaultTexture não é null");
                Debug.Log("_defaultTexture.name = " + _defaultTexture.name);
            }
        }

        [NaughtyAttributes.Button]
        private void ChangeTexture()
        {
            mesh.sharedMaterials[0].SetTexture(shaderIdName, texture);
            //mesh.sharedMaterials[0].SetTexture(shaderIdName, texture);
        }

        public void ChangeTexture(ClothSetup setup)
        {
            if (mesh == null || mesh.sharedMaterials.Length == 0) return;

            //_defaultTexture = (Texture2D)mesh.sharedMaterials[0].GetTexture(shaderIdName);
            
            Debug.Log("(ChangeTexture) _defaultTexture.name = " + _defaultTexture);

            Debug.Log("(ChangeTexture) Entrou em ChangeTexture");

            if (setup != null && setup.texture != null)
            {
                Debug.Log("(ChangeTexture) setup.texture = " + setup.texture.name);
                mesh.sharedMaterials[0].SetTexture(shaderIdName, setup.texture);
            }

            Debug.Log($"Mesh: {mesh != null}, Materials: {mesh?.sharedMaterials?.Length}");
            Debug.Log($"Shader Property exists: {mesh?.sharedMaterials[0]?.HasProperty(shaderIdName)}");
        }

        public void ResetTexture()
        {
            Debug.Log("(ResetTexture) Entrou em ResetTexture");

            Debug.Log("(ResetTexture) _defaultTexture.name = " + _defaultTexture);

            if (mesh == null || mesh.sharedMaterials.Length == 0 || _defaultTexture == null)
            {
                return;
            }

            mesh.sharedMaterials[0].SetTexture(shaderIdName, _defaultTexture);
        }
    }
}