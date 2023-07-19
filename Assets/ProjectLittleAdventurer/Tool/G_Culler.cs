using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class G_Culler : MonoBehaviour
{
    MeshRenderer _renderer;

    public Vector3 Center;
    ParticleSystem _particleSystem;

    VisualEffect _visualEffect;

    public enum CullerType
    {
        MeshRenderer, ParticleSystem
    }

    public CullerType Type;


    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _particleSystem = GetComponent<ParticleSystem>();
        _visualEffect = GetComponent<VisualEffect>();

        if (_renderer != null)
        {
            Center = _renderer.bounds.center;
            Type = CullerType.MeshRenderer;
        }

        if (_particleSystem != null || _visualEffect != null)
        {
            Center = transform.position;
            Type = CullerType.ParticleSystem;
        }

    }

    public void Cull(bool isVisiable)
    {
        if (_renderer != null)
        {
            _renderer.enabled = isVisiable;
        }

        if (_particleSystem != null)
        {
            if (isVisiable)
                _particleSystem.Play();
            else
                _particleSystem.Stop();
        }

        if (_visualEffect != null)
        {
            if (isVisiable)
                _visualEffect.Play();
            else
                _visualEffect.Stop();
        }
    }

}
