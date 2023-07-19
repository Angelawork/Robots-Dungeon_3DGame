using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_CullManager : MonoBehaviour
{
    public static G_CullManager Instance;
    public List<G_Culler> _Cullers;

    private CullingGroup _cullingGroup;

    private BoundingSphere[] _boundingSphere;
    public float CullingRadius = 10f;
    public float ParticleCullingRadius = 5f;



    private void Start()
    {
        _Cullers = new List<G_Culler>();
        _Cullers = new List<G_Culler>(GetComponentsInChildren<G_Culler>());

        SetupCullingGroup();
    }

    private void SetupCullingGroup()
    {
        if (_Cullers == null)
            return;

        _cullingGroup = new CullingGroup();

        _cullingGroup.targetCamera = Camera.main;
       // Debug.Log(_cullingGroup.targetCamera);

        _boundingSphere = new BoundingSphere[_Cullers.Count];

        for (int i = 0; i < _Cullers.Count; i++)
        {

            _boundingSphere[i] = new BoundingSphere(_Cullers[i].Center, _Cullers[i].Type == G_Culler.CullerType.MeshRenderer ? CullingRadius : ParticleCullingRadius);

            _Cullers[i].Cull(false);
        }

        _cullingGroup.SetBoundingSpheres(_boundingSphere);
        _cullingGroup.SetBoundingSphereCount(_boundingSphere.Length);
        _cullingGroup.onStateChanged += StateChangedMethod;

    }


    private void StateChangedMethod(CullingGroupEvent evt)
    {
        _Cullers[evt.index].Cull(evt.isVisible);
    }


    public void AddCuller(G_Culler culler)
    {
        if (!_Cullers.Contains(culler))
            _Cullers.Add(culler);
    }

    private void OnDestroy()
    {
        _cullingGroup.Dispose();
    }

}
