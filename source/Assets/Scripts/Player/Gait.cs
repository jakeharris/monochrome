using UnityEngine;
using System.Collections;

public class Gait : MonoBehaviour
{
    private bool isRunning = false;

    public bool isHeadBobbing = true;
    [System.Serializable]
    public class HeadBobbing
    {
        private Camera c;
        private float timer = 0.0f;
        public float intensity = 0.2f;
        public float speed = 0.15f;
        public float runSpeed = 0.12f;
        public float midpoint = 1.8f;

        public void Setup(Camera c)
        {
            this.c = c;
        }

        public void Bob(bool isRunning) {
            float waveslice = Mathf.PI / 2;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
            {
                timer = Mathf.PI / 2;
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                if (isRunning) timer += runSpeed;
                else timer += speed;
                if (timer > 2 * Mathf.PI)
                {
                    timer = timer - (2 * Mathf.PI);
                }
            }
            if (waveslice != 0)
            {
                var translateChange = waveslice * intensity;
                var totalAxes = Mathf.Abs(h) + Mathf.Abs(v);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                c.transform.localPosition = new Vector3(
                                    c.transform.localPosition.x,
                                    midpoint + translateChange,
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

    public HeadBobbing headbobbing = new HeadBobbing();

    void Awake()
    {
        headbobbing.Setup(this.gameObject.GetComponent<Camera>());
    }

    void Update()
    {
        if (Input.GetAxis("Run") > 0)
            isRunning = true;
        else 
            isRunning = false;

        // if isrunning, change character motor max speeds

        headbobbing.Bob(isRunning);
    }
}
