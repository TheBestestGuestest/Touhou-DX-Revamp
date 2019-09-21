using UnityEngine;

public class Drop : Projectile {
    private GameObject player;
    private Transform playerTransform;
    private Effect effect;

    private bool followingPlayer = false;
    private float followTime;

    public static Drop Create(string prefab, Vector3 pos, Effect e) {
        Drop drop = (Instantiate((GameObject)Resources.Load(prefab), pos, Quaternion.identity, ProjectilePool.SharedInstance.transform) as GameObject).GetComponent<Drop>();
        //drop.effect = e;
        drop.name = prefab.Substring(prefab.LastIndexOf("/") + 1);  //optimize???
        drop.destroyTime = 5f;
        return drop;
    }
    public void setValues(Vector3 pos) {
        trans.position = pos;
    }
    new void OnEnable(){
        base.OnEnable();
        followingPlayer = false;
        followTime = 0f;
    }
    new void Awake() {
        base.Awake();

        player = GameObject.Find("Player");
        if (player != null) playerTransform = player.transform;

        path = (float t, Vector3 pos) => {
            Vector3 temp = trans.position;
            Vector3 lookPos = temp - playerTransform.position;
            float dist = Vector3.Distance(temp, playerTransform.position);

            if(dist < 2f && !followingPlayer){
                followingPlayer = true;
                followTime = t;
            }
            if(followingPlayer){
                Vector3 travel = lookPos * (t - followTime + 0.4f) * (t - followTime + 0.4f);
                temp -= Vector3.Magnitude(travel) > dist ? lookPos : travel;
            }
            else{
                temp += new Vector3(0, Mathf.Max(-0.064f, -0.32f * t * t + 0.064f * t + 0.064f), 0);
            }

            return temp;
        };
    }

    new void Update() {
        if (player != null) {
            base.Update();
        }
        else {
            ProjectilePool.SharedInstance.ReturnToPool(gameObject);
        }
    }
}
public delegate void Effect();