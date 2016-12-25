using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter : MonoBehaviour {

    SphereCollider col;
    Texture2D paintTexture;
    void Start()
    {
        col = GetComponent<SphereCollider>();
    }
    void SetTexturePaint(Vector2 uvCoord, Transform _target)
    {
        Material mat = _target.GetComponent<MeshRenderer>().material;

    }
    void OnCollisionStay(Collision col)
    {
        int triangle;

        if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            foreach (ContactPoint P in col.contacts)
            {
                RaycastHit hit;
                Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
                if (Physics.Raycast(ray, out hit, 0.1f, LayerMask.GetMask("ground")))
                {
                    triangle = hit.triangleIndex;
                    SetTexturePaint(hit.textureCoord, hit.transform);
                    break;
                }
            }
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("ground")))
            {
                SetTexturePaint(hit.textureCoord, hit.transform);
            }
        }
    }
}
