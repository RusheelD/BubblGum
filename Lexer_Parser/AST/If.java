package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public abstract class If extends Statement {
  public Exp cond;
  public Statement s1;
  public If(Location pos) {
    super(pos);
  }
  public abstract void accept(Visitor v);
}

