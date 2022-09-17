using UnityEngine;

public class IntVector3
{
    public readonly int x;
    public readonly int y;
    public readonly int z;

    public IntVector3(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public IntVector3(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x);
        y = Mathf.FloorToInt(pos.y);
        z = Mathf.FloorToInt(pos.z);
    }

    public override string ToString()
    {
        return x + " " + y + " " + z;
    }

    public static IntVector3 operator +(IntVector3 a, IntVector3 b)
    {
        return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static IntVector3 operator +(IntVector3 a, Vector3 b)
    {
        return new IntVector3(a.x + Mathf.FloorToInt(b.x), a.y + Mathf.FloorToInt(b.y), a.z + Mathf.FloorToInt(b.z));
    }

    public override bool Equals(object obj) => this.Equals(obj as IntVector3);

    public bool Equals(IntVector3 p)
    {
        if (p is null || this.GetType() != p.GetType())
            return false;
        if (Object.ReferenceEquals(this, p))
            return true;

        return x == p.x && y == p.y && z == p.z;
    }

    public override int GetHashCode() => (x, y, z).GetHashCode();

    public static bool operator ==(IntVector3 a, IntVector3 b)
    {
        return b is not null && a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(IntVector3 a, IntVector3 b)
    {
        return b is null || !(a.x == b.x && a.y == b.y && a.z == b.z);
    }

    public static IntVector3 operator *(IntVector3 a, int b)
    {
        return new IntVector3(a.x * b, a.y * b, a.z * b);
    }

    public static IntVector3 operator -(IntVector3 a, IntVector3 b)
    {
        return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }
}