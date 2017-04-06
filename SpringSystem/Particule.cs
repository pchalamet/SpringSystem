using System;
using System.Collections.Generic;

namespace SpringSystem
{
    public class Spring
    {
        public double Length { get; private set; }
        public double Stiffness { get; private set; }

        public Spring(double length, double stiffness)
        {
            Length = length;
            Stiffness = stiffness;
        }
    }

    public class Particule
    {
        private readonly bool _fix;
        private readonly List<Spring> _springs = new List<Spring>();
        private Vector _speed = new Vector(0, 0, 0);
        private Vector _force;
        private Vector _position;
        private readonly double _weight;

        public Vector Position
        {
            get { return _position; }
        }

        public double Weight
        {
            get { return _weight; }
        }

        public List<Particule> Links { get; private set; }

        public Particule(bool fix, double weight, Vector position)
        {
            _position = position;
            _fix = fix;
            _weight = weight;
            Links = new List<Particule>();
        }

        public Particule Link(Particule particule, Spring spring)
        {
            Links.Add(particule);
            _springs.Add(spring);

            particule.Links.Add(this);
            particule._springs.Add(spring);

            return this;
        }

        public void Apply()
        {
            var viscosity = .1;
            _force = new Vector(0, 0, 0);
            for (var i = 0; i < _springs.Count; ++i)
            {
                var l = Links[i].Position - Position;
                var norm = Math.Sqrt(l * l);

                var coef = 1.0 - _springs[i].Length / norm;

                var recovery = l * _springs[i].Stiffness * (1.0 - _springs[i].Length / norm);
                var gravity = new Vector(0, 0, _weight * 9.8);
                var friction = _speed * viscosity;
                _force += recovery + gravity - friction;
            }
        }

        public void Move(double dt)
        {
            if (_fix || dt == 0.0)
                return;

            var acceleration = _force / _weight / dt;
            _speed += acceleration * dt;
            _position += _speed * dt * 0.5;
        }
    }
}