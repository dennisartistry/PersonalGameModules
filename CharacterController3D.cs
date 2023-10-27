using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotationSpeed = 500f;

    //is on ground checking variables->
    [SerializeField] Vector3 GrndCheckOffset;
    [SerializeField] float GrndCheckRadius = 0.2f;
    [SerializeField] LayerMask GrndLayer;
    [SerializeField] float jumpSpeed;
    [SerializeField] float ySpeed;
    bool isJumping;
    bool isCrouching = false;
    bool isFalling;
    bool isGrounded;
    
    bool OnGround = false;
   
    Quaternion PlayerRotation;

    float Stamina;

    CamRotation CamControl;
    CharacterController characterController;
    Animator animator;
    [SerializeField] Slider StaminaSlide;
    [SerializeField] Gradient StaminaGradient;
    public Image fill;


    private void Start()
    {
       Stamina = StaminaSlide.maxValue;
       StaminaSlide.value = Stamina;
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        //Getting the camera component from the camRotation Script

        CamControl = Camera.main.GetComponent<CamRotation>();

        animator = GetComponent<Animator>();
    }
    void Update()
    {
        float horiInput = Input.GetAxis("Horizontal");
        float vertiInput = Input.GetAxis("Vertical");
        float MoveValue = Mathf.Abs(horiInput) + Mathf.Abs(vertiInput);
        float MoveIntensity = Mathf.Clamp01(MoveValue);

        Vector3 MoveDir = new Vector3();
        
        Vector3 PlayerPosition = new Vector3(horiInput, 0, vertiInput).normalized;
                      
        //getting the rotation of the camera and directing the player to move according to the camera rotation.
                      
        IsOnGround();
        
        MoveAnimate(MoveIntensity);

  
        MoveDir = CamControl.PlanarRotation * PlayerPosition;
       
      if (MoveIntensity > 0)
            {
               
                //Player Looks in the rotating direction
                PlayerRotation = Quaternion.LookRotation(MoveDir);
            }

        //rotate the player towards the movement rotation  
        transform.rotation = Quaternion.RotateTowards(transform.rotation, PlayerRotation, rotationSpeed * Time.deltaTime);
        Vector3 Velocity = MoveDir * moveSpeed;

        ySpeed += Physics.gravity.y * Time.deltaTime;
        ySpeed = Mathf.Clamp(ySpeed , -0.5f, float.MaxValue); 

        playerJump();
        Velocity.y = ySpeed;
        characterController.Move(Velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.JoystickButton2) && !isCrouching)
        {            
            crouchAction();
            isCrouching = true;          
        }
        else if(Input.GetKeyDown(KeyCode.JoystickButton2) && isCrouching)
        {
            stand();
            isCrouching = false;
        }


    }

    public void crouchAction()
    {
        animator.SetBool("Stand", false);
        animator.SetBool("Crouch", true);
        animator.SetFloat("CrouchMotion",0.9f,0.2f, Time.deltaTime);
       
    }

    public void stand()
    {       
        animator.SetBool("Stand" , true);
        animator.SetBool("Crouch", false);
        // animator.SetFloat("CrouchMotion", -1, 0.2f, Time.deltaTime);
       
    }
    public void playerJump()
    {

        if (OnGround) {
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false) ;
            isJumping = false;
            isFalling = false;

            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("IsJumping", true) ;
                isJumping = true;
                isGrounded = false;

                ySpeed = jumpSpeed;
                OnGround = false;              
            }

            if(isJumping)
            {
                animator.SetBool("IsFalling", true);
            }
           
        }
        else
        {
            animator.SetBool("IsGrounded", false );
            isGrounded = false;

                animator.SetBool("IsJumping", false ) ;
                animator.SetBool("IsFalling", true );
                isJumping = false;
                isFalling = true;
        }
          
     
    }
    void IsOnGround()
    {
       
        OnGround = Physics.CheckSphere(transform.TransformPoint(GrndCheckOffset), GrndCheckRadius, GrndLayer);
        
    }

       
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(GrndCheckOffset), GrndCheckRadius);
    }
    
    void MoveAnimate( float MoveIntensity)
    {
        bool IsRunning = false;

        //Assigning animation according to the speed of the player
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            MoveIntensity = 0.2f;
            animator.SetFloat("MotionBlend", MoveIntensity, 0.2f, Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.JoystickButton7))
            {
                moveSpeed = 4.5f;
  
                //MoveIntensity = Mathf.Clamp01(MoveIntensity + 0.1f * Time.deltaTime);
                MoveIntensity = 1;

                //MoveIntensity = Mathf.Clamp01(MoveIntensity);
                animator.SetFloat("MotionBlend", MoveIntensity);
                IsRunning = true;                
            }
            else
            {
                moveSpeed = 1;
            }
        }

        else
        {
            // moveSpeed = 3.5f;
            MoveIntensity = 0;
            animator.SetFloat("MotionBlend", MoveIntensity, 0.2f, Time.deltaTime);
        }

        StaminaCheck(MoveIntensity, IsRunning);
        //AnimationControlling->
        //animator.SetFloat("MotionBlend", MoveIntensity, 0.2f, Time.deltaTime);
    }
      
    void StaminaCheck(float MoveIntensity, bool IsRunning)
    {
        fill.color = StaminaGradient.Evaluate(StaminaSlide.value);
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (IsRunning && Stamina > 0)
            {
                Stamina -= 0.05f * Time.deltaTime;
                StaminaSlide.value = Stamina;

                if (Stamina < 0.3)
                {
                    moveSpeed = 4.5f;
                    MoveIntensity = 0.8f;
                    animator.SetFloat("MotionBlend", MoveIntensity, 0.2f, Time.deltaTime);
                }

                else if (Stamina >= 0.3)
                {
                    moveSpeed = 4.5f;
                    MoveIntensity = 1;
                    animator.SetFloat("MotionBlend", MoveIntensity, 0.2f, Time.deltaTime);
                }

            }
            else if (Stamina <= 0)
            {
                moveSpeed = 4.5f;
                MoveIntensity = 0.1f;
                animator.SetFloat("MotionBlend", MoveIntensity, 0.1f, Time.deltaTime);
            }
        }
        if (!IsRunning && Stamina <= 1)
        {
            IsRunning = false;
            if (    StaminaSlide.value <= StaminaSlide.maxValue)
            {
                Stamina += 0.05f * Time.deltaTime;
                StaminaSlide.value = Stamina;
            }
        }
               
    }
}
