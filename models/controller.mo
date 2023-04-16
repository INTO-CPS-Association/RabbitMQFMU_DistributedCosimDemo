model controller
  input Real x;
  parameter Real set_x = 1.0;
  output Real fk;
  
  Modelica.Blocks.Continuous.LimPID pid(k=1);
  
equation
  
  pid.u_m = x;
  
  pid.u_s = set_x;
  
  fk = pid.y;
  
end controller;
