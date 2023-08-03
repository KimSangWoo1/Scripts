using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector3 Amount = new Vector3(1f, 1f, 0);

    public float Duration = 1;
    public float Speed = 10;

    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public bool DeltaMovement = true;

    [SerializeField]
    private float time = 0;
    [SerializeField]
    private float shakeTime;

    private Camera cam;
    protected Vector3 lastPos;
    protected Vector3 nextPos;
    protected float lastFoV;
    protected float nextFoV;
    protected bool destroyAfterPlay;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        time = 0f;
    }

    private void LateUpdate()
    {
        time += Time.deltaTime;
        if (time < shakeTime)
        {
            //next position based on perlin noise
            nextPos = (Mathf.PerlinNoise(time * Speed, time * Speed * 2) - 0.5f) * Amount.x * transform.right * Curve.Evaluate(1f - time / Duration) +
                      (Mathf.PerlinNoise(time * Speed * 2, time * Speed) - 0.5f) * Amount.y * transform.up * Curve.Evaluate(1f - time / Duration);
            nextFoV = (Mathf.PerlinNoise(time * Speed * 2, time * Speed * 2) - 0.5f) * Amount.z * Curve.Evaluate(1f - time / Duration);


            cam.fieldOfView += (nextFoV - lastFoV);
            cam.transform.Translate(DeltaMovement ? (nextPos - lastPos) : nextPos);

            lastPos = nextPos;
            lastFoV = nextFoV;

        }
        else
        {
            ResetCam();
        }
    }

    private void ResetCam()
    {
        //reset the last delta
        cam.transform.Translate(DeltaMovement ? -lastPos : Vector3.zero);
        cam.fieldOfView -= lastFoV;

        //clear values
        lastPos = nextPos = Vector3.zero;
        lastFoV = nextFoV = 0f;

        time = 0f;

        enabled = false;
    }
}
