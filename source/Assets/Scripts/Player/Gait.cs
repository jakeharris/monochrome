using UnityEngine;
using System.Collections;

public class Gait : MonoBehaviour
{
    private bool isRunning = false;
    private CharacterMotor cm;

    public bool isHeadBobbing = true;
    [System.Serializable]
    public class HeadBobbing
    {
        private Camera c;
        private float timer = 0.0f;
        public float xIntensity = 0.2f;
        public float yIntensity = 0.2f;
        public float speed = 0.15f;
        public float midpoint = 1f;

        public void Setup(Camera c)
        {
            this.c = c;
        }

        public void Bob(bool isRunning) {
            float xWaveslice = Mathf.PI / 2;
            float yWaveslice = Mathf.PI / 2;

            if (!isMoving())
            {
                xWaveslice = Mathf.PI / 2;
                yWaveslice = Mathf.PI / 2;
            }
            else
            {
                xWaveslice = Mathf.Sin((timer + Mathf.PI)/2);
                yWaveslice = Mathf.Sin(timer);
                float effectiveSpeed = speed;

                if (timer < Mathf.PI / 2 || timer > (3 * Mathf.PI / 2))
                    effectiveSpeed = speed / 1.2f;
                if (isRunning)
                    timer += (4 / 3) * effectiveSpeed;
                else
                    timer += effectiveSpeed;

                if (timer > 4 * Mathf.PI)
                    timer = timer - (4 * Mathf.PI);
            }
            if (xWaveslice != 0 && yWaveslice != 0)
            {
                var xEffectiveIntensity = xIntensity;
                var yEffectiveIntensity = yIntensity;
                if (isRunning) xEffectiveIntensity *= (2 / 3);
                if (isRunning) yEffectiveIntensity *= (2 / 3);
                var xTranslateChange = xWaveslice * xIntensity;
                var yTranslateChange = yWaveslice * yIntensity;
                var totalAxes = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                yTranslateChange = totalAxes * yTranslateChange;
                c.transform.localPosition = new Vector3(
                                    0 + xTranslateChange,
                                    midpoint + yTranslateChange,
                                    c.transform.localPosition.z
                                );
            }
            else
            {
                c.transform.localPosition = new Vector3(
                                    c.transform.localPosition.x,
                                    midpoint,
                                    c.transform.localPosition.z
                                );
            }
        }
    }
    [System.Serializable]
    public class Footsteps
    {
        private bool wasRunning = false;
        private bool wasMoving = false;

        public float walkVolume = 1f;
        public float runVolume = 1f;

        private AudioSource src;
        private CharacterMotor cm;
        // Add one of each for different floors?
        // Check e.g. https://www.youtube.com/watch?v=U2kLPkpIkw0
        public AudioClip walkCycle;
        public AudioClip runCycle;

        public void Setup(AudioSource src, CharacterMotor cm)
        {
            this.src = src;
            this.cm = cm;
            src.clip = walkCycle;
        }

        public void Update(bool isRunning)
        {
            if (!isMoving() || !cm.IsGrounded())
            {
                src.Stop();
                wasMoving = false;
            }
            else if (isMoving() && !wasMoving)
            {
                wasMoving = true;

                if (isRunning)
                    src.clip = runCycle;
                else
                    src.clip = walkCycle;

                src.Play();
            }

            if (isRunning && !wasRunning)
            {
                src.clip = runCycle;
                src.Play();
                wasRunning = true;
            }
            if (!isRunning && wasRunning)
            {
                src.clip = walkCycle;
                src.Play();
                wasRunning = false;
            }
        }
    }
    [System.Serializable]
    public class MoveSpeed
    {
        public float walkSpeed = 5f;
        public float runSpeed = 9f;
    }

    public HeadBobbing headbobbing = new HeadBobbing();
    public Footsteps footsteps = new Footsteps();
    public MoveSpeed movespeed = new MoveSpeed();

    void Awake()
    {
        cm = GetComponent<CharacterMotor>();
        headbobbing.Setup(GetComponentInChildren<Camera>());
        footsteps.Setup(GetComponent<AudioSource>(), cm);
    }

    void Update()
    {
        if (Input.GetAxis("Run") > 0 && Input.GetAxis("Vertical") > 0 && cm.IsGrounded())
            isRunning = true;
        else 
            isRunning = false;

        // if is running, change character motor max speeds
        if (isRunning && cm.IsGrounded()) cm.movement.maxForwardSpeed = movespeed.runSpeed;
        else if (cm.IsGrounded()) cm.movement.maxForwardSpeed = movespeed.walkSpeed;

        // bob head, if necessary
        if (isHeadBobbing) headbobbing.Bob(isRunning);
        // play footstep noises
        footsteps.Update(isRunning);
    }

    static bool isMoving()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        return !(Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0);
    }

}
