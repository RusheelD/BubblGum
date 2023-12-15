package AST;

import AST.Visitor.Visitor;
import java_cup.runtime.ComplexSymbolFactory.Location;

// empty class great for debugging when writing in the Cup file
public class Test extends Program {
  
  public Test(Location pos) {super(null, null, pos);}

  public void accept(Visitor v) {
    v.visit(this);
  }
}
