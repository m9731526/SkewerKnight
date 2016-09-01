using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public enum CharacterState {
        Normal,
        Jumping,
        Bending
    }

    CharacterState State;
    
	public bool moving;
	public float jspeed = 0f;
	public float top = 2f;
	public float gravity = 7f;
	public float JumpPower = 10f;
	public float GrY;
	public Animator animator;
    public bool ToggleSwitch = true;

    public GameObject Spear;

	// Use this for initialization
	void Start () {
		State = CharacterState.Normal;
		moving = false;
		animator = GetComponent<Animator>();

        GrY = WorldManager.Instance.GroundY;

        Spear = GetComponentInChildren<Spear>().gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		//basic move
		transform.Translate(Vector3.right * GameManager.Instance.ScrollSpeed * Time.deltaTime);
        //end basic move
        
        //input
        if (Input.GetKeyDown(KeyCode.Space) && State == CharacterState.Normal) {
            GameManager.Instance.MainCamera.GetComponent<CameraController>().LockY = true;
            State = CharacterState.Jumping;
            jspeed = JumpPower;
			transform.Translate(Vector3.up * (jspeed - gravity) * Time.deltaTime);
            animator.SetBool("Jump", true);
        }
		else if(Input.GetKey(KeyCode.LeftShift) && State != CharacterState.Jumping)
        {
            if (State == CharacterState.Bending && ToggleSwitch)
            {
                ToggleSwitch = false;
                Spear.GetComponent<Spear>().ToggleLock();
            }
            State = CharacterState.Bending;
            animator.SetBool("Bending", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ToggleSwitch = true;
            Spear.GetComponent<Spear>().ToggleLock();
        }
        else
        {
            if (State == CharacterState.Bending)
            {
                if (State == CharacterState.Normal) Spear.GetComponent<Spear>().ToggleLock();
                State = CharacterState.Normal;
                animator.SetBool("Bending", false);
                moving = false;
            }
        }
	}

	void FixedUpdate() {
		if(transform.position.y > GrY) {
			transform.Translate(Vector3.up * (jspeed - gravity) * Time.deltaTime);
		}
		if(State == CharacterState.Jumping) {
			jspeed -= 0.14f;
            if (this.transform.position.y <= GrY) //著地
            {
                jspeed = 0;
                GameManager.Instance.MainCamera.GetComponent<CameraController>().LockY = false;
                State = CharacterState.Normal;
                animator.SetBool("JumpBack", false);
				transform.position = new Vector2(transform.position.x,GrY);
			}
            else if(jspeed <= gravity) //當腳色跳到最高時
            {
                animator.SetBool("JumpBack", true);
                animator.SetBool("Jump", false);
            }
			
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 8)
        {
            if (other.tag == "Gate")
            {
                if (State != CharacterState.Bending) GameManager.Instance.GameOver();
            } else GameManager.Instance.GameOver();
        } 
    }
    

}
