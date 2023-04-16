model coupled_system

  msd m;
  
  controller ctrl;

equation

  m.fk = ctrl.fk;
  
  ctrl.x = m.x;

end coupled_system;
