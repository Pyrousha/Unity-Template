using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (parentTransform == null)
            parentTransform = transform.parent;
    }
#endif

    void Start()
    {
        transform.parent = null;
    }

    void FixedUpdate()
    {
        #region Set position and scale of "shadow" object
        float newScale = 0;
        if (Physics.Raycast(parentTransform.position, -parentTransform.up, out RaycastHit _hit, 50, groundLayer))
        {
            transform.position = _hit.point;
            float distToGround = _hit.distance;
            transform.up = _hit.normal;

            //Shadow should be smaller the further away the character is from the ground
            newScale = Mathf.Max(maxScale - 0.1f * distToGround, minScale);
        }
        transform.localScale = new Vector3(newScale, transform.localScale.y, newScale);
        #endregion
    }
}
