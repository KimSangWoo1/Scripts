using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletController : MonoBehaviour
{
    private Profile profile;

    private BulletManager BM;
    private DataManager DM;

    private AudioSource audioSource;
    [SerializeField]
    private float bulletSpeed; //총알 속도
    private float timeSpeed; //생명 시간 속도
    private float lifeTime; //생명 시간
    private float deadTime; //죽는 시간

    private bool pop;

    private void Awake()
    {
        //싱글톤 생성
        BM = BulletManager.Instance;
        DM = DataManager.Instance;

        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        if (DM.GetSFSXSound())
        {
            audioSource.Play();
        }

        pop = false;
        lifeTime = 0f;
    }

    void Start()
    {
        bulletSpeed = 90f;
        timeSpeed = 2f;
        deadTime = 1.5f;
    }

    void Update()
    {
        if (!pop)
        {
            StartCoroutine(BulletLife());
        }
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed, Space.Self);
    }

    IEnumerator BulletLife()
    {
        //총 생명 시간 1초   
        lifeTime += Time.deltaTime * timeSpeed;
        yield return new WaitUntil(() => lifeTime > deadTime);

        lifeTime = 0f;  //초기화
        BM.bullet_Control(this.gameObject); //Push 및 active 설정
        
        pop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            BM.bullet_Control(this.gameObject); //Push 및 active 설정
        }
        else if (other.gameObject.tag == "AI")
        {
            BM.bullet_Control(this.gameObject); //Push 및 active 설정
        }
        else if (other.gameObject.tag == "Obstacle")
        {
            BM.bullet_Control(this.gameObject); //Push 및 active 설정
        }
    }
    
    public void Set_PlayerProfile(Profile _profile)
    {
        profile = _profile;
    }

    public string Get_ProfileName()
    {
        return profile.name;
    }

}
