using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersEffects : MonoBehaviour
{
    public struct Effect
    {
        public string effectName;
        public int actorNumberOwner;
        public float timeStamp;

        public Effect(string effectName, int actorNumberOwner)
        {
            this.effectName = effectName;
            this.actorNumberOwner = actorNumberOwner;
            this.timeStamp = Time.time;
        }
    }

    public static PlayersEffects _instance;
    List<Effect> effects;

    void Start()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        effects = new List<Effect>();
    }
    
    [PunRPC]
    public void AddEffect(string effectName, int actorNumberOwner)
    {
        Effect effect = new Effect(effectName, actorNumberOwner);

        effects.Add(effect);
    }

    public List<Effect> GetActiveEffects(int actorNumber)
    {
        List<Effect> activeEffects = new List<Effect>();

        foreach(var effect in effects)
        {
            if(Time.time - effect.timeStamp > 6f)
            {
                continue;
            }
            if(effect.actorNumberOwner == actorNumber)
            {
                continue;
            }

            activeEffects.Add(effect);
        }

        return activeEffects;
    }
}
