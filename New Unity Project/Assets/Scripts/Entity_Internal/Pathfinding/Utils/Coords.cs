public struct Coords
{
    public int x;
    public int y;

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public static bool operator ==(Coords a, Coords b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Coords a, Coords b)
    {
        return a.x != b.x || a.y != b.y;
    }
    public static Coords operator +(Coords a, Coords b)
    {
        Coords out_ = new Coords(a.x + b.x, a.y + b.y);
        return out_;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
