namespace SpringSystem
{
    public class Vector
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static double operator *(Vector left, Vector right)
        {
            return left.X*right.X + left.Y*right.Y + left.Z*right.Z;
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return new Vector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            return new Vector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector operator -(Vector left)
        {
            return new Vector(-left.X, -left.Y, -left.Z);
        }

        public static Vector operator *(Vector left, double mult)
        {
            return new Vector(left.X*mult, left.Y*mult, left.Z*mult);
        }

        public static Vector operator /(Vector left, double div)
        {
            return new Vector(left.X / div, left.Y / div, left.Z / div);
        }

    }
}