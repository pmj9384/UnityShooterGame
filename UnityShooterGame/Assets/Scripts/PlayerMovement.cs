using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;          
    public Camera mainCamera;                 
    public Vector3 cameraFixedPosition = new Vector3(0, 10f, -10f); 
    public Vector3 cameraRotationEuler = new Vector3(45f, 0f, 0f); 

    private Rigidbody rb;
    private Animator animator;           

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();  

        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = true;                  

        mainCamera.transform.position = cameraFixedPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotationEuler);
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");  
        float vertical = Input.GetAxis("Vertical");     


        Vector3 moveInput = (transform.right * horizontal + transform.forward * vertical).normalized;

        if (moveInput.sqrMagnitude > 0.01f)  
        {

            Vector3 moveDirection = moveInput * speed * Time.deltaTime;
            rb.MovePosition(rb.position + moveDirection);

            animator.SetBool("Walk", true);
        }
        else
        {
         
            animator.SetBool("Walk", false);
        }
        RotateTowardsMouse();

        LockCamera();
    }

    private void RotateTowardsMouse()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);  

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(enter);  
            Vector3 direction = (mouseWorldPosition - transform.position).normalized;  // 바라볼 방향
            direction.y = 0f; 

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;  // 플레이어 회전
            }
        }
    }

    private void LockCamera()
    {

        mainCamera.transform.position = cameraFixedPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotationEuler);
    }
}
