package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class IfElse extends If {
  public Else s2;

  public IfElse(Exp ae, Statement as1, Else as2, Location pos) {
    super(pos);
    cond=ae; 
    s1=as1; 
    s2=as2;
  }

  public void accept(Visitor v) {
    v.visit(this);
  }
}

