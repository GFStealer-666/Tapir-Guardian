// Assets/Scripts/AI/EnemyController2D.cs
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(StateMachine))]
public class EnemyController : MonoBehaviour
{
    [Header("Composition")]
    [SerializeField] private EnemyRoot root; // holds IMover2D + IHealth + Target
    [SerializeField] private IEnemyAttack attack;           // inject at Awake (melee or ranged)
    [SerializeField] private IAttackRangeProvider rangeProv; // inject at Awake

    [Header("Perception")]
    [SerializeField] private float sightRadius = 8f;
    [SerializeField] private float fov = 110f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Patrol (optional)")]
    public Transform[] waypoints;
    [SerializeField] private float waypointTolerance = 0.2f;
    [SerializeField] private float waitAtPoint = 0.5f;

    [Header("Chase")]
    [SerializeField] private float stopDistance = 0.8f;
    [SerializeField] private float loseTargetAfter = 2.0f;

    public float SightRadius => sightRadius;
    public float Fov => fov;
    private StateMachine sm;
    private int patrolIndex;
    private float loseTimer;

    void Awake()
    {
        sm = GetComponent<StateMachine>();
        root = root ? root : GetComponent<EnemyRoot>();

        // Inject concrete behavior present on this GO (or children/parent)
        attack = attack ?? GetComponent<IEnemyAttack>() ?? GetComponentInChildren<IEnemyAttack>() ?? GetComponentInParent<IEnemyAttack>();
        rangeProv = rangeProv ?? GetComponent<IAttackRangeProvider>() ?? GetComponentInChildren<IAttackRangeProvider>() ?? GetComponentInParent<IAttackRangeProvider>();

        if (root?.Mover == null) Debug.LogError("EnemyController2D: IMover2D missing.");
        if (attack == null || rangeProv == null) Debug.LogError("EnemyController2D: attack behaviour missing.");
    }

    void OnEnable() => sm.ChangeState(new Idle(this));

    // -------- States --------
    abstract class State : IAIState
    {
        protected readonly EnemyController c;
        protected State(EnemyController c) { this.c = c; }
        public virtual void OnEnter() { }
        public virtual void Tick(float dt) { }
        public virtual void OnExit() { }
        protected bool SeeTarget()
        {
            if (!c.root || !c.root.Target) return false;
            float dist = Vector2.Distance(c.transform.position, c.root.Target.position);
            if (dist > c.sightRadius) return false;

            var to = (c.root.Target.position - c.transform.position);
            if (Vector2.Angle(c.transform.right, to) > c.fov * 0.5f) return false;

            return !Physics2D.Raycast(c.transform.position, to.normalized, to.magnitude, c.obstacleMask);
        }
    }

    class Idle : State
    {
        float t = 0.1f;
        public Idle(EnemyController c) : base(c) { }
        public override void Tick(float dt)
        {
            t -= dt;
            if (SeeTarget()) { c.sm.ChangeState(new Chase(c)); return; }
            if (t <= 0f) c.sm.ChangeState(new Patrol(c));
        }
    }

    class Patrol : State
    {
        float wait;
        public Patrol(EnemyController c) : base(c) { }
        public override void OnEnter() { wait = 0f; }
        public override void Tick(float dt)
        {
            if (SeeTarget()) { c.sm.ChangeState(new Chase(c)); return; }

            if (c.waypoints == null || c.waypoints.Length == 0)
            {
                c.root.Mover.Move(Vector2.zero);
                return;
            }
            var wp = c.waypoints[c.patrolIndex];
            var pos = (Vector2)c.transform.position;
            var dir = ((Vector2)wp.position - pos);
            if (dir.magnitude <= c.waypointTolerance)
            {
                c.root.Mover.Move(Vector2.zero);
                wait -= dt;
                if (wait <= 0f) { c.patrolIndex = (c.patrolIndex + 1) % c.waypoints.Length; wait = c.waitAtPoint; }
                return;
            }
            dir.Normalize();
            c.root.Mover.Move(dir);
        }
        public override void OnExit() { c.root.Mover.Move(Vector2.zero); }
    }

    class Chase : State
    {
        public Chase(EnemyController c) : base(c) { }
        public override void OnEnter() { c.loseTimer = c.loseTargetAfter; }

        public override void Tick(float dt)
        {
            if (c.root.Target == null) { c.sm.ChangeState(new Patrol(c)); return; }

            if (SeeTarget())
            {
                c.loseTimer = c.loseTargetAfter;

                // use attack origin if provided
                Vector2 origin = (c.attack is IAttackOriginProvider op)
                    ? op.GetOrigin(c.transform)
                    : (Vector2)c.transform.position;

                Vector2 tp = TargetPoint(c.root.Target, origin);
                float dist = Vector2.Distance(origin, tp);
                float atkRange = c.rangeProv.AttackRange;

                if (dist <= Mathf.Max(atkRange, c.stopDistance))
                {
                    c.root.Mover.Move(Vector2.zero);
                    c.sm.ChangeState(new Attack(c));
                    return;
                }

                // ✅ move toward target until within range
                Vector2 dir = ((Vector2)c.root.Target.position - (Vector2)c.transform.position);
                if (dir.sqrMagnitude > 0.0001f)
                {
                    dir.Normalize();
                    c.root.Mover.Move(dir);
                }
            }
            else
            {
                // lost LOS, chase a bit toward last known position until timer expires
                c.loseTimer -= dt;
                if (c.loseTimer <= 0f) { c.sm.ChangeState(new Patrol(c)); return; }

                Vector2 dir = ((Vector2)c.root.Target.position - (Vector2)c.transform.position);
                if (dir.sqrMagnitude > 0.0001f)
                {
                    dir.Normalize();
                    c.root.Mover.Move(dir);
                }
            }
        }

        public override void OnExit() { c.root.Mover.Move(Vector2.zero); }
    }

    class Attack : State
    {
        public Attack(EnemyController c) : base(c) { }
        public override void Tick(float dt)
        {
            if (!c.root.Target) { c.sm.ChangeState(new Patrol(c)); return; }

            // face target (optional)
            Vector2 to = c.root.Target.position - c.transform.position;
            if (to.sqrMagnitude > 0.001f) c.transform.right = to.normalized;

            // measure from origin (hand) if available
            Vector2 origin = (c.attack is IAttackOriginProvider op) ? op.GetOrigin(c.transform)
                                                      : (Vector2)c.transform.position;

            Vector2 tp = TargetPoint(c.root.Target, origin);
            float dist = Vector2.Distance(origin, tp);
            float atkRange = c.rangeProv.AttackRange;

            if (dist > atkRange * 1.02f) // small hysteresis
            {
                c.sm.ChangeState(new Chase(c));
                return;
            }

            bool fired = c.attack.TryAttack(c.transform, c.root.Target);
            if (!fired) c.root.Mover.Move(Vector2.zero);
        }
    }
    static Vector2 TargetPoint(Transform target, Vector2 from)
    {
        if (target && target.TryGetComponent<Collider2D>(out var col))
            return col.ClosestPoint(from);
        return (Vector2)target.position;
    }
}
