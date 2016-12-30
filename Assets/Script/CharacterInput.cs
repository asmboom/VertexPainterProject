using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterMovement))]
public class CharacterInput : MonoBehaviour {

    protected CharacterMovement movement;
    void Start () {
        movement = GetComponent<CharacterMovement>();
	}
	void KeyInput()
    {
        Vector3 axis=new Vector3();
        //axis.x = CrossPlatformInputManager.GetAxis("Horizontal");
        //axis.z = CrossPlatformInputManager.GetAxis("Vertical");
        axis.x = Input.GetAxis("Horizontal");
        axis.y = Input.GetAxis("Vertical");
        if (axis != Vector3.zero)
        {
            axis = Camera.main.transform.TransformVector(axis);
            axis.y = 0;
            axis.Normalize();
            //movement.RotateTo(Quaternion.LookRotation(axis), 40);
            movement.Move(axis, 3, 0.1f);
        }
        else
        {
            if (movement.IsMoving())
            {
                movement.Stop();
            }
        }
    }
	void Update () {
        KeyInput();

    }
}
