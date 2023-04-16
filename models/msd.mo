model msd
  parameter Real c1 = 1.0;
  parameter Real d1 = 1.0;
  parameter Real m1 = 1.0;
  output Real x(start = 0);
  output Real v(start = 0);
  input Real fk;
equation
  der(x) = v;
  der(v) = 1 / m1 * ((-c1 * x) - d1 * v + fk);
end msd;
