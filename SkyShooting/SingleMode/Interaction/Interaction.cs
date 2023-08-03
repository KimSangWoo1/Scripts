using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MessageSender))]
public partial class Interaction : MonoBehaviour
{
    private MessageSender messageSender;
    private PlaneBase planeBase;

    private void Awake()
    {
        planeBase = GetComponent<PlaneBase>();
    }
    private void Start()
    {
        messageSender = GetComponent<MessageSender>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            messageSender.Self_Destruction();
        }

        if (!planeBase.ghostMode)
        {
            if (other.gameObject.tag == "Bullet")
            {
                messageSender.ApplyDamage(other.gameObject.GetComponent<BulletController>().Get_ProfileName());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!planeBase.ghostMode)
        {
            if (collision.gameObject.tag == "Player")
            {
                messageSender.Self_Destruction();
            }
            else if (collision.gameObject.tag == "AI")
            {
                messageSender.Self_Destruction();
            }
            else if (collision.transform.tag == "Item_Bullet")
            {
                planeBase.FXM.FX_ItemPop(transform);
                messageSender.Apply_AddBullet();
            }
            else if (collision.transform.tag == "Item_Muzzle")
            {
                planeBase.FXM.FX_ItemPop(transform);
                messageSender.Apply_AddMuzzle();
            }
            else if (collision.transform.tag == "Item_Turbin")
            {
                planeBase.FXM.FX_ItemPop(transform);
                messageSender.Apply_AddTurbin();

            }
            else if (collision.transform.tag == "Item_Health")
            {
                planeBase.FXM.FX_ItemPop(transform);
                messageSender.Apply_AddHealth(collision.transform.GetComponent<ItemControl>().healthState);
            }
            else if (collision.transform.tag == "Item_Dollar")
            {
                planeBase.FXM.FX_MoneyPop(transform);
                messageSender.Apply_AddMoney(collision.transform.GetComponent<ItemControl>().dollarState);

            }
        }

        if (collision.gameObject.tag == "Wall")
        {
            messageSender.Self_Destruction();
        }
    }
}
