using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotation : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float CamPlaceDistance = 5f;

    float RotationX;
    float RotationY;
    
    float minRotDegree = -30f;
    float maxRotDegree = 12f;
    [SerializeField] Vector2 CamOffset;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void Update()
    {
               
        RotationX += Input.GetAxis("Camera Y") ;
        RotationY += Input.GetAxis("Camera X") ;

        RotationX = Mathf.Clamp(RotationX, minRotDegree, maxRotDegree);

        var PlayerRotation = Quaternion.Euler(RotationX, RotationY, 0) ;
               
       var OffsetPosition = Player.position + new Vector3(CamOffset.x, CamOffset.y);
        transform.position = OffsetPosition - PlayerRotation * new Vector3(0,0,CamPlaceDistance);
        transform.rotation = PlayerRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, RotationY, 0) ;
}
