using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : Enemy
{
    private float radius;
    private Transform gearSprite;

    private enum GearMovement {SLOW, FAST, CW, CCW}
    public float maxLinearSpd;
    private float currLinearSpeed;
    public Vector3 angleOfMovement;
    public float maxRotationSpd;
    private float currRotationSpd;
    public bool initialRotationDirection;
    private int rotationDirection;

    private float shootCooldown = 5f;
    private float shootTimer = 0f;
    private float functionCooldown = 6f;
    private float functionTimer = 0f;
    private int currFunction = 0;

    protected override void setUpBPandFUNCS(){
        gearSprite = trans.Find("Sprite");
        bp = new GearsBulletPatterns(transform, gearSprite);
        funcs = new GearsFunctions(transform);
        shootingCoroutines = new Coroutine[bp.getCount()];
        functionCoroutines = new Coroutine[funcs.getCount()];
    }
    protected override void initiateStats() {
        GameQueue.SharedInstance.isQueueing = false;
        radius = 0.75f;
        maxHealth = 1500;
        currHealth = maxHealth;
        setGearMovement(GearMovement.FAST);
        setGearMovement(initialRotationDirection ? GearMovement.CW : GearMovement.CCW);
    }
    private void setGearMovement(GearMovement gm) {
        switch(gm){
            case GearMovement.SLOW:
                currRotationSpd = maxRotationSpd;
                currLinearSpeed = maxLinearSpd / 2;
            break;
            case GearMovement.FAST:
                currRotationSpd = maxRotationSpd / 3;
                currLinearSpeed = maxLinearSpd;
            break;
            case GearMovement.CW:
                rotationDirection = 1;
                ((GearsBulletPatterns)bp).setRotationDirection(rotationDirection);
            break;
            case GearMovement.CCW:
                rotationDirection = -1;
                ((GearsBulletPatterns)bp).setRotationDirection(rotationDirection);
            break;
        }
    }

    protected override void shoot(float globalTimer) {
        if(bp.allPatternsIdle() && shootTimer >= shootCooldown){
            shootTimer = 0f;
            setGearMovement(GearMovement.SLOW);
            int bpIndex = (int)(Random.value * (bp.getCount() - 1) + 1);
            shootingCoroutines[bpIndex] = StartCoroutine(bp.runBulletPattern(bpIndex));
        }
        else if(bp.allPatternsIdle()){
            if(shootTimer == 0) setGearMovement(GearMovement.FAST);
            shootTimer += Time.deltaTime;
        }
    }
    protected override void move(float globalTimer) { //sus
        if(bp.getPatternState(1) == PatternState.IDLE && bp.getPatternState(2) == PatternState.IDLE){
            trans.position += currLinearSpeed * angleOfMovement * Time.deltaTime;
            if (trans.position.x + radius > InGameDimentions.rightEdge ||
                trans.position.x - radius < InGameDimentions.leftEdge){
                angleOfMovement.x *= -1;
                if(bp.getPatternState(0) != PatternState.FIRING) shootingCoroutines[0] = StartCoroutine(bp.runBulletPattern(0));
            }
            if (trans.position.y + radius > InGameDimentions.topEdge ||
                trans.position.y - radius < InGameDimentions.bottomEdge){
                angleOfMovement.y *= -1;
                if(bp.getPatternState(0) != PatternState.FIRING) shootingCoroutines[0] = StartCoroutine(bp.runBulletPattern(0));
            }
        }
        gearSprite.Rotate(rotationDirection * currRotationSpd * Vector3.forward * Time.deltaTime);
    }
    protected override void function(float globalTimer) {
        if(funcs.getFunction(currFunction).getCurrProcess() == FunctionProcess.UNDRAWN){
            currFunction = (int)(Random.value * funcs.getCount());
            setGearMovement(initialRotationDirection ? GearMovement.CCW : GearMovement.CW);
            if(currFunction == 0) functionCoroutines[currFunction] = StartCoroutine(funcs.getFunction(currFunction).drawFunction(600, 4f, -6f, 6f));
            else functionCoroutines[currFunction] = StartCoroutine(funcs.getFunction(currFunction).drawFunction(600, 4f, -1.7f, 2.7f));
        }
        else if(funcs.getFunction(currFunction).getCurrProcess() == FunctionProcess.IDLE){
            setGearMovement(initialRotationDirection ? GearMovement.CW : GearMovement.CCW);
            //functionCoroutines[currFunction] = StartCoroutine(funcs.getFunction(currFunction).deleteFunction(2f));
            functionCoroutines[currFunction] = StartCoroutine(funcs.getFunction(currFunction).test());
        }
    }

    protected override void OnPlayerCollision(Collider2D collision) {
        base.OnPlayerCollision(collision);
    }
}

public class GearsBulletPatterns : EnemyBulletPatterns
{
    private int rotationDirection;
    private Transform gearSprite;

    public GearsBulletPatterns(Transform transform, Transform gS) : base(transform) {
        gearSprite = gS;
    }

