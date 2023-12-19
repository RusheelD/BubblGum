package AST;

import java.util.List;

import AST.Visitor.Visitor;

import java.util.ArrayList;
import java_cup.runtime.ComplexSymbolFactory.Location;

public class StatementList extends Statement {
   private List<Statement> list;

   public StatementList(Location pos) {
      super(pos);
      list = new ArrayList<Statement>();
   }

   public void add(Statement n) {
      list.add(n);
   }

   public Statement get(int i)  { 
      return list.get(i); 
   }

   public int size() { 
      return list.size(); 
   }

  public void accept(Visitor v) {
    v.visit(this);
  }
}
