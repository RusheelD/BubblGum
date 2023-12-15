package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class TypedAssign extends Statement {
  public Identifier i;
  public Exp e;
  public Type t;

  public TypedAssign(Type ty, Identifier ai, Exp ae, Location pos) {
    super(pos);
    i=ai; e=ae; t = ty; 
  }

  public void accept(Visitor v) {
    v.visit(this);
  }
}
