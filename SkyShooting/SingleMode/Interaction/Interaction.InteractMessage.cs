using UnityEngine;

public partial class Interaction : MonoBehaviour
{
    public struct InteractMessage{
       public int amount;
       public bool upgrade;
    }

    public struct DamageMessage
    {
        public string name;
        public int damage;
    }

}
