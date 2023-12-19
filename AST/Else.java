package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class Else extends Statement {
  public Statement s1;

  public Else(Statement s, Location pos) {
    super(pos);
    s1=s;
  }

  public void accept(Visitor v) {
    v.visit(this);
  }
}

