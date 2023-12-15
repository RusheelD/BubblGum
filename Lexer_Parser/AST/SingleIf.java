package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class SingleIf extends If {

  public SingleIf(Exp ae, Statement s, Location pos) {
    super(pos);
    cond=ae; 
    s1=s;
  }

  public void accept(Visitor v) {
    v.visit(this);
  }
}