    private WaitForSeconds circleWait = new WaitForSeconds(0.1f);
    private WaitForSeconds spiralWait = new WaitForSeconds(0.14f);
    private WaitForSeconds succWait = new WaitForSeconds(0.4f);
    private IEnumerator makeCirclePattern()
    {
        patternStates[0] = PatternState.FIRING;
        int k = rotationDirection;
        float offset = Mathf.Deg2Rad * gearSprite.eulerAngles.z;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 24; j++)
            {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_DG, trans.position, circlePattern(i, j, k, offset));
            }
            yield return circleWait;
        }
        patternStates[0] = PatternState.IDLE;
    }
    private IEnumerator makeSpiralPattern()
    {
        patternStates[1] = PatternState.FIRING;
        int k = rotationDirection;
        for (int i = 0; i < 24; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_LG, trans.position, spiralPattern(i,j,k));
            }
            yield return spiralWait;
        }
        patternStates[1] = PatternState.IDLE;
    }
    private IEnumerator makeSuccPattern()
    {
        patternStates[2] = PatternState.FIRING;
        int k = rotationDirection;
        Vector3 succPos = trans.position;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                ProjectilePool.SharedInstance.GetPooledProjectile(gearProjPrefab_INV, succPos, succPattern(i,j,k), 0, 4);
            }
            yield return succWait;
        }
        patternStates[2] = PatternState.IDLE;
    }

    private MovePath circlePattern(int i, int j, int k, float offset)
    {
        return delegate (float t, Vector3 pos)
        {
            float rads = k * (j * Mathf.PI / 8 - i * Mathf.PI / 24) + offset;
            float distX = t * 5 * Mathf.Cos(rads);
            float distY = t * 5 * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath spiralPattern(int i, int j, int k)
    {
        return delegate (float t, Vector3 pos)
        {
            int radianCoeff = i + 1;
            float distCoeff = (25 * radianCoeff - radianCoeff * radianCoeff) / 20f;
            float rads = k * ((j * 2 * Mathf.PI / 3) + radianCoeff * Mathf.PI / 11);
            float distX = t * distCoeff * Mathf.Cos(rads);
            float distY = t * distCoeff * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }
    private MovePath succPattern(int i, int j, int k)
    {
        return delegate (float t, Vector3 pos)
        {
            float rads = k * (j - t) * Mathf.PI / 6;
            float distX = t / 3f * Mathf.Max(16f - t * t) * Mathf.Cos(rads);
            float distY = t / 3f * Mathf.Max(16f - t * t) * Mathf.Sin(rads);
            return new Vector3(pos.x + distX, pos.y + distY, pos.z);
        };
    }

    private string gearProjPrefab_DG = "Prefabs/Projectiles/Level 1/gearProjDG";
    private string gearProjPrefab_LG = "Prefabs/Projectiles/Level 1/gearProjLG";
    private string gearProjPrefab_INV = "Prefabs/Projectiles/Level 1/gearProjINV";

    public override IEnumerator makeDrops(int num)
    {
        return base.makeDrops(num);
    }

    protected override void cacheAll()
    {
        bulletPatterns.Add(makeCirclePattern);
        bulletPatterns.Add(makeSuccPattern);
        bulletPatterns.Add(makeSpiralPattern);

        for(int i = 0; i < 3; i++) patternStates.Add(PatternState.IDLE);
    }

    public void setRotationDirection(int RD)
    {
        rotationDirection = RD;
    }
}

public class GearsFunctions : EnemyFunctions
{
    public GearsFunctions(Transform transform) : base(transform)
    {
    }

    protected override void cacheAll()
    {
        /* 
        temp = (x) =>
        {
            return new Vector3(x, Mathf.Sin(x * x) / (x * Mathf.Tan(x)));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = sin(x^2)/(x*tan(x))", temp);
        eq.addDiscontinuity(new Discontinuity(0, 
        new Vector3(0, 1), 
        new Vector3(0, float.NaN), 
        new Vector3(0, 1)));
        for(int x = -10; x <= 10; x++) 
            if(x != 0) 
                eq.addDiscontinuity(new Discontinuity(x * Mathf.PI, 
                new Vector3(x * Mathf.PI, float.PositiveInfinity), 
                new Vector3(x * Mathf.PI, float.NaN), 
                new Vector3(x * Mathf.PI, float.NegativeInfinity)));
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, FunctionType.DISCONTINUOUS, Color.green));

        temp = (x) =>
        {
            return new Vector3(x, (Mathf.Pow(x, 4) - Mathf.Pow(4, x)) / Mathf.Sin(Mathf.PI * x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = (x^4-4^x)/sin(pix)", temp);
        eq.addDiscontinuity(new Discontinuity(2, 
        new Vector3(2, 32 * (1 - Mathf.Log(2)) / Mathf.PI), 
        new Vector3(2, float.NaN), 
        new Vector3(2, 32 * (1 - Mathf.Log(2)) / Mathf.PI)));
        for(int x = -10; x <= 10; x++) 
            if(x != 2) 
                eq.addDiscontinuity(new Discontinuity(x, 
                new Vector3(x, float.PositiveInfinity), 
                new Vector3(x, float.NaN), 
                new Vector3(x, float.NegativeInfinity)));
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, FunctionType.DISCONTINUOUS, Color.green));
        */

        temp = (x) =>
        {
            return new Vector3(x, Mathf.Sin(x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = sin(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, FunctionType.CONTINUOUS, Color.black));
    }
}