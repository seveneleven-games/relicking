using UnityEngine;
using static Define;

public class BaseController : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

    public float ColliderRadius { get { return Collider?.radius ?? 0.0f; } }
    public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        float direction = _lookLeft ? 1f : -1f;
        transform.localScale = new Vector3(_initialScale.x * direction, _initialScale.y, _initialScale.z);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        Animator = GetComponent<Animator>();
        RigidBody = GetComponent<Rigidbody2D>();

        return true;
    }

    public void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);

        if (dir.x < 0)
            LookLeft = true;
        else if (dir.x > 0)
            LookLeft = false;
    }

    protected virtual void UpdateAnimation()
    {
    }
    
}