package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class EqualSign extends Exp {
  public Exp e1, e2;
  
  public EqualSign(Exp e1, Exp e2, Location pos) {
    super(pos);
    this.e1 = e1;
    this.e2 = e2;
  }
  public void accept(Visitor v) {
    v.visit(this);
  } 
}

