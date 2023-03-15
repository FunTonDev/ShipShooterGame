using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Reimplement buffs as independently controlled from player prefab
public class Buff {
    protected string buffId = "Empty";
    protected AudioClip audioclip;

    public Buff() {
        init("Empty");
    }

    protected void init(string newId) {
        buffId = newId;
        audioclip = Resources.Load<AudioClip>("Audio/buff" + buffId);
    }

    public virtual AudioClip activate() {
        Debug.Log(string.Format("Activated buff: {0}", buffId));
        return audioclip;
    }   
}

public class ShockwaveBuff : Buff {
    public ShockwaveBuff() {
        init("Shockwave");
    }

    public override AudioClip activate() {
        GameManager.instance.currentPlayer.GetComponent<PlayerShip>().activateShockwave();
        return base.activate();
    }
}

public class MachinegunBuff : Buff {
    public MachinegunBuff() {
        init("Machinegun");
    }

    public override AudioClip activate() {
        GameManager.instance.currentPlayer.GetComponent<PlayerShip>().activateMachinegun();
        return base.activate();
    }
}

public class ShieldBuff : Buff {
    public ShieldBuff() {
        init("Shield");
    }

    public override AudioClip activate() {
        GameManager.instance.currentPlayer.GetComponent<PlayerShip>().enableShield(true);
        return base.activate();
    }
}

public class PlayerBuff : MonoBehaviour {
    private Buff buff;
    private AudioSource buffAudiosource;

    public void Awake() {
        switch(Random.Range(1, 4)) {
            case 1:
                buff = new ShockwaveBuff();
                break;
            case 2:
                buff = new MachinegunBuff();
                break;
            default:
                buff = new ShieldBuff();
                break;
        }
        buffAudiosource = gameObject.AddComponent<AudioSource>();
        buffAudiosource.PlayOneShot(Resources.Load<AudioClip>("Audio/spawnBuff"), 0.8f);
        StartCoroutine(Common.lerpAlpha(transform.Find("Sprite").GetComponent<SpriteRenderer>(), 8.0f, false));
        Destroy(gameObject, 8.0f);
        Debug.Log(string.Format("Powerup created: {0}", buff));
    }

    public Buff getBuff() {
        return buff;
    }
}
